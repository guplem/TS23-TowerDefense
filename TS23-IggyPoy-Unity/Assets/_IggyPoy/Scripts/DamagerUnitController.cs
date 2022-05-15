using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DamagerUnitController : UnitController
{
    private List<PropertyController> attackables;
    private PropertyController attacking;

    protected void Start()
    {
        StartCoroutine(nameof(Attack));
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter " + other.gameObject.name, this);
        PropertyController property = other.GetComponent<PropertyController>();
        if (property == null) return;
        if (property.team != Team.Player) 
            return;
        if (!attackables.Contains(property))
        {
            attackables.Add(property);
            gameObject.GetComponentRequired<NavMeshAgent>().isStopped = true;
        }

    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerEnter", this);
        PropertyController property = other.GetComponent<PropertyController>();
        if (property == null) return;
        if (property.team != Team.Player) 
            return;
        attackables.Remove(property);
        if (attackables.Count <= 0)
        {
            gameObject.GetComponentRequired<NavMeshAgent>().destination = Vector3.zero;
            gameObject.GetComponentRequired<NavMeshAgent>().isStopped = false;
        }
    }
    
    private IEnumerable<WaitForSeconds> Attack()
    {
        while (true)
        {
            Debug.Log($"Attack end of cooldown. Enemies in range = {attackables.Count}", this);
            if (attackables.Count > 0) 
                PerformAttack(attackables[0]);
            yield return new WaitForSeconds(cooldown);
        }

    }

    private void PerformAttack(PropertyController attackable)
    {
        Debug.Log($"Going to attack {attackable.gameObject}.");
        HealthController hpController = attackable.GetComponent<HealthController>();
        if (hpController == null)
        {
            Debug.LogWarning($"Trying to attack '{attackable.gameObject.name}' but it doesn't have  HealthController component");
            return;
        }

        if (hpController.health - effect <= 0)
            attackables.Remove(attackable);

        hpController.health -= effect;
        Debug.Log($"ATTACKED {attackable.gameObject}. {hpController.health} hp remaining");
    }
    
}
