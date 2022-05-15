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
            if (attackables.Count > 0) 
                PerformAttack(attackables[0]);
            yield return new WaitForSeconds(cooldown);
        }

    }

    private void PerformAttack(PropertyController attackable)
    {
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
