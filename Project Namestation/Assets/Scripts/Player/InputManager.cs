using UnityEngine;

namespace Namestation.Player
{
    public class InputManager : PlayerComponent
    {
        #region Variables
        [HideInInspector] public bool movementInputEnabled = true;
        [HideInInspector] public bool interactionInputEnabled = true;
        [HideInInspector] public bool interactionButtonPressed = false;
        [HideInInspector] public Vector2 mousePosition;
        #endregion

        #region References
        CameraManager cameraManager;
        Camera playerCamera;
        #endregion

        public override void Initialize()
        {
            base.Initialize();
            cameraManager = playerManager.cameraManager;
            playerCamera = cameraManager.playerCamera;
        }

        private void SetInputEnabled(bool mode)
        {
            movementInputEnabled = mode;
            interactionInputEnabled = mode;
        }

        private void Update()
        {
            if (!initialized || !isLocalPlayer) return;

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
