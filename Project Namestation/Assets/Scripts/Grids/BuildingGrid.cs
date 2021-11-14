using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

namespace Namestation.Grids
{
    /// <summary>
    /// Grids used by the namestation building system (Examples: Station, asteroid, planet, ship)
    /// </summary>
    public class BuildingGrid : NetworkBehaviour
    {
        [SyncVar] public string gridName;
        [HideInInspector] public List<GridObject> gridObjects;

        private void Start()
        {
            TryAssignValues();
        }

        public void TryAssignValues()
        {
            if (gridName != null) gameObject.name = gridName;
        }

        public SerializableBuildingGrid GetSeriarizableBuildingGrid()
        {
            gridName = gameObject.name;
            Rigidbody2D rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
            Vector3 gridVelocity = rigidBody2D.velocity;
            Vector3 gridPosition = transform.position;
            Quaternion gridRotation = transform.rotation;

            return new SerializableBuildingGrid(this, gridVelocity, gridPosition, gridRotation);
        }
    }

    [Serializable]
    public class SerializableBuildingGrid
    {
        public string gridName;
        public Vector2 gridVelocity;
        public Vector2 gridPosition;
        public Quaternion gridRotation;
        public GridObjectWrapper gridObjectWrapper;

        public SerializableBuildingGrid(BuildingGrid buildingGrid, Vector2 velocity, Vector2 position, Quaternion rotation)
        {
            gridName = buildingGrid.gridName;
            gridVelocity = velocity;
            gridPosition = position;
            gridRotation = rotation;
            gridObjectWrapper = new GridObjectWrapper(buildingGrid.gridObjects);
        }
    }

    [Serializable]
    public class BuildingGridWrapper
    {
        public List<SerializableBuildingGrid> serializableBuildingGrids;

        public BuildingGridWrapper(List<BuildingGrid> buildingGrids)
        {
            List<SerializableBuildingGrid> serializableBuildingGrids = new List<SerializableBuildingGrid>();
            foreach (BuildingGrid buildingGrid in buildingGrids)
            {
                Debug.Log(buildingGrid.name);
                serializableBuildingGrids.Add(buildingGrid.GetSeriarizableBuildingGrid());
            }

            this.serializableBuildingGrids = serializableBuildingGrids;
        }
    }
}