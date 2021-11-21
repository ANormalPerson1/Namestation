using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Namestation.Grids
{
    /// <summary>
    /// Grids used by the namestation building system (Examples: Station, asteroid, planet, ship)
    /// </summary>
    public class BuildingGrid : NetworkBehaviour
    {
        [SyncVar] public string gridName;
        public List<Tile> tiles = new List<Tile>();

        private void Start()
        {
            TryAssignValues();
        }

        public void TryAssignValues() //Basically syncvar, but for gameobject parent, position and name
        {
            if (gridName != null)
            {
                gameObject.name = gridName;
            }
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
        public TileWrapper tileWrapper;

        public SerializableBuildingGrid(BuildingGrid buildingGrid, Vector2 velocity, Vector2 position, Quaternion rotation)
        {
            gridName = buildingGrid.gridName;
            gridVelocity = velocity;
            gridPosition = position;
            gridRotation = rotation;
            tileWrapper = new TileWrapper(buildingGrid.tiles.ToList());
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
                serializableBuildingGrids.Add(buildingGrid.GetSeriarizableBuildingGrid());
            }

            this.serializableBuildingGrids = serializableBuildingGrids;
        }
    }
}