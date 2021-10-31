using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Namestation.Grids;
using System.Linq;
//using Namestation.Grids;

namespace Namestation.Player
{
    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] float collisionDetectionRadius;
        [SerializeField] GameObject emptyGameObject;
        [SerializeField] GameObject floorPrefab;
        [SerializeField] LayerMask floorLayerMask;
        [SerializeField] LayerMask wallLayerMask;
        [SerializeField] LayerMask entityLayerMask;

        PlayerComponents playerComponents;
        InputManager inputManager;

        private void Start()
        {
            playerComponents = PlayerComponents.instance;
            inputManager = playerComponents.inputManager;
        }

        private void Update()
        {
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
            Transform parent = Instantiate(emptyGameObject, mousePosition, Quaternion.identity).transform;
            parent.gameObject.name = "New Object";
            GameObject floor = Instantiate(floorPrefab, mousePosition, Quaternion.identity, parent: parent);
            floor.name = "Floor";
        }

        private void PlaceObjectAddToExistingStructure(Vector2 mousePosition, Collider2D existingStructure)
        {
            Transform structureTransform = existingStructure.transform;
            Vector2? placementPosition = ConvertRawToPlacementPosition(mousePosition, structureTransform);
            if (placementPosition == null) return;

            if(GridClear(placementPosition.Value, structureTransform.rotation))
            {
                GameObject floor = Instantiate(floorPrefab, placementPosition.Value, structureTransform.rotation, parent: structureTransform.parent);
                floor.name = "Floor";
            }
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

