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
        [SerializeField] LayerMask constructionLayermask;
        InputManager inputManager;
        Transform currentGrid;
        Rigidbody2D currentGridRigidbody2D;

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
            CheckOverlap();
        }

        void CheckOverlap()
        {
            Collider2D collider = Physics2D.OverlapCircle(playerRigidbody.position, 1f, constructionLayermask);

            if (collider == null)
            {
                currentGrid = null;
                currentGridRigidbody2D = null;
            }
            else
            {
                currentGrid = collider.transform.parent.parent;
            }

            if(transform.parent != currentGrid)
            {
                SetParentServer(currentGrid);
            }
        }

        [Command]
        private void SetParentServer(Transform parent)
        {
            transform.parent = parent;
            SetParentClient(parent);
        }

        [ClientRpc]
        private void SetParentClient(Transform parent)
        {
            transform.parent = parent;
            if(parent != null)
            {
                currentGridRigidbody2D = parent.GetComponent<Rigidbody2D>();
            }
            else
            {
                currentGridRigidbody2D = null;
            }
        }

        void HandleMovement()
        {
            Vector2 localVelocity = inputManager.movementInput * movementSpeed;
            playerRigidbody.velocity = localVelocity;

            if (currentGridRigidbody2D != null)
            {
                playerRigidbody.velocity += currentGridRigidbody2D.velocity;
            }
        }

        void HandleRotation()
        {
            Vector3 localMousePosition = inputManager.localMousePosition;
            playerModelTransform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(localMousePosition.y, localMousePosition.x) * Mathf.Rad2Deg);
        }
    }
}

