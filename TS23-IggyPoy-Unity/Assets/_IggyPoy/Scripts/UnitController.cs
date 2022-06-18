using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : StateController
{
    
    [SerializeField] private AttackController attackController;
    
    public override void SetNewState()
    {
        if (attackController.target == null)
        {
            SetDestination(Vector3.zero); // TODO: Change for the precise location of the main structure
        }
        else if (Vector3.Distance(attackController.target.transform.position, this.transform.position) <= attackController.range)
        {
            StopMovement();
        }
        else
        {
            SetDestination(attackController.target.transform.position);
        }
    }
    
    private void SetDestination(Vector3 destination)
    {
        gameObject.GetComponentRequired<NavMeshAgent>().destination = destination;
        gameObject.GetComponentRequired<NavMeshAgent>().isStopped = false; 
    }
    
    private void StopMovement()
    {
        gameObject.GetComponentRequired<NavMeshAgent>().isStopped = true; 
    }
    
}

