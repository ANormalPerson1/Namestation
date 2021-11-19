using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;
namespace Namestation.Grids
{
    [Serializable]
    public class GridObject : NetworkBehaviour
    {
        [SyncVar] public string gridName;
        [SyncVar] public float currentHealth;
        [SyncVar, HideInInspector] public Vector2Int position;
        [SyncVar, HideInInspector, NonSerialized] public Transform currentParent;
        [SyncVar] public ObjectType type;

        private void Start()
        {
            TryAssignValues();
        }

        public void TryAssignValues() //Basically syncvar, but for gameobject parent, position and name
        {
            if(currentParent != null)
            {
                transform.parent = currentParent;
                gameObject.name = gridName;
                transform.localPosition = new Vector2(position.x, position.y);
            }
        }
    }

    public enum ObjectType
    {
        Wall,
        Underfloor,
        Wire,
        Pipe,
        Floor
    }

    [Serializable]
    public class GridObjectWrapper
    {
        public List<string> gridObjectNames = new List<string>();
        public List<string> gridObjectsJSON = new List<string>();

        public GridObjectWrapper(List<GridObject> gridObjects)
        {
            foreach(GridObject gridObject in gridObjects)
            {
                gridObjectsJSON.Add(JsonUtility.ToJson(gridObject));
                gridObjectNames.Add(gridObject.gridName);
            }
        }
    }
}