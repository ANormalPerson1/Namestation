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
    public class BuildingManager : PlayerComponent
    {
        [SerializeField] float collisionDetectionRadius;
        [SerializeField] LayerMask floorLayerMask;
        [SerializeField] GameObject currentlyBuildingPrefab;
        [SerializeField] LayerMask wallLayerMask;
        [SerializeField] LayerMask entityLayerMask;

        InputManager inputManager;

        public override void Initialize()
        {
            base.Initialize();
            inputManager = playerManager.inputManager;
        }

        private void Update()
        {
            if (!initialized || !isLocalPlayer) return;
            //REDO THIS!
            CheckPlaceObject(inputManager.globalMousePosition);
        }

        private void CheckPlaceObject(Vector2 position)
        {
            if (inputManager.interactionButtonPressed)
            {
                AttemptToPlaceObject(position);
            }
        }

        private void AttemptToPlaceObject(Vector2 position)
        {
            Collider2D[] colliders = GetCollidersNearPlacementPoint(position);
            if(colliders.Length == 0)
            {
                PlaceObjectAsNewStructure(position);
            }
            else
            {
                Collider2D nearestCollider = colliders[0];
                PlaceObjectAddToExistingStructure(position, nearestCollider);
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
            TileObject tileObject = currentlyBuildingPrefab.GetComponent<TileObject>();

            if (tileObject.CanPlaceOn(null))
            {
                PlaceObjectServer(currentlyBuildingPrefab.name, mousePosition, null, null);
            }
        }

        private void PlaceObjectAddToExistingStructure(Vector2 mousePosition, Collider2D existingTileObjectCollider)
        {
            Transform existingTileObject = existingTileObjectCollider.transform;
            Vector2? localPlacementPosition = ConvertRawToLocalPlacementPosition(mousePosition, existingTileObject);
            if (localPlacementPosition == null) return;
            Vector2 globalPlacementPosition = existingTileObject.TransformPoint(localPlacementPosition.Value);

            if (GridClear(globalPlacementPosition, existingTileObject.rotation))
            {
                PlaceObjectAddAsNewTile(existingTileObject, globalPlacementPosition);
            }
            else
            {
                PlaceObjectAddToTile(existingTileObject);
            }
        }

        private void PlaceObjectAddAsNewTile(Transform existingTileObject, Vector3 placementPosition)
        {
            Transform existingTile = existingTileObject.parent;
            Transform existingGrid = existingTile.parent;
            Vector2 localPlacementPosition = existingGrid.InverseTransformPoint(placementPosition);

            TileObject tileObject = currentlyBuildingPrefab.GetComponent<TileObject>();

            if(tileObject.CanPlaceOn(null))
            {
                PlaceObjectServer(currentlyBuildingPrefab.name, localPlacementPosition, existingGrid, null);
            }
        }

        private void PlaceObjectAddToTile(Transform existingTileObject)
        {
            Transform existingTile = existingTileObject.parent;
            Transform existingGrid = existingTile.parent;
            Vector2 localPlacementPosition = Vector2.zero;

            Tile tile = existingTile.GetComponent<Tile>();
            TileObject tileObject = currentlyBuildingPrefab.GetComponent<TileObject>();

            if(tileObject.CanPlaceOn(tile))
            {
                Debug.Log("Placed object, added to existing!");
                PlaceObjectServer(currentlyBuildingPrefab.name, localPlacementPosition, existingGrid, existingTile);
            }
            else
            {
                Debug.Log("Warning! Existing object contains tile layer!");
            }
        }


        [Command]
        private void PlaceObjectServer(string tileObjectPrefabString, Vector2 placementPosition, Transform parentGridTransform, Transform parentTileTransform)
        {
            GameObject prefab  = ResourceManager.GetGridPrefab(tileObjectPrefabString);
            GridManager serverGridManager = GridManager.instance;

            if (parentGridTransform == null)
            {
                //Position is passed globally, as there are no local references
                //Create new building grid and tile
                BuildingGrid buildingGrid = serverGridManager.CreateBuildingGridServer(placementPosition, Quaternion.identity, Vector3.zero);
                Vector2 localPlacementPosition = buildingGrid.transform.InverseTransformPoint(placementPosition);

                Tile tile = serverGridManager.CreateTileServer(buildingGrid, localPlacementPosition);
                serverGridManager. CreateTileObjectServer(prefab, tile);
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
                    tile = serverGridManager.CreateTileServer(buildingGrid, localPlacementPosition);
                }
                else
                {
                    tile = parentTileTransform.GetComponent<Tile>();
                }

                serverGridManager.CreateTileObjectServer(prefab, tile);
            }
        }

        private Vector2? ConvertRawToLocalPlacementPosition(Vector2 mousePosition, Transform structureTransform)
        {
            //Convert the position to local space, check where to place relative to the object's local grid and reconvert into world space.
            Vector2 baseLocalPosition = structureTransform.InverseTransformPoint(mousePosition);
            Vector2? localPlacementPosition = CalculateObjectLocalPlacementPosition(baseLocalPosition);
            if(localPlacementPosition != null)
            {
                return localPlacementPosition;
            }
            return null;
        }

        private Vector2? CalculateObjectLocalPlacementPosition(Vector2 baseLocalPosition)
        {
            //Place in same object if in same grid
            if (Mathf.Abs(baseLocalPosition.x) < 0.5f && Mathf.Abs(baseLocalPosition.y) < 0.5f) return Vector2.zero;

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
            Collider2D[] colliders = Physics2D.OverlapBoxAll(position, Vector2.one * 0.95f, quaternionEuler.z, floorLayerMask);
            return colliders.Length == 0f;
        }
    }
}

