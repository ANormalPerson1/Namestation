using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Namestation.Player
{
    public class MovementManager : PlayerComponent
    {
        [SerializeField] Transform playerModelTransform;
        InputManager inputManager;

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
            Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
            transform.position += movement * Time.deltaTime * 2.5f;
            
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

