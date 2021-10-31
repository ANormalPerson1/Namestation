using UnityEngine;

namespace Namestation.Player
{
    public class InputManager : MonoBehaviour
    {
        [HideInInspector] public bool movementInputEnabled = true;
        [HideInInspector] public bool interactionInputEnabled = true;
        [HideInInspector] public bool interactionButtonPressed = false;
        [HideInInspector] public Vector2 mousePosition;

        #region References
        CameraManager cameraManager;
        Camera playerCamera;
        #endregion

        private void Start()
        {
            cameraManager = PlayerComponents.instance.cameraManager;
            playerCamera = cameraManager.playerCamera;
        }

        public void SetInputEnabled(bool mode)
        {
            movementInputEnabled = mode;
            interactionInputEnabled = mode;
        }

        private void Update()
        {
            if (interactionInputEnabled)
            {
                interactionButtonPressed = Input.GetMouseButtonDown(0);
            }
            else
            {
                interactionButtonPressed = false;
            }

            mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
