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
            // Debug.Log("Unit state: to center");
            SetDestination(Vector3.zero); // TODO: Change for the precise location of the main structure
        }
        else if (Vector3.Distance(attackController.target.transform.position, this.transform.position) <= attackController.range)
        {
            // Debug.Log("Unit state: stop");
            StopMovement();
        }
        else
        {
            // Debug.Log("Unit state: to target on the way");
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

