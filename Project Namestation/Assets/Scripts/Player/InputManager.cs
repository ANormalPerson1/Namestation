using UnityEngine;

namespace Namestation.Player
{
    public class InputManager : PlayerComponent
    {
        #region Variables
        [HideInInspector] public bool movementInputEnabled = true;
        [HideInInspector] public bool interactionInputEnabled = true;
        [HideInInspector] public bool interactionButtonPressed = false;
        [HideInInspector] public Vector2 globalMousePosition, localMousePosition;
        [HideInInspector] public Vector2 movementInput;

        private Vector2 screenWorldScale;
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
            DetermineScreenScale();
        }

        private void DetermineScreenScale()
        {
            Vector2 bottomLeftPoint = playerCamera.ViewportToWorldPoint(Vector2.zero);
            Vector2 topRightPoint = playerCamera.ViewportToWorldPoint(Vector2.one);
            screenWorldScale = topRightPoint - bottomLeftPoint;
        }

        private void SetInputEnabled(bool mode)
        {
            movementInputEnabled = mode;
            interactionInputEnabled = mode;
        }

        private void Update()
        {
            if (!initialized || !isLocalPlayer) return;
            ParseInput();
        }

        private void ParseInput()
        {
            ParseMovementInput();
            ParseInteractionInput();
            ParseMousePosition();
        }

        private void ParseMovementInput()
        {
            if(movementInputEnabled)
            {
                movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            }
            else
            {
                movementInput = Vector2.zero;
            }
        }

        private void ParseInteractionInput()
        {
            if (interactionInputEnabled)
            {
                interactionButtonPressed = Input.GetMouseButtonDown(0);
            }
            else
            {
                interactionButtonPressed = false;
            }
        }

        private void ParseMousePosition()
        {
            globalMousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector2 viewportMousePosition = playerCamera.ScreenToViewportPoint(Input.mousePosition);
            Vector2 viewportCameraPosition = new Vector2(0.5f, 0.5f);
            Vector2 viewportMouseLocalDirection = (viewportMousePosition - viewportCameraPosition);
            localMousePosition = new Vector2(viewportMouseLocalDirection.x * screenWorldScale.x, viewportMouseLocalDirection.y * screenWorldScale.y);
        }
    }
}
