using System.Collections;
using System.Collections.Generic;
using Namestation.Interactables;
using Namestation.Grids;
using Namestation.Saving;
using UnityEngine;
using System.Linq;
using Mirror;

namespace Namestation.Player
{
    public class InteractionManager : PlayerComponent
    {
        [SerializeField] float collisionDetectionRadius;
        [SerializeField] LayerMask floorLayerMask;
        [SerializeField] GameObject currentlyBuildingPrefab;
        [SerializeField] LayerMask wallLayerMask;
        [SerializeField] LayerMask entityLayerMask;

        GameObject buildingGridPrefab;
        GameObject tilePrefab;

        InputManager inputManager;

        public override void Initialize()
        {
            base.Initialize();
            inputManager = playerManager.inputManager;
            buildingGridPrefab = ResourceManager.GetGridPrefab("BuildingGrid");
            tilePrefab = ResourceManager.GetGridPrefab("Tile");
        }

        private void Update()
        {
            if (!initialized || !isLocalPlayer) return;
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
            PlaceObjectServer(currentlyBuildingPrefab.name, mousePosition, null, null);
        }

        private void PlaceObjectAddToExistingStructure(Vector2 mousePosition, Collider2D existingTileObject)
        {
            Transform tileObjectTransform = existingTileObject.transform;
            Vector2? localPlacementPosition = ConvertRawToLocalPlacementPosition(mousePosition, tileObjectTransform);
            if (localPlacementPosition == null) return;
            Vector2 globalPlacementPosition = tileObjectTransform.TransformPoint(localPlacementPosition.Value);

            if (GridClear(globalPlacementPosition, tileObjectTransform.rotation))
            {
                Vector2 localPositionRelativeToParent = tileObjectTransform.parent.InverseTransformPoint(globalPlacementPosition);
                Transform currentTileTransform = tileObjectTransform.parent;
                Transform currentGridTransform = currentTileTransform.parent;
                PlaceObjectServer(currentlyBuildingPrefab.name, localPositionRelativeToParent, currentTileTransform);
            }
        }

        [Command]
        private void PlaceObjectServer(string tileObjectPrefabString, Vector2 placementPosition, Transform parentGridTransform, Transform parentTileTransform)
        {
            GameObject prefab  = ResourceManager.GetGridPrefab(tileObjectPrefabString);
            BuildingManager serverBuildingManager = BuildingManager.instance;

            if (parentGridTransform == null)
            {
                //Position is passed globally, as there are no local references
                //Create new building grid and tile
                BuildingGrid buildingGrid = serverBuildingManager.CreateBuildingGridServer(placementPosition, Quaternion.identity, Vector3.zero);
                Vector2 localPlacementPosition = buildingGrid.transform.InverseTransformPoint(placementPosition);

                Tile tile = serverBuildingManager.CreateTileServer(buildingGrid, localPlacementPosition);
                serverBuildingManager. CreateTileObjectServer(prefab, tile);
            }
            else
            {
                //Position is passed locally for existing objects to reduce chance of error.
                //Add to existing object
                BuildingGrid buildingGrid = parentGridTransform.GetComponent<BuildingGrid>();
                Vector2 localPlacementPosition = placementPosition;

                Tile tile;
                if (parentTileTransform == null)
                {
                    tile = serverBuildingManager.CreateTileServer(buildingGrid, localPlacementPosition);
                }
                else
                {
                    tile = parentTileTransform.GetComponent<Tile>();
                }

                serverBuildingManager.CreateTileObjectServer(prefab, tile);
            }
        }

        private Vector2? ConvertRawToLocalPlacementPosition(Vector2 mousePosition, Transform structureTransform)
        {
            //Convert the position to local space, check where to place relative to the object's local grid and reconvert into world space.
            Vector2 baseLocalPosition = structureTransform.InverseTransformPoint(mousePosition);
            Vector2? localPlacementPosition = CalculateObjectLocalPlacementPosition(baseLocalPosition.normalized);
            if(localPlacementPosition != null)
            {
                return localPlacementPosition;
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

