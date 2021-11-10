using System.Collections;
using System.Collections.Generic;
using Namestation.Interactables;
using Namestation.Grids;
using UnityEngine;
using System.Linq;
using Mirror;

namespace Namestation.Player
{
    public class InteractionManager : PlayerComponent
    {
        [SerializeField] float collisionDetectionRadius;
        [SerializeField] LayerMask floorLayerMask;
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
            PlaceObjectServer(0, mousePosition, null);
        }

        private void PlaceObjectAddToExistingStructure(Vector2 mousePosition, Collider2D existingStructure)
        {
            Transform structureTransform = existingStructure.transform;
            Vector2? localPlacementPosition = ConvertRawToLocalPlacementPosition(mousePosition, structureTransform);
            if (localPlacementPosition == null) return;
            Vector2 globalPlacementPosition = structureTransform.TransformPoint(localPlacementPosition.Value);

            if (GridClear(globalPlacementPosition, structureTransform.rotation))
            {
                BuildingGrid buildingGrid = structureTransform.parent.GetComponent<BuildingGrid>();
                PlaceObjectServer(0, localPlacementPosition.Value, buildingGrid.gridName);
            }
        }

        [Command]
        private void PlaceObjectServer(int objectIndex, Vector2 placementPosition, string parentObjectName)
        {
            PlaceObjectClient(objectIndex, placementPosition, parentObjectName);
        }

        [ClientRpc]
        private void PlaceObjectClient(int objectIndex, Vector2 placementPosition, string parentObjectName)
        {
            //We need some other way of getting the parent, as this currently only respesents the local parent! Oh no!

            if(parentObjectName == null)
            {
                PlaceAsNewObjectClient(objectIndex, placementPosition);
            }
            else
            {
                AddObjectToExistingClient(objectIndex, placementPosition, parentObjectName);
            }
        }

        private void PlaceAsNewObjectClient(int objectIndex, Vector2 globalPosition)
        {
            Transform parent = Instantiate(BuildableCollection.instance.buildables[2], globalPosition, Quaternion.identity).transform;
            parent.name = "New Object";
            //Complete this! You need some way of getting the parent here!!!

            parent.gameObject.AddComponent<BuildingGrid>();
            GameObject prefab = BuildableCollection.instance.buildables[objectIndex];
            AddSubObjectClient(prefab, globalPosition, parent);
        }


        private void AddObjectToExistingClient(int objectIndex, Vector2 localPosition, string parentObjectName)
        {
            BuildableCollection buildableCollection = BuildableCollection.instance;
            Transform parent = null;
            foreach (BuildingGrid buildingGrid in buildableCollection.buildingGrids)
            {
                if(buildingGrid.gridName.Equals(parentObjectName))
                {
                    parent = buildingGrid.transform;
                    break;
                }
            }

            if(parent == null)
            {
                Debug.LogError("Warning! Parent not found!");
                return;
            }

            Vector3 globalPosition = parent.TransformPoint(localPosition);
            GameObject prefab = BuildableCollection.instance.buildables[objectIndex];
            AddSubObjectClient(prefab, globalPosition, parent);
        }

        private void AddSubObjectClient(GameObject prefab, Vector3 globalPosition, Transform parent)
        {
            GameObject newObjectInstance = Instantiate(prefab, globalPosition, parent.rotation, parent);
            newObjectInstance.name = "New Subobject";

            BuildingGrid buildingGrid = parent.GetComponent<BuildingGrid>();

            GridObject gridObject = newObjectInstance.GetComponent<GridObject>();
            Vector2 localPosition = parent.InverseTransformPoint(globalPosition);
            gridObject.position = new Vector2Int(Mathf.RoundToInt(localPosition.x), Mathf.RoundToInt(localPosition.y));

            buildingGrid.gridObjects.Add(gridObject);
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

