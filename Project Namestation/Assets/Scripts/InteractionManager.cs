using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

namespace Namestation.Player
{
    public class InteractionManager : PlayerComponent
    {
        [SerializeField] float collisionDetectionRadius;
        [SerializeField] GameObject emptyGameObject;
        [SerializeField] GameObject floorPrefab;
        [SerializeField] LayerMask floorLayerMask;
        [SerializeField] LayerMask wallLayerMask;
        [SerializeField] LayerMask entityLayerMask;

        PlayerManager playerComponents;
        InputManager inputManager;

        public override void Initialize()
        {
            base.Initialize();
            playerComponents = PlayerManager.instance;
            inputManager = playerComponents.inputManager;
        }

        protected override void Update()
        {
            base.Update();
            CheckPlaceObject();
        }

        private void CheckPlaceObject()
        {
            if (inputManager.interactionButtonPressed)
            {
                AttemptToPlaceObject();
            }
        }

        private void AttemptToPlaceObject()
        {
            Vector2 mousePosition = inputManager.mousePosition;
            Collider2D[] colliders = GetCollidersNearPlacementPoint(mousePosition);
            if(colliders.Length == 0)
            {
                PlaceObjectAsNewStructure(mousePosition);
            }
            else
            {
                Collider2D nearestCollider = colliders[0];
                PlaceObjectAddToExistingStructure(mousePosition, nearestCollider);
            }
        }

        private Collider2D[] GetCollidersNearPlacementPoint(Vector2 mousePosition)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePosition, collisionDetectionRadius, floorLayerMask);
            if (colliders.Length == 0) return colliders;

            colliders = colliders.OrderBy(col => Vector2.Distance(mousePosition, col.ClosestPoint(mousePosition))).ToArray();

            return colliders;
        }

        private void PlaceObjectAsNewStructure(Vector2 mousePosition)
        {
            PlaceObjectServer(floorPrefab, mousePosition, Quaternion.identity, null);
        }

        private void PlaceObjectAddToExistingStructure(Vector2 mousePosition, Collider2D existingStructure)
        {
            Transform structureTransform = existingStructure.transform;
            Vector2? placementPosition = ConvertRawToPlacementPosition(mousePosition, structureTransform);
            if (placementPosition == null) return;

            if(GridClear(placementPosition.Value, structureTransform.rotation))
            {
                PlaceObjectServer(floorPrefab, placementPosition.Value, structureTransform.rotation, structureTransform.parent);
            }
        }

        [Command]
        private void PlaceObjectServer(GameObject newObject, Vector2 position, Quaternion rotation, Transform parent)
        {
            PlaceObjectClient(newObject, position, rotation, parent);
        }

        [ClientRpc]
        private void PlaceObjectClient(GameObject newObject, Vector2 position, Quaternion rotation, Transform parent)
        {
            if(parent == null)
            {
                parent = Instantiate(emptyGameObject, position, rotation).transform;
                parent.name = "New Object";
            } 

            GameObject newObjectInstance = Instantiate(newObject, position, rotation, parent);
            newObjectInstance.name = "New Subobject";
        }

        private Vector2? ConvertRawToPlacementPosition(Vector2 mousePosition, Transform structureTransform)
        {
            //Convert the position to local space, check where to place relative to the object's local grid and reconvert into world space.
            Vector2 baseLocalPosition = structureTransform.InverseTransformPoint(mousePosition);
            Vector2? localPlacementPosition = CalculateObjectLocalPlacementPosition(baseLocalPosition.normalized);
            if(localPlacementPosition != null)
            {
                Vector2 placementPosition = structureTransform.TransformPoint(localPlacementPosition.Value);
                return placementPosition;
            }
            return null;
        }

        private Vector2? CalculateObjectLocalPlacementPosition(Vector2 baseLocalPosition)
        {
            //If under 45 degree angle, do not place.
            if(Mathf.Abs(baseLocalPosition.x) >= 0.5f && Mathf.Abs(baseLocalPosition.y) >= 0.5f)
            {
                return null;
            }

            if (Mathf.Abs(baseLocalPosition.x) >= Mathf.Abs(baseLocalPosition.y))
            {
                if (baseLocalPosition.x >= 0f)
                {
                    return Vector2.right;
                }
                else
                {
                    return Vector2.left;
                }
            }
            else
            {
                if (baseLocalPosition.y >= 0f)
                {
                    return Vector2.up;
                }
                else
                {
                    return Vector2.down;
                }
            }
        }

        private bool GridClear(Vector2 position, Quaternion rotation)
        {
            Vector3 quaternionEuler = rotation.eulerAngles;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(position, Vector2.one * 0.95f, quaternionEuler.z);
            Debug.Log(colliders.Length);
            return colliders.Length == 0f;
        }
    }
}

