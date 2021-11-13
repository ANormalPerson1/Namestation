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
        [SyncVar] public Vector2Int position;
        [HideInInspector, SyncVar] public float currentHealth;
        [HideInInspector, SyncVar] public Transform currentParent;

        

        private void Start()
        {
            TryAssignValues();
        }

        public void TryAssignValues()
        {
            if(currentParent != null)
            {
                transform.parent = currentParent;
                gameObject.name = gridName;
                transform.localPosition = new Vector2(position.x, position.y);
            }
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
        public string scriptableObjectName;
        [HideInInspector] public Vector2Int position;
        [HideInInspector] public float currentHealth;

        public SerializableGridObject(GridObject gridObject)
        {
            scriptableObjectName = gridObject.gridObjectSO.name;
            position = gridObject.position;
            currentHealth = gridObject.currentHealth;
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