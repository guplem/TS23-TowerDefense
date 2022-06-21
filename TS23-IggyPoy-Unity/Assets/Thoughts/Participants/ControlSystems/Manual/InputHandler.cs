using Thoughts.Game.Map.MapElements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Thoughts.Participants.ControlSystems.Manual
{
    /// <summary>
    /// Handles the input for a Manual ControlSystem
    /// </summary>
    [RequireComponent(typeof(Manual))]
    public class InputHandler : MonoBehaviour
    {
        /// <summary>
        /// Reference to the Manual ControlSystem using the input handled by this InputHandler
        /// </summary>
        private Manual manualControlSystem;
        
        /// <summary>
        /// Layers that the Map Elements use
        /// </summary>
        [Tooltip("Layers that the Map Elements use")]
        [SerializeField] private LayerMask mapElementLayers;
        
        /// <summary>
        /// Initial setup
        /// </summary>
        private void Awake()
        {
            manualControlSystem = this.GetComponentRequired<Manual>();
        }

        //[SerializeField] private KeyCode forward, backward, left, right, up, down;

        private void Update()
        {
            HandleCameraInput();
        }

        /// <summary>
        /// Checks if the mouse is over a UI element
        /// </summary>
        /// <returns>True if the mouse is over a UI element, otherwise, false .</returns>
        private bool IsMouseOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        /// <summary>
        /// The start rotation of the mouse.
        /// </summary>
        private Vector3 mouseStartRotation;
        
        /// <summary>
        /// The current rotation of the mouse.
        /// </summary>
        private Vector3 mouseCurrentRotation;
        
        /// <summary>
        /// Handles the input to control the camera
        /// </summary>
        private void HandleCameraInput()
        {
            var input = InputManager.instance.playerControls.Player;
            bool isFastSpeed = input.FastMove.ReadValue<float>() > 0.5f;//Input.GetButton("Shift");
            // Movement of the camera
            Vector3 direction = input.Move.ReadValue<Vector2>().ToVector3NewY(0f);//new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
            manualControlSystem.cameraController.Move(direction, isFastSpeed);

            
            // Rotation
            manualControlSystem.cameraController.Rotate(new Vector2((input.RotateL.ReadValue<float>() - input.RotateR.ReadValue<float>())/2, input.Zoom.ReadValue<float>()/2*-100), isFastSpeed);
        }
    }
}
