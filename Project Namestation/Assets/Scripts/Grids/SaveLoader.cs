using Mirror;
using UnityEngine;
using Namestation.Interactables;
using Namestation.Grids;

namespace Namestation.Saving;
public class SaveLoader : NetworkBehaviour
{
    public static SaveLoader instance;
    private void Awake()
    {
        if (!isServer) return;
        if (instance != null) 
        {
            Debug.LogError("More than 1 instance of saveloader found!");
            return;
        }
       
        instance = this;
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

        GameObject buildingGridObject = Instantiate(BuildableCollection.instance.buildables[2], gridPosition, gridRotation);
        BuildingGrid buildingGrid = buildingGridObject.GetComponent<BuildingGrid>();
        Rigidbody rigidBody = buildingGridObject.GetComponent<Rigidbody>();
        rigidBody.velocity = gridVelocity;

        buildingGrid.gridName = serializableBuildingGrid.gridName;
        buildingGridObject.name = buildingGrid.gridName;

        foreach (SerializableGridObject serializableGridObject in serializableBuildingGrid.gridObjectWrapper.serializableGridObjects)
        {
            GridObject gridObject = LoadGridObject(serializableGridObject);
            buildingGrid.gridObjects.Add(gridObject);
        }

        return buildingGrid;
    }

    public GridObject LoadGridObject(SerializableGridObject serializableGridObject)
    {
        if (serializableGridObject == null)
        {
            Debug.Log("Warning! Null reference expection on loading grid object!");
            return null;
        }

        if (serializableGridObject.gridObjectSO == null)
        {
            Debug.Log("Warning! Null reference expection on loading grid object scriptable object!");
            return null;
        }

        string gridName = serializableGridObject.gridObjectSO.gridObjectName;
        GridObjectSO gridObjectSO = serializableGridObject.gridObjectSO;
        float currentHealth = serializableGridObject.currentHealth;

        Transform currentParent = serializableGridObject.currentParent;
        Vector2Int localPosition = serializableGridObject.position;
        
        //Get by name dependent on a variable

        GameObject buildingGridObject = Instantiate(BuildableCollection.instance.buildables[2], Vector3.zero, currentParent.rotation, currentParent);
        buildingGridObject.transform.localPosition = new Vector2(localPosition.x, localPosition.y);
        GridObject gridObject = buildingGridObject.GetComponent<GridObject>();
        gridObject.gridName = gridName;
        gridObject.gridObjectSO = gridObjectSO;
        gridObject.currentHealth = currentHealth;

        return gridObject;
    }
}
