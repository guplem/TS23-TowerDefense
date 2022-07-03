using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class ConstructionController : MonoBehaviour
{
    public static ConstructionController instance { get; private set; }

    public StructureController placeHolderBuilding
    {
        get => _placeHolderBuilding;
        private set
        {
            if (_placeHolderBuilding != value)
            {
                _placeHolderBuilding = value;
                UIManager.instance.FullRefresh();
            }
        }
    }

    private StructureController _placeHolderBuilding;
    public bool hasSelectedStructureToBuild => placeHolderBuilding != null;
    [SerializeField] private Transform structuresParent;
    public LayerMask buildingLayers;
    public LayerMask structureLayers;
    private bool isPlacementOnNavMesh = true;
    private Vector3 buildingPlacement;
    private EnergySource placeHolderEnergySource;

    [Space] [ColorUsageAttribute(true, true)] [SerializeField]
    private Color colorBlueprintOk;

    [ColorUsageAttribute(true, true)] [SerializeField]
    private Color colorBlueprintWrong;

    [Header("Sounds")]
    [SerializeField] private AudioClip selectStructureClip;
    [SerializeField] private AudioClip buildStructureClip;
    [SerializeField] private AudioClip unselectStructureClip;
    [SerializeField] private AudioClip errorBuildStructureClip;
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another ConstructionController already exists. Sestroying it");
            Destroy(instance.gameObject);
        }

        instance = this;
    }

    public void SelectStructureToBuild(GameObject structure)
    {
        UnselectStructureNoSound();

        if (structure == null)
        {
            Debug.LogWarning("Selecting null structure");
            return;
        }

        StructureController structureController = structure.gameObject.GetComponentRequired<StructureController>();
        if (structureController == null)
            return;

        placeHolderBuilding = Instantiate(structure, Vector3.one * 10000, Quaternion.Euler(new Vector3(0, Random.Range(0.0f, 360.0f), 0)), this.transform).GetComponentRequired<StructureController>();
        // placeHolderBuilding.GetComponentRequired<StructureController>().isPlaced = false; // Not necessary, default isPlaced is false.
        Debug.Log($"Selected structure '{placeHolderBuilding.ToString()}' to build.", this);
        GameManager.instance.generalAudioSource.PlayClip(selectStructureClip);
        couldBeBuilt = false;
        SetBlueprintColor(false);
    }

    private void SetBlueprintColor(bool b)
    {
        placeHolderBuilding.blueprint.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().sharedMaterial.SetColor("_Color", b ? colorBlueprintOk : colorBlueprintWrong);
    }

    public void BuildSelectedStructure()
    {
        if (!CanBeBuild())
        {
            GameManager.instance.generalAudioSource.PlayClip(errorBuildStructureClip);
            return;
        }

        MapElement instantiated = GameManager.instance.mapManager.SpawnMapElement(placeHolderBuilding.gameObject, buildingPlacement, placeHolderBuilding.transform.rotation,
            structuresParent);
        GameManager.instance.unitsSpawner.RegenerateNavMeshForUnits();
        GameManager.instance.gameData.resources -= placeHolderBuilding.cost;
        UnselectStructureNoSound();

        StructureController instantiatedStructure = instantiated.gameObject.GetComponentRequired<StructureController>();
        instantiatedStructure.isPlaced = true;
        instantiatedStructure.team = PropertyController.Team.Player;
        instantiatedStructure.energySource = placeHolderEnergySource;
        instantiatedStructure.health = 1;
        instantiatedStructure.FullyHealOverTime(instantiatedStructure.constructionTime);
        Destroy(instantiatedStructure.exclusionArea.gameObject); // To remove no longer used objects
        GameManager.instance.generalAudioSource.PlayClip(buildStructureClip);
        Debug.Log($"Built structure '{instantiated.ToString()}'.", this);
    }

    private bool CanBeBuild()
    {
        return GetReasonWhyCantBeBuilt() == ConstructionError.None;
    }

    public ConstructionError GetReasonWhyCantBeBuilt()
    {
        if (!hasSelectedStructureToBuild) return ConstructionError.NotSelected; // No element selected
        if (!isPlacementOnNavMesh) return ConstructionError.Location; // No valid location
        if (!placeHolderBuilding.exclusionArea.hasExclusionAreaFree) return ConstructionError.Distance; // Too close to other structures
        if (placeHolderEnergySource == null) return ConstructionError.Energy; // No energy source close enough
        if (placeHolderEnergySource.structure.constructionTime > 0) return ConstructionError.Energy; // Energy source not built yet
        if (GameManager.instance.gameData.resources < placeHolderBuilding.cost) return ConstructionError.Resources; // Not enough resources
        
        return ConstructionError.None; // No issues. Can be built
    }

    private void Update()
    {
        if (hasSelectedStructureToBuild)
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.instance.playerControls.Player.MousePosition.ReadValue<Vector2>());

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5000, ConstructionController.instance.buildingLayers))
            {
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1.0f, NavMesh.AllAreas))
                {
                    buildingPlacement = navMeshHit.position;
                    isPlacementOnNavMesh = true;
                }
                else
                {
                    buildingPlacement = hit.point;
                    isPlacementOnNavMesh = false;
                }
            }
            else
            {
                buildingPlacement = Vector3.one * 10000;
                isPlacementOnNavMesh = false;
            }

            placeHolderBuilding.transform.position = buildingPlacement;
            placeHolderEnergySource = EnergySource.GetBestFor(buildingPlacement, placeHolderBuilding.GetComponent<EnergySource>());
            bool canBeBuild = CanBeBuild();
            if (canBeBuild != couldBeBuilt)
            {
                SetBlueprintColor(canBeBuild);
                couldBeBuilt = canBeBuild;
            }
        }
        else
        {
            buildingPlacement = Vector3.one * 10000;
            isPlacementOnNavMesh = false;
        }
    }

    private bool couldBeBuilt = false;

    public void UnselectStructure()
    {
        GameManager.instance.generalAudioSource.PlayClip(unselectStructureClip);
        UnselectStructureNoSound();
    }
    
    public void UnselectStructureNoSound()
    {
        if (placeHolderBuilding != null)
            Destroy(placeHolderBuilding.gameObject);
        placeHolderBuilding = null;
    }
}

public enum ConstructionError
{
    None,
    NotSelected,
    Distance,
    Resources,
    Energy,
    Other,
    Location
}