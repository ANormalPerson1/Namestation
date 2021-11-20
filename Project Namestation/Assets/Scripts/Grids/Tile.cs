using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Namestation.Grids
{
    public class Tile : NetworkBehaviour
    {
        [SyncVar, HideInInspector] public Vector2Int position;
        [SyncVar, HideInInspector, NonSerialized] public Transform currentParent;
        [SyncVar, HideInInspector] public List<TileObject> tileObjects;

        private void Start()
        {
            TryAssignValues();
        }

        public void TryAssignValues() //Basically syncvar, but for gameobject parent, position and name
        {
            if (currentParent != null)
            {
                transform.parent = currentParent;
                transform.localPosition = new Vector2(position.x, position.y);
            }
        }

        public SerializableTile GetSerializableTile ()
        {
            return new SerializableTile(this);
        }

        public bool HasPlacedLayer(Layer layer)
        {
            foreach(TileObject tileObject in tileObjects)
            {
                if(tileObject.layer == layer)
                {
                    return true;
                }
            }
            return false;
        }
    }

    [Serializable]
    public class SerializableTile
    {
        public Vector2Int position;
        public Transform currentParent;
        public List<string> tileObjectNames = new List<string>();
        public List<string> tileObjectsJSON = new List<string>();

        public SerializableTile (Tile tile)
        {
            position = tile.position;
            currentParent = tile.currentParent;
            foreach(TileObject tileObject in tile.tileObjects)
            {
                tileObjectNames.Add(tileObject.tileName);
                tileObjectsJSON.Add(JsonUtility.ToJson(tileObjectsJSON));
            }
        }
    }

    [Serializable]
    public class TileWrapper
    {
        public List<SerializableTile> serializableTiles;

        public TileWrapper(List<Tile> tiles)
        {
            foreach (Tile tile in tiles)
            {
                serializableTiles.Add(tile.GetSerializableTile());
            }
        }
    }

}
