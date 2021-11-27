using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Namestation.Player
{
    public class MovementManager : PlayerComponent
    {
        [SerializeField] float movementSpeed;
        [SerializeField] Transform playerModelTransform;
        [SerializeField] Rigidbody2D playerRigidbody;
        InputManager inputManager;
        Transform currentGrid;

        public override void Initialize()
        {
            base.Initialize();
            inputManager = playerManager.inputManager;
        }

        private void Update()
        {
            if (!initialized || !isLocalPlayer) return;
            HandleMovement();
            HandleRotation();
        }

        void HandleMovement()
        {
            Debug.Log(inputManager.movementInput);
            Vector2 localVelocity = inputManager.movementInput * movementSpeed;
            playerRigidbody.velocity = localVelocity;
        }

        void HandleRotation()
        {
            Vector3 mousePosition = inputManager.mousePosition;
            float deltaY = mousePosition.y - playerModelTransform.position.y;
            float deltaX = mousePosition.x - playerModelTransform.position.x;
            playerModelTransform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg);
        }
    }
}

