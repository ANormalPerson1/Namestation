using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Namestation.Grids
{
    public class GridObject : MonoBehaviour
    {
        public int netID;
        public GridObjectSO gridObjectSO;
        [HideInInspector] public Vector2Int position;
        [HideInInspector] public float currentHealth;

        public GridObject(GridObjectSO gridObjectSO, Vector2Int position, float health)
        {
            this.gridObjectSO = gridObjectSO;
            this.position = position;
            currentHealth = health;
        }

        public SerializableGridObject GetSerializableGridObject()
        {
            return new SerializableGridObject(this);
        }
    }

    [Serializable]
    public class SerializableGridObject
    {
        public int netID;
        public GridObjectSO gridObjectSO;
        [HideInInspector] public Vector2Int position;
        [HideInInspector] public float currentHealth;

        public SerializableGridObject (GridObject gridObject)
        {
            netID = gridObject.netID;
            gridObjectSO = gridObject.gridObjectSO;
            position = gridObject.position;
            currentHealth = gridObject.currentHealth;
        }
    }

    [Serializable]
    public class GridObjectWrapper
    {
        public List<SerializableGridObject> serializableGridObjects;

        public GridObjectWrapper (List<GridObject> gridObjects)
        {
            List<SerializableGridObject> serializableGridObjects = new List<SerializableGridObject>();
            foreach (GridObject gridObject in gridObjects)
            {
                serializableGridObjects.Add(gridObject.GetSerializableGridObject());
            }

            this.serializableGridObjects = serializableGridObjects;
        }
    }
}