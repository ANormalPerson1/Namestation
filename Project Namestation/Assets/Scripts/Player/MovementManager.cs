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
            TEMP_HandleGridMovement();
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
                transform.parent = currentGrid;
                currentGridRigidbody2D = currentGrid.GetComponent<Rigidbody2D>();
            }
        }

        void TEMP_HandleGridMovement()
        {
            Debug.Log("wat..");
            if(currentGrid != null)
            {
                Debug.Log(Input.GetKey(KeyCode.Space) + " aaaa");
                Vector2 forceDirection = Input.GetKey(KeyCode.Space) ? Vector2.left * 5f * Time.deltaTime : Vector2.zero;
                currentGridRigidbody2D.AddForce(forceDirection);
            }
        }

        void HandleMovement()
        {
            Vector2 localVelocity = inputManager.movementInput * movementSpeed;
            playerRigidbody.velocity = localVelocity;

            if (currentGrid != null)
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

