using Mirror;
using UnityEngine;
using Namestation.Interactables;
using Namestation.Grids;

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
            Debug.Log("cautious test");
            buildingGridPrefab = ResourceManager.GetGridPrefab("BuildingGrid");
            SaveManager.Load();
        }


        public BuildingGrid LoadBuildingGrid(SerializableBuildingGrid serializableBuildingGrid)
        {
            if (serializableBuildingGrid == null)
            {
                Debug.Log("Warning! Null reference expection on loading building grid!");
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

            foreach (SerializableGridObject serializableGridObject in serializableBuildingGrid.gridObjectWrapper.serializableGridObjects)
            {
                GridObject gridObject = LoadGridObject(serializableGridObject, buildingGridObject.transform);
                buildingGrid.gridObjects.Add(gridObject);
            }

            return buildingGrid;
        }

        public GridObject LoadGridObject(SerializableGridObject serializableGridObject, Transform parent)
        {
            if (serializableGridObject == null)
            {
                Debug.Log("Warning! Null reference expection on loading grid object!");
                return null;
            }

            string scriptableObjectName = serializableGridObject.scriptableObjectName;
            GridObjectSO gridObjectSO = ResourceManager.GetGridObjectSO(scriptableObjectName);
            if (gridObjectSO == null)
            {
                Debug.Log("Warning! Null reference expection on loading grid object scriptable object!");
                return null;
            }

            string gridName = serializableGridObject.scriptableObjectName;
            float currentHealth = serializableGridObject.currentHealth;

            Vector2Int localPosition = serializableGridObject.position;

            //Get by name dependent on a variable! Change this to enum!
            GameObject prefab = ResourceManager.GetGridPrefab(gridObjectSO.type.ToString());
            GameObject gridObjectGO = Instantiate(prefab, Vector3.zero, parent.rotation, parent);
            NetworkServer.Spawn(gridObjectGO);
            Debug.Log(gridObjectGO);
            gridObjectGO.transform.localPosition = new Vector2(localPosition.x, localPosition.y);
         
            GridObject gridObject = gridObjectGO.GetComponent<GridObject>();
            Debug.Log(gridObject);
            gridObject.gridName = gridName;
            gridObject.gridObjectSO = gridObjectSO;
            gridObject.currentHealth = currentHealth;
            gridObject.currentParent = parent;
            gridObject.position = localPosition;

            return gridObject;
        }
    }
}