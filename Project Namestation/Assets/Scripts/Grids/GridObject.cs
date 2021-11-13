using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;
namespace Namestation.Grids
{
    public class GridObject : NetworkBehaviour
    {
        [SyncVar] public string gridName;
        [SyncVar] public GridObjectSO gridObjectSO;
        [HideInInspector, SyncVar] public Vector2Int position;
        [HideInInspector, SyncVar] public float currentHealth;
        [HideInInspector, SyncVar] public Transform currentParent;

        

        private void Start()
        {
            //Transform.parent/name can't be synhronized directly, thus they have to be saved and loaded seperately
            //if(currentParent != null) transform.parent = currentParent;
            //if(gridName != null) gameObject.name = gridName;
            TryAssignValues();
        }

        public void TryAssignValues()
        {
            if(currentParent != null) transform.parent = currentParent;
            if(gridName != null) gameObject.name = gridName;
        }

        public GridObject()
        {

        }

        public GridObject(GridObjectSO gridObjectSO, Vector2Int position, float health, Transform parent)
        {
            this.gridObjectSO = gridObjectSO;
            this.position = position;
            currentHealth = health;
            currentParent = parent;
        }

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
        [HideInInspector] public Transform currentParent;

        public SerializableGridObject(GridObject gridObject)
        {
            gridObjectSO = gridObject.gridObjectSO;
            position = gridObject.position;
            currentHealth = gridObject.currentHealth;
            currentParent = gridObject.currentParent;
        }
    }

    [Serializable]
    public class GridObjectWrapper
    {
        public List<SerializableGridObject> serializableGridObjects;

        public GridObjectWrapper(List<GridObject> gridObjects)
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