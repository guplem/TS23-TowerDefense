using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackController : MonoBehaviour
{
    [Tooltip("In seconds")] [SerializeField]
    private float cooldown = 1;

    [Tooltip("Damage (or healing if negative)")] [SerializeField]
    private int damage = 1;

    [Tooltip("Attack range")] [SerializeField]
    public int range = 1;

    private List<HealthController> detectedAttackables = new();

    private bool running_attackCoroutine = false;

    [SerializeField] private StateController stateController;

    public HealthController target { get; private set; }

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

            if (target != targetCandidate)
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
            Debug.LogWarning($"No targets found. attackables list length = {detectedAttackables.Count}");
            if (target != null)
            {
                target = null;
                stateController.SetNewState();
            }
        }

        return;
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter " + other.gameObject.name, this);
        HealthController property = other.GetComponent<HealthController>();
        if (property == null) return;
        if (property.team != PropertyController.Team.Player)
            return;
        if (!detectedAttackables.Contains(property))
        {
            detectedAttackables.Add(property);
            Debug.Log($"{other.gameObject.name} added to attackables list", this);
            UpdateTarget();
        }

        if (!detectedAttackables.IsNullOrEmpty() && !running_attackCoroutine)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTriggerExit", this);
        HealthController exitElement = other.GetComponent<HealthController>();
        if (exitElement == null) return;
        if (exitElement.team != PropertyController.Team.Player)
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

        while (!detectedAttackables.IsNullOrEmpty())
        {
            UpdateTarget();
            // Debug.Log($"Attack end of cooldown. Enemies in range = {attackables.Count}", this);
            if (PerformAttack(target))
            {
                yield return new WaitForSeconds(cooldown);
            }
            else
            {
                yield return new WaitForSeconds(target != null? 0.5f : 1f); // Mode delay if there is no target
            }
        }

        running_attackCoroutine = false;
    }

    private bool PerformAttack(HealthController target)
    {
        if (target == null)
            return false;
        
        if (Vector3.Distance(target.transform.position, this.transform.position) > range)
        {
            return false;
        }

        target.health -= damage;
        Debug.Log($"ATTACKED {target.gameObject} with {damage} damage. Now {target.health} hp are still remaining.");
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}