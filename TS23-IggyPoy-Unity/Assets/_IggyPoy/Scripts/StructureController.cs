using System;
using System.Collections;
using UnityEngine;

public class StructureController : StateController
{
    [SerializeField] private GameObject visuals;
    [SerializeField] private GameObject blueprint;
    [SerializeField] private GameObject construction;

    [Tooltip("Minimum allowed distance to the closes building")] [SerializeField]
    public ExclusionArea exclusionArea;

    [Tooltip("The cost to build the structure")] [SerializeField]
    public int cost = 20;

    [Tooltip("The time it takes to build the structure")] [SerializeField]
    public float constructionTime = 3;

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

    [NonSerialized] public EnergySource _energySource;

    protected new void Awake()
    {
        base.Awake();
        SetNewState();
        if (!isPlaced || constructionTime > 0)
        {
            if (attackController != null) attackController.enabled = false;
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

    public override void SetNewState()
    {
        if (!isPlaced)
        {
            canBeDamaged = false;
            visuals.SetActive(false);
            blueprint.SetActive(true);
            construction.SetActive(false);
            if (attackController != null) attackController.enabled = false;
        }
        else
        {
            canBeDamaged = true;
            blueprint.SetActive(false);
            if (constructionTime <= 0)
            {
                visuals.SetActive(true);
                construction.SetActive(false);
                if (attackController != null) attackController.enabled = energySource != null;
            }
            else
            {
                visuals.SetActive(false);
                construction.SetActive(true);
                if (attackController != null) attackController.enabled = false;
            }
        }
    }
}