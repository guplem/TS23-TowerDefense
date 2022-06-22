using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StructureController : StateController
{
    [SerializeField] public Sprite icon;
    [SerializeField] private GameObject visuals;
    [SerializeField] public GameObject blueprint;
    [SerializeField] private GameObject construction;
    [SerializeField] private DecalProjector attackRangeDecalProjector;

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
            // BLUEPRINT
            canBeDamaged = false;
            visuals.SetActive(false);
            blueprint.SetActive(true);
            construction.SetActive(false);
            if (attackController != null) attackController.enabled = false;
            if (attackController != null)
            {
                attackRangeDecalProjector.gameObject.SetActive(true);
                attackRangeDecalProjector.size = new Vector3(attackController.range*2, attackController.range*2, 50);
            }
            else
            {
                attackRangeDecalProjector.gameObject.SetActive(false);
            }
        }
        else
        {
            // PLACED IN MAP
            canBeDamaged = true;
            blueprint.SetActive(false);
            attackRangeDecalProjector.gameObject.SetActive(false);
            if (constructionTime <= 0)
            {
                // -- Built
                visuals.SetActive(true);
                construction.SetActive(false);
                if (attackController != null) attackController.enabled = energySource != null && energySource.structure.constructionTime <= 0;
            }
            else
            {
                // -- Under Construction
                visuals.SetActive(false);
                construction.SetActive(true);
                if (attackController != null) attackController.enabled = false;
            }
        }
    }
}