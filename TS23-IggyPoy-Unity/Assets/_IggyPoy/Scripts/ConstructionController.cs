using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConstructionController : MonoBehaviour
{
    public static ConstructionController instance { get; private set; }
    private StructureController selectedStructureToConstruct
    {
        get => _selectedStructureToConstruct;
        set
        {
            if (_selectedStructureToConstruct != value)
            {
                _selectedStructureToConstruct = value;
                UIManager.instance.FullRefresh();
            }
        }
    }
    private StructureController _selectedStructureToConstruct;
    public bool hasSelectedStructureToBuild => selectedStructureToConstruct != null;
    [SerializeField] private Transform structuresParent;
    private GameObject placeHolderBuilding;
    public LayerMask buildingLayers ;
    private bool isValidBuildingPlacement = true; // TODO: Update
    private Vector3 buildingPlacement;

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

        selectedStructureToConstruct = structure.gameObject.GetComponentRequired<StructureController>();
        if (selectedStructureToConstruct == null)
            return;

        placeHolderBuilding = Instantiate(selectedStructureToConstruct.gameObject, Vector3.one*10000, Quaternion.identity, this.transform);
        placeHolderBuilding.GetComponentRequired<StructureController>().ShowBlueprint();
        Debug.LogWarning($"Selected structure '{structure.ToString()}' to build.", this);
    }

    public void BuildSelectedStructure()
    {
        if (!hasSelectedStructureToBuild || !isValidBuildingPlacement)
            return;

        MapElement instantiated = GameManager.instance.mapManager.SpawnMapElement(selectedStructureToConstruct.gameObject, buildingPlacement, Quaternion.identity,
            structuresParent);
        UnselectStructure();
        instantiated.gameObject.GetComponentRequired<StructureController>().ShowVisuals();
        Debug.Log($"Built structure '{instantiated.ToString()}'.", this);
    }

    private void Update()
    {
        if (placeHolderBuilding != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.instance.playerControls.Player.MousePosition.ReadValue<Vector2>());

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5000, ConstructionController.instance.buildingLayers))
            {
                buildingPlacement = hit.point;
                isValidBuildingPlacement = true; // TODO: Proper check
            }
            else
            {
                buildingPlacement = Vector3.one*10000;
                isValidBuildingPlacement = false;
            }

            placeHolderBuilding.transform.position = buildingPlacement;
        }
        else
        {
            buildingPlacement = Vector3.one*10000;
            isValidBuildingPlacement = false;
        }
    }

    public void UnselectStructure()
    {
        if (placeHolderBuilding != null)
            Destroy(placeHolderBuilding);
        placeHolderBuilding = null;
        selectedStructureToConstruct = null;
    }
}