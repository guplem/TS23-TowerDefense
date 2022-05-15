using System;
using UnityEngine;

public class StructureController : HealthController
{
    [SerializeField] private GameObject visuals;
    [SerializeField] private GameObject blueprint;
    [Tooltip("Effect (attack, healing, ...) range of the structure")]
    [SerializeField] private float range = 10;
    [Tooltip("Minimum allowed distance to the closes building of the same type")]
    [SerializeField] private float exclusionArea = 2;
    [Tooltip("The cost to build the structure")]
    [SerializeField] private float cost = 20;

    public void ShowBlueprint()
    {
        visuals.SetActive(false);
        blueprint.SetActive(true);
    }
    
    public void ShowVisuals()
    {
        visuals.SetActive(true);
        blueprint.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, exclusionArea);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
