using System;
using System.Collections;
using UnityEngine;

public class StructureController : StateController
{
    [SerializeField] private GameObject visuals;
    [SerializeField] private GameObject blueprint;
    [SerializeField] private GameObject construction;

    [Tooltip("Minimum allowed distance to the closes building")] [SerializeField]
    private float exclusionArea = 2;

    [Tooltip("The cost to build the structure")] [SerializeField]
    private float cost = 20;

    [Tooltip("The time it takes to build the structure")] [SerializeField]
    private float constructionTime = 3;

    [SerializeField] private AttackController attackController;

    private void Awake()
    {
        if (constructionTime > 0)
        {
            attackController.enabled = true;
            StartCoroutine(ConstructionCoroutine());
        }
    }
    
    private IEnumerator ConstructionCoroutine()
    {
        float steps = 0.1f; // Every X seconds will be called

        while (constructionTime > 0)
        {
            constructionTime -= steps;
            yield return new WaitForSeconds(steps);
        }

    }


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
    }

    public override void SetNewState()
    {
        if (constructionTime < 0)
        {
            visuals.SetActive(true);
            construction.SetActive(false);
            attackController.enabled = true;
        }
    }
}