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
    private int effect = 1;

    [Tooltip("Attack range")] [SerializeField]
    public int range = 1;

    private List<HealthController> attackables = new List<HealthController>();
    
    private bool running_attackCoroutine = false;

    [SerializeField] private StateController stateController;

    public HealthController target { get; private set; }

    private void UpdateTarget()
    {
        if (attackables.Contains(target) && target.health > 0)
            return;
        foreach (HealthController targetCandidate in attackables)
        {
            if (targetCandidate.health <= 0)
            {
                attackables.Remove(targetCandidate);
                continue;
            }

            target = targetCandidate;
        }

        Debug.LogWarning($"No targets found. attackables list length = {attackables.Count}");
        target = null;
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter " + other.gameObject.name, this);
        HealthController property = other.GetComponent<HealthController>();
        if (property == null) return;
        if (property.team != PropertyController.Team.Player)
            return;
        if (!attackables.Contains(property))
        {
            attackables.Add(property);
            Debug.Log($"{other.gameObject.name} added to attackables list", this);
            UpdateTarget();
        }

        if (!attackables.IsNullOrEmpty() && !running_attackCoroutine)
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
        attackables.Remove(exitElement);
        if (exitElement == target || attackables.Count <= 0)
        {
            UpdateTarget();
            stateController.SetNewState();
        }
    }

    private IEnumerator AttackCoroutine()
    {
        running_attackCoroutine = true;

        while (!attackables.IsNullOrEmpty())
        {
            UpdateTarget();
            // Debug.Log($"Attack end of cooldown. Enemies in range = {attackables.Count}", this);
            if (PerformAttack(target))
            {
                yield return new WaitForSeconds(cooldown);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        running_attackCoroutine = false;
    }

    private bool PerformAttack(HealthController target)
    {
        if (Vector3.Distance(target.transform.position, this.transform.position) > range)
        {
            return false;
        }

        target.health -= effect;
        Debug.Log($"ATTACKED {target.gameObject} with {effect} effect. {target.health} hp remaining. (Should be... {(target.health - effect)}?) ");
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}