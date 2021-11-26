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
            Vector3 mousePosition = inputManager.mousePosition;
            mousePosition.z = playerCamera.transform.position.z;
            Vector3 direction = (mousePosition - playerCamera.transform.position) * cameraFollowIntensity;

            if (direction.magnitude > maximumCameraFollowLength)
            {
                direction = direction.normalized * maximumCameraFollowLength;
            }

            Vector3 endPosition = direction + playerCamera.transform.position;

            Vector3 middle = (endPosition + transform.position) / 2f;
            playerCamera.transform.position = new Vector3(middle.x, middle.y, playerCamera.transform.position.z);
        }
    }
}
