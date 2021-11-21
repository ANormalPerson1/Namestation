using UnityEngine;
using Namestation.Grids;
using System.Collections.Generic;
using Mirror;

namespace Namestation.Saving
{
    public class SaveLoader : NetworkBehaviour
    {
        GameObject buildingGridPrefab;
        [SerializeField] BuildingManager buildingManager;

        public static SaveLoader instance;
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("More than 1 instance of saveloader found!");
                return;
            }
            instance = this;
        }

        private void Start()
        {
            if (!isServer) return;
            buildingGridPrefab = ResourceManager.GetGridPrefab("BuildingGrid");
            SaveManager.Load();
        }


        public void LoadBuildingGrid(SerializableBuildingGrid serializableBuildingGrid)
        {
            if (serializableBuildingGrid == null)
            {
                Debug.LogError("Warning! Null reference expection on loading building grid!");
                return;
            }

            Vector3 gridPosition = serializableBuildingGrid.gridPosition;
            Vector3 gridVelocity = serializableBuildingGrid.gridVelocity;
            Quaternion gridRotation = serializableBuildingGrid.gridRotation;

            BuildingGrid buildingGrid = buildingManager.CreateBuildingGridServer(gridPosition, gridRotation, gridVelocity);
            foreach(SerializableTile serializableTile in serializableBuildingGrid.tileWrapper.serializableTiles)
            {
                LoadTile(buildingGrid, serializableTile);
            }
        }

        public void LoadTile(BuildingGrid buildingGrid, SerializableTile serializableTile)
        {
            Tile tile = buildingManager.CreateTileServer(buildingGrid, serializableTile.position);

            List<string> tileObjectsJSON = serializableTile.tileObjectsJSON;
            List<string> tileObjectNames = serializableTile.tileObjectNames;

            for (int i = 0; i < tileObjectsJSON.Count; i++)
            {
                string currentObjectName = tileObjectNames[i];
                string currentTileObject = tileObjectsJSON[i];

                LoadTileObject(currentObjectName, currentTileObject, tile);
            }
        }


        public void LoadTileObject(string tileObjectName, string tileObjectJSON, Tile tile)
        {
            GameObject prefab = ResourceManager.GetGridPrefab(tileObjectName);
            if (prefab == null)
            {
                Debug.LogError("Warning! Null reference expection on loading grid object prefab!");
            }

            BuildingManager.instance.CreateTileObjectServer(prefab, tile, tileObjectJSON);
        }
    }
}