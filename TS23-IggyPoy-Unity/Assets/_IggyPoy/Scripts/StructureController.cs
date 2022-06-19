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
    public int cost = 20;

    [Tooltip("The time it takes to build the structure")] [SerializeField]
    private float constructionTime = 3;

    public bool isPlaced
    {
        get => _isPlaced;
        set
        {
            if (value == _isPlaced) return;
            if (_isPlaced == false && value == true)
                StartCoroutine(ConstructionCoroutine());
            _isPlaced = value;
            SetNewState();
        }
    }

    private bool _isPlaced = false;

    [SerializeField] private AttackController attackController;

    public EnergySource energySource
    {
        get => _energySource;
        set
        {
            if (energySource == value && value != null)
                return;

            if (_energySource != null) 
                _energySource.Detach(this);

            _energySource = value == null ? EnergySource.GetBestFor(transform.position, GetComponent<EnergySource>()) : value;
            
            if (_energySource != null) 
                _energySource.Attatch(this);
            
            SetNewState();
        }
    }

    public EnergySource _energySource;

    private void Awake()
    {
        SetNewState();
        if (!isPlaced || constructionTime > 0)
        {
            attackController.enabled = false;
        }
    }

    private IEnumerator ConstructionCoroutine()
    {
        float steps = 0.25f; // Every X seconds will be called

        while (constructionTime > 0)
        {
            constructionTime -= steps;
            yield return new WaitForSeconds(steps);
        }

        SetNewState();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, exclusionArea);
    }

    public override void SetNewState()
    {
        if (!isPlaced)
        {
            canBeDamaged = false;
            visuals.SetActive(false);
            blueprint.SetActive(true);
            construction.SetActive(false);
            attackController.enabled = false;
        }
        else
        {
            canBeDamaged = true;
            blueprint.SetActive(false);
            if (constructionTime <= 0)
            {
                visuals.SetActive(true);
                construction.SetActive(false);
                attackController.enabled = energySource != null;
            }
            else
            {
                visuals.SetActive(false);
                construction.SetActive(true);
                attackController.enabled = false;
            }
        }
    }
}