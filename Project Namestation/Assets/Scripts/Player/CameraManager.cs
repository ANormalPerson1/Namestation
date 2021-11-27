using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Namestation.Player
{
    public class CameraManager : PlayerComponent
    {
        public Camera playerCamera;
        [SerializeField] float cameraFollowIntensity;
        [SerializeField] float maximumCameraFollowLength;

        InputManager inputManager;

        protected override void InitializeNotLocalPlayer()
        {
            base.InitializeNotLocalPlayer();
            Destroy(playerCamera.gameObject);
        }

        public override void Initialize()
        {
            base.Initialize();
            inputManager = playerManager.inputManager;
        }

        private void Update()
        {
            if (!initialized || !isLocalPlayer) return;
            PositionCamera();
        }

        private void PositionCamera()
        {
            Vector2 localMousePosition = inputManager.localMousePosition;

            if (localMousePosition.magnitude > maximumCameraFollowLength)
            {
                localMousePosition = localMousePosition.normalized * maximumCameraFollowLength;
            }

            Vector3 middle = localMousePosition / 2f;
            playerCamera.transform.localPosition = new Vector3(middle.x, middle.y, playerCamera.transform.position.z);
        }
    }
}
