using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackController : MonoBehaviour
{
    [SerializeField] private PropertyController.Team teamToAttack = PropertyController.Team.Player;
    
    [Tooltip("NO LONGER IN USE. ANIMATION DEFINES CD NOW")] [SerializeField]
    private float cooldown = 1;

    [Tooltip("Damage (or healing if negative)")] [SerializeField]
    private int damage = 1;

    [Tooltip("Attack range")] [SerializeField]
    public int range = 1;

    [Tooltip("The cost of the attack")]
    [SerializeField] private int attackCost = 0;

    [SerializeField] private bool targetOnlyEnergySources = false;

    private List<HealthController> detectedAttackables = new();

    private bool running_attackCoroutine = false;

    [SerializeField] private StateController stateController;

    public HealthController target { get; private set; }

    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileSpawnLocation;
    private bool attacksWithProjectile => projectile != null && projectileSpawnLocation != null;
    [SerializeField] private float maxProjectileOffset = 0.15f;
    private RandomEssentials rng = new RandomEssentials();
    [SerializeField] private MapElement mapElementRef;
    [SerializeField] private bool attackWithAnimation = false;

    // private PoolEssentials projectilePool;
    // private int projectilePoolSize => Mathf.CeilToInt(2.5f / cooldown);

    // protected void Start()
    // {
    //     //base.Start();
    //     if (attacksWithProjectile)
    //         projectilePool = new PoolEssentials(projectile, projectilePoolSize);
    // }

    private void UpdateTarget()
    {
        if (detectedAttackables.Contains(target) && target.health > 0) // Keep the target if a valid one is selected
            return;

        List<HealthController> toRemove = new();
        bool targetFound = false;

        foreach (HealthController targetCandidate in detectedAttackables)
        {
            if (targetCandidate.health <= 0)
            {
                toRemove.Add(targetCandidate);
                continue;
            }

            if (targetOnlyEnergySources && !targetCandidate.isEnergySource)
                continue;
            
            if (target != targetCandidate )
            {
                target = targetCandidate;
                stateController.SetNewState();
            }

            targetFound = true;
            break;
        }

        foreach (HealthController element in toRemove)
        {
            detectedAttackables.Remove(element);
        }

        if (!targetFound)
        {
            // Debug.LogWarning($"No targets found. attackables list length = {detectedAttackables.Count}", this);
            target = null;
            stateController.SetNewState();
        }

        return;
    }


    private void OnEnable()
    {
        // if (attacksWithProjectile && projectilePool == null)
        //     projectilePool = new PoolEssentials(projectile, projectilePoolSize);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        foreach (Collider hitCollider in hitColliders)
        {
            RegisterAsAttackable(hitCollider);
        }
    }

    private void OnDisable()
    {
        detectedAttackables = new();
        UpdateTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        RegisterAsAttackable(other);
    }

    private void RegisterAsAttackable(Collider collider)
    {
        if (collider.isTrigger)
            return;
        
        //Debug.Log("OnTriggerEnter " + other.gameObject.name, this);
        HealthController healthController = collider.GetComponent<HealthController>();
        if (healthController == null || healthController.enabled == false || !healthController.canBeDamaged) // El check de "can be damage" puede provocar errores, pues cuando el placeholder de la estructura se empiece a construir (es decir que se posicione en algun lado), se podrá dañar la estructura pero no se encontrará en la lista de potenciales targets
            return; 
        if (healthController.team != teamToAttack)
            return;
        if (!detectedAttackables.Contains(healthController))
        {
            detectedAttackables.Add(healthController);
            // Debug.Log($"{other.gameObject.name} added to attackables list", this);
            UpdateTarget();
        }

        if (!detectedAttackables.IsNullOrEmpty() && !running_attackCoroutine)
        {
            //Debug.Log("On Trigger Enter. AttackController enabled? " + this.enabled, this);
            StartCoroutine(AttackCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enabled)
            return;
        if (other.isTrigger)
            return;
        
        //Debug.Log("OnTriggerExit", this);
        HealthController exitElement = other.GetComponent<HealthController>();
        if (exitElement == null) return;
        if (exitElement.team != teamToAttack)
            return;
        detectedAttackables.Remove(exitElement);
        if (exitElement == target || detectedAttackables.Count <= 0)
        {
            UpdateTarget();
        }
    }

    private IEnumerator AttackCoroutine()
    {
        running_attackCoroutine = true;

        while (!detectedAttackables.IsNullOrEmpty() && this.enabled)
        {
            UpdateTarget();


            if (attackWithAnimation)
            {
                if (target == null)
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }

                if (GameManager.instance.gameData.resources < attackCost)
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }

                if (Vector3.Distance(target.transform.position, this.transform.position) > range)
                {
                    yield return new WaitForSeconds(0.75f);
                    continue;
                }

                stateController.SetNewState();
                yield return new WaitForSeconds(0.5f); // Mode delay if there is no target
            }
            else
            {
                if (PerformAttack(target))
                    yield return new WaitForSeconds(cooldown);
                else
                    yield return new WaitForSeconds(target != null? 0.5f : 1f); // Mode delay if there is no target   
            }
        }

        running_attackCoroutine = false;
    }

    public void AttackTarget()
    {
        PerformAttack(target);
    }
    
    private bool PerformAttack(HealthController target)
    {
        switch (attacksWithProjectile)
        {
            case false:
            {
                target.health -= damage;
                // Debug.Log($"ATTACKED (mele) {target.gameObject} with {damage} damage. Now {target.health} hp are still remaining.", this);
                break;
            }
            case true:
            {
                // Debug.Log($"ATTACKED (projectile) {target.gameObject} with {damage} damage. Now {target.health} hp are still remaining.", this);
                // GameObject spawnedGO = projectilePool.Spawn(projectileSpawnLocation.position, projectileSpawnLocation.rotation, Vector3.one, projectileSpawnLocation);
                GameObject spawnedGO = Instantiate(projectile, projectileSpawnLocation.position, projectileSpawnLocation.rotation);
                spawnedGO.GetComponentRequired<ProjectileMover>().SetTarget(target, damage, rng.GetRandomFloat(-maxProjectileOffset, maxProjectileOffset));
                break;
            }
        }

        GameManager.instance.gameData.resources -= attackCost;
        return true;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}