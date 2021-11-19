using Mirror;
using UnityEngine;
using Namestation.Interactables;
using Namestation.Grids;
using System.Collections;
using System.Collections.Generic;

namespace Namestation.Saving
{
    public class SaveLoader : MonoBehaviour
    {
        GameObject buildingGridPrefab;

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
            buildingGridPrefab = ResourceManager.GetGridPrefab("BuildingGrid");
            SaveManager.Load();
        }


        public BuildingGrid LoadBuildingGrid(SerializableBuildingGrid serializableBuildingGrid)
        {
            if (serializableBuildingGrid == null)
            {
                Debug.LogError("Warning! Null reference expection on loading building grid!");
                return null;
            }

            Vector3 gridPosition = serializableBuildingGrid.gridPosition;
            Vector3 gridVelocity = serializableBuildingGrid.gridVelocity;
            Quaternion gridRotation = serializableBuildingGrid.gridRotation;

            GameObject buildingGridObject = Instantiate(buildingGridPrefab, gridPosition, gridRotation);
            NetworkServer.Spawn(buildingGridObject);
            BuildingGrid buildingGrid = buildingGridObject.GetComponent<BuildingGrid>();
            Rigidbody2D rigidBody2D = buildingGridObject.GetComponent<Rigidbody2D>();
            rigidBody2D.velocity = gridVelocity;

            buildingGrid.gridName = serializableBuildingGrid.gridName;
            buildingGridObject.name = buildingGrid.gridName;

            List<string> tileObjectsJSON = serializableBuildingGrid.tileObjectWrapper.tileObjectsJSON;
            List<string> tileObjectNames = serializableBuildingGrid.tileObjectWrapper.tileObjectNames;
            for(int i = 0; i < tileObjectsJSON.Count; i++)
            {
                string currentObjectName = tileObjectNames[i];
                string currentTileObject = tileObjectsJSON[i];

                TileObject loadedObject = LoadTileObject(currentObjectName, currentTileObject, buildingGridObject.transform);
                buildingGrid.tileObjects.Add(loadedObject);
            }

            return buildingGrid;
        }

        public TileObject LoadTileObject(string tileObjectName, string tileObjectJSON, Transform parent)
        {
            GameObject prefab = ResourceManager.GetGridPrefab(tileObjectName);
            if (prefab == null)
            {
                Debug.LogError("Warning! Null reference expection on loading grid object prefab!");
            }

            GameObject tileObjectGO = Instantiate(prefab, Vector3.zero, parent.rotation, parent);
            NetworkServer.Spawn(tileObjectGO);
            TileObject tileObject = tileObjectGO.GetComponent<TileObject>();
            JsonUtility.FromJsonOverwrite(tileObjectJSON, tileObject);
            if (tileObject == null)
            {
                Debug.LogError("Warning! Null reference expection on spawning grid objecvt!");
                return null;
            }

            tileObject.currentParent = parent;
            tileObject.TryAssignValues();
            

            return tileObject;
        }
    }
}