using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Namestation.Grids
{
    public class GridObject : MonoBehaviour
    {
        public GridObjectSO gridObjectSO;
        [HideInInspector] public Vector2Int position;
        [HideInInspector] public float currentHealth;

        public SerializableGridObject GetSerializableGridObject()
        {
            return new SerializableGridObject(this);
        }
    }

    [Serializable]
    public class SerializableGridObject
    {
        public GridObjectSO gridObjectSO;
        [HideInInspector] public Vector2Int position;
        [HideInInspector] public float currentHealth;

        public SerializableGridObject (GridObject gridObject)
        {
            gridObjectSO = gridObject.gridObjectSO;
            position = gridObject.position;
            currentHealth = gridObject.currentHealth;
        }
    }

    [Serializable]
    public class GridObjectWrapper
    {
        public List<SerializableGridObject> serializableGridObjects;

        public GridObjectWrapper (List<SerializableGridObject> serializableGridObjects)
        {
            this.serializableGridObjects = serializableGridObjects;
        }
    }
}