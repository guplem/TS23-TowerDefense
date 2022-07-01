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

    private StructureController placeHolderBuilding
    {
        get => _placeHolderBuilding;
        set
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
    [Space]
    [ColorUsageAttribute(true,true)]
    [SerializeField] private Color colorBlueprintOk;
    [ColorUsageAttribute(true,true)]
    [SerializeField] private Color colorBlueprintWrong;

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
        UnselectStructure();

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
            return;

        MapElement instantiated = GameManager.instance.mapManager.SpawnMapElement(placeHolderBuilding.gameObject, buildingPlacement, placeHolderBuilding.transform.rotation,
            structuresParent);
        GameManager.instance.unitsSpawner.RegenerateNavMeshForUnits();
        GameManager.instance.gameData.resources -= placeHolderBuilding.cost;
        UnselectStructure();

        StructureController instantiatedStructure = instantiated.gameObject.GetComponentRequired<StructureController>();
        instantiatedStructure.isPlaced = true;
        instantiatedStructure.team = PropertyController.Team.Player;
        instantiatedStructure.energySource = placeHolderEnergySource;
        instantiatedStructure.health = 1;
        instantiatedStructure.FullyHealOverTime(instantiatedStructure.constructionTime);
        Destroy(instantiatedStructure.exclusionArea.gameObject); // To remove no longer used objects
        Debug.Log($"Built structure '{instantiated.ToString()}'.", this);
    }

    private bool CanBeBuild()
    {
        return GetReasonWhyCantBeBuilt() == ConstructionError.None;
    }

    public ConstructionError GetReasonWhyCantBeBuilt()
    {
        if (!hasSelectedStructureToBuild)
        {
            // Debug.Log($"Structure could not be build. No structure is selected");
            return ConstructionError.NotSelected;
        }
        else if (!isPlacementOnNavMesh)
        {
            // Debug.Log($"Structure could not be build. Invalid placement");
            return ConstructionError.Location;
        }
        else if (GameManager.instance.gameData.resources < placeHolderBuilding.cost)
        {
            // Debug.Log($"Structure could not be build. Could not afford the cost ({placeHolderBuilding.cost} needed, player has {GameManager.instance.gameData.resources})");
            return ConstructionError.Resources;
        }
        else if (placeHolderEnergySource == null)
        {
            // Debug.Log($"Structure could not be build. Energy source not in range");
            return ConstructionError.Energy;
        } else if (placeHolderEnergySource.structure.constructionTime > 0)
        {
            // Debug.Log($"Structure could not be build. Energy source not built yet");
            return ConstructionError.Energy;
        } else if (!placeHolderBuilding.exclusionArea.hasExclusionAreaFree)
        {
            // Debug.Log($"Structure could not be build. Other structures are too close.");
            placeHolderBuilding.exclusionArea.structuresInExclusionArea.DebugLog(", ", "Too close structures: ");
            return ConstructionError.Distance;
        }

        return ConstructionError.None;
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