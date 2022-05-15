using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerControls playerControls { get; private set; }
    public static InputManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Another ConstructionController already exists. Sestroying it");
            Destroy(instance.gameObject);
        }

        instance = this;
        playerControls = new PlayerControls();
    }


    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        playerControls.Player.Click.performed += Click;
        playerControls.Player.Cancel.performed += Back;
    }

    private void Back(InputAction.CallbackContext context)
    {
        if (ConstructionController.instance.hasSelectedStructureToBuild)
        {
            ConstructionController.instance.UnselectStructure();
        }
    }

    private void Click(InputAction.CallbackContext context)
    {
        // context.ReadValue<float>(); // To read values
        //context.ReadValueAsButton();
        if (ConstructionController.instance.hasSelectedStructureToBuild)
        {
            ConstructionController.instance.BuildSelectedStructure();
        }
    }
}
