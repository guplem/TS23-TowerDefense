using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private bool isValidBuildingPlacement = true;
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
        else if (!isValidBuildingPlacement)
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
                buildingPlacement = hit.point;
                isValidBuildingPlacement = true; // TODO: Proper check with NavMesh
            }
            else
            {
                buildingPlacement = Vector3.one * 10000;
                isValidBuildingPlacement = false;
            }

            placeHolderBuilding.transform.position = buildingPlacement;
            placeHolderEnergySource = EnergySource.GetBestFor(buildingPlacement, placeHolderBuilding.GetComponent<EnergySource>());
        }
        else
        {
            buildingPlacement = Vector3.one * 10000;
            isValidBuildingPlacement = false;
        }
    }

    public void UnselectStructure()
    {
        if (placeHolderBuilding != null)
            Destroy(placeHolderBuilding.gameObject);
        placeHolderBuilding = null;
    }
}