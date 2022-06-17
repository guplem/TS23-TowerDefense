using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DamagerUnitController : UnitController
{
    private List<PropertyController> attackables = new List<PropertyController>();
    private PropertyController attacking;
    private IEnumerator attackCoroutine;

    protected void Start()
    {
        attackCoroutine = AttackCoroutine();
        StartCoroutine(attackCoroutine);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter " + other.gameObject.name, this);
        PropertyController property = other.GetComponent<PropertyController>();
        if (property == null) return;
        if (property.team != Team.Player)
            return;
        if (!attackables.Contains(property))
        {
            attackables.Add(property);
            gameObject.GetComponentRequired<NavMeshAgent>().isStopped = true;
            Debug.Log($"{other.gameObject.name} added to attackables list", this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTriggerExit", this);
        PropertyController property = other.GetComponent<PropertyController>();
        if (property == null) return;
        if (property.team != Team.Player)
            return;
        attackables.Remove(property);
        if (attackables.Count <= 0)
        {
            SetDestination();
        }
    }

    private void SetDestination()
    {
        gameObject.GetComponentRequired<NavMeshAgent>().destination = Vector3.zero;
        gameObject.GetComponentRequired<NavMeshAgent>().isStopped = false;
    }

    private IEnumerator AttackCoroutine()
    {
        while (true)
        {
            // Debug.Log($"Attack end of cooldown. Enemies in range = {attackables.Count}", this);
            if (attackables.Count > 0)
                PerformAttack(attackables[0]);
            yield return new WaitForSeconds(cooldown);
        }
    }

    private void PerformAttack(PropertyController attackable)
    {
        HealthController hpController;

        if (attackable != null && attackable.gameObject != null)
        {
            Debug.Log($"Going to attack {attackable.gameObject}.");
            hpController = attackable.GetComponentRequired<HealthController>();
            if (hpController == null)
            {
                Debug.LogWarning($"Trying to attack '{attackable.gameObject.name}' but it doesn't have  HealthController component");
                return;
            }

            if (hpController.health - effect <= 0)
            {
                attackables.Remove(attackable);
                SetDestination();
            }

            hpController.health -= effect;
            Debug.Log($"ATTACKED {attackable.gameObject} with {effect} effect. {hpController.health} hp remaining. (Should be... {(hpController.health - effect)}?) ");
        }
        else
        {
            attackables.Remove(attackable);
            SetDestination();
        }
    }
}