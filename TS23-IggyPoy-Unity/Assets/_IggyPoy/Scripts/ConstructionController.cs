using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

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
    private bool isPlacementOnNavMesh = true;
    private Vector3 buildingPlacement;
    private EnergySource placeHolderEnergySource;

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
        if (structure == null)
        {
            Debug.LogWarning("Selecting null structure");
            return;
        }

        StructureController structureController = structure.gameObject.GetComponentRequired<StructureController>();
        if (structureController == null)
            return;

        placeHolderBuilding = Instantiate(structure, Vector3.one * 10000, Quaternion.identity, this.transform).GetComponentRequired<StructureController>();
        // placeHolderBuilding.GetComponentRequired<StructureController>().isPlaced = false; // Not necessary, default isPlaced is false.
        Debug.Log($"Selected structure '{placeHolderBuilding.ToString()}' to build.", this);
    }

    public void BuildSelectedStructure()
    {
        if (!hasSelectedStructureToBuild)
        {
            Debug.Log($"Structure could not be build. No structure is selected");
            return;
        }
        else if (!isPlacementOnNavMesh)
        {
            Debug.Log($"Structure could not be build. Invalid placement");
            return;
        }
        else if (GameManager.instance.gameData.resources < placeHolderBuilding.cost)
        {
            Debug.Log($"Structure could not be build. Could not afford the cost ({placeHolderBuilding.cost} needed, player has {GameManager.instance.gameData.resources})");
            return;
        }
        else if (placeHolderEnergySource == null)
        {
            Debug.Log($"Structure could not be build. Energy source not in range");
            return;
        } else if (!placeHolderBuilding.exclusionArea.hasExclusionAreaFree)
        {
            Debug.Log($"Structure could not be build. Other structures are too close.");
            placeHolderBuilding.exclusionArea.structuresInExclusionArea.DebugLog(", ", "Too close structures: ");
            return;
        }

        MapElement instantiated = GameManager.instance.mapManager.SpawnMapElement(placeHolderBuilding.gameObject, buildingPlacement, Quaternion.identity,
            structuresParent);
        GameManager.instance.unitsSpawner.RegenerateNavMeshForUnits();
        GameManager.instance.gameData.resources -= placeHolderBuilding.cost;
        UnselectStructure();

        StructureController instantiatedStructure = instantiated.gameObject.GetComponentRequired<StructureController>();
        instantiatedStructure.isPlaced = true;
        instantiatedStructure.team = PropertyController.Team.Player;
        instantiatedStructure.energySource = placeHolderEnergySource;
        Destroy(instantiatedStructure.exclusionArea.gameObject); // To remove no longer used objects
        Debug.Log($"Built structure '{instantiated.ToString()}'.", this);
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
        }
        else
        {
            buildingPlacement = Vector3.one * 10000;
            isPlacementOnNavMesh = false;
        }
    }

    public void UnselectStructure()
    {
        if (placeHolderBuilding != null)
            Destroy(placeHolderBuilding.gameObject);
        placeHolderBuilding = null;
    }
}