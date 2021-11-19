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

        InputManager inputManager;

        public override void Initialize()
        {
            base.Initialize();
            inputManager = playerManager.inputManager;
            buildingGridPrefab = ResourceManager.GetGridPrefab("BuildingGrid");
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
            PlaceObjectServer(currentlyBuildingPrefab.name, mousePosition, null);
        }

        private void PlaceObjectAddToExistingStructure(Vector2 mousePosition, Collider2D existingStructure)
        {
            Transform structureTransform = existingStructure.transform;
            Vector2? localPlacementPosition = ConvertRawToLocalPlacementPosition(mousePosition, structureTransform);
            if (localPlacementPosition == null) return;
            Vector2 globalPlacementPosition = structureTransform.TransformPoint(localPlacementPosition.Value);

            if (GridClear(globalPlacementPosition, structureTransform.rotation))
            {
                Debug.Log(structureTransform.parent);
                Vector2 localPositionRelativeToParent = structureTransform.parent.InverseTransformPoint(globalPlacementPosition);
                PlaceObjectServer(currentlyBuildingPrefab.name, localPositionRelativeToParent, structureTransform.parent);
            }
        }

        [Command]
        private void PlaceObjectServer(string gridObjectPrefabString, Vector2 placementPosition, Transform parent)
        {
            GameObject prefab  = ResourceManager.GetGridPrefab(gridObjectPrefabString);
            if (parent == null)
            {
                Transform newParent = Instantiate(buildingGridPrefab, placementPosition, Quaternion.identity).transform;
                NetworkServer.Spawn(newParent.gameObject);
                BuildingGrid buildingGrid = newParent.GetComponent<BuildingGrid>();
                SaveManager.buildingGrids.Add(buildingGrid);
                
                PlaceObject(prefab, placementPosition, newParent);
                SyncParentObject(buildingGrid);
            }
            else
            {
                //Position is passed locally for existing objects to reduce chance of error.
                Vector3 globalPosition = parent.TransformPoint(placementPosition);
                PlaceObject(prefab, globalPosition, parent);
            }
        }

        [ClientRpc]
        private void SyncParentObject(BuildingGrid buildingGrid)
        {
            buildingGrid.TryAssignValues();
        }

        private void PlaceObject(GameObject prefab, Vector3 position, Transform parent)
        {
            GameObject gridObjectGO = Instantiate(prefab, position, parent.rotation);
            NetworkServer.Spawn(gridObjectGO);
            GridObject gridObject = gridObjectGO.GetComponent<GridObject>();
        
            SaveSubObject(gridObject, parent.gameObject, position);
        }

        private void SaveSubObject(GridObject gridObject, GameObject parent, Vector3 placementPosition)
        {
            Vector2 localPosition = parent.transform.InverseTransformPoint(placementPosition);
            Vector2Int localPositionInt = new Vector2Int(Mathf.RoundToInt(localPosition.x), Mathf.RoundToInt(localPosition.y));

            gridObject.position = localPositionInt;
            gridObject.currentParent = parent.transform;
            gridObject.transform.parent = parent.transform;
            SyncSubObject(gridObject);

            BuildingGrid buildingGrid = parent.GetComponent<BuildingGrid>();
            buildingGrid.gridObjects.Add(gridObject);
        }

        [ClientRpc]
        private void SyncSubObject(GridObject gridObject)
        {
            gridObject.TryAssignValues();
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

