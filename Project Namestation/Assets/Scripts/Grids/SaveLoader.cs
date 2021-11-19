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

            List<string> gridObjectsJson = serializableBuildingGrid.gridObjectWrapper.gridObjectsJSON;
            List<string> gridObjectNames = serializableBuildingGrid.gridObjectWrapper.gridObjectNames;
            for(int i = 0; i < gridObjectsJson.Count; i++)
            {
                string currentObjectName = gridObjectNames[i];
                string currentGridObject = gridObjectsJson[i];

                GridObject loadedObject = LoadGridObject(currentObjectName, currentGridObject, buildingGridObject.transform);
                buildingGrid.gridObjects.Add(loadedObject);
            }

            return buildingGrid;
        }

        public GridObject LoadGridObject(string gridObjectName, string gridObjectJSON, Transform parent)
        {
            GameObject prefab = ResourceManager.GetGridPrefab(gridObjectName);
            if (prefab == null)
            {
                Debug.LogError("Warning! Null reference expection on loading grid object prefab!");
            }

            GameObject gridObjectGO = Instantiate(prefab, Vector3.zero, parent.rotation, parent);
            NetworkServer.Spawn(gridObjectGO);
            GridObject gridObject = gridObjectGO.GetComponent<GridObject>();
            JsonUtility.FromJsonOverwrite(gridObjectJSON, gridObject);
            if (gridObject == null)
            {
                Debug.LogError("Warning! Null reference expection on spawning grid objecvt!");
                return null;
            }

            gridObject.currentParent = parent;
            gridObject.TryAssignValues();
            

            return gridObject;
        }
    }
}