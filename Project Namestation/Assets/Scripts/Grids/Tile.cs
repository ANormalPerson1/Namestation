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
        [SyncVar, HideInInspector] public Transform currentParent;
        public List<TileObject> tileObjects = new List<TileObject>();

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

                currentParent.GetComponent<BuildingGrid>().tiles.Add(this);
            }
        }


        public SerializableTile GetSerializableTile ()
        {
            return new SerializableTile(this);
        }

        public bool ContainsPlacedLayer(Layer layer)
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
        public List<string> tileObjectNames = new List<string>();
        public List<string> tileObjectsJSON = new List<string>();

        public SerializableTile (Tile tile)
        {
            position = tile.position;
            foreach(TileObject tileObject in tile.tileObjects)
            {
                tileObjectNames.Add(tileObject.tileName);
                tileObjectsJSON.Add(JsonUtility.ToJson(tileObject));
            }
        }
    }

    [Serializable]
    public class TileWrapper
    {
        public List<SerializableTile> serializableTiles = new List<SerializableTile>();

        public TileWrapper(List<Tile> tiles)
        {
            foreach (Tile tile in tiles)
            {
                serializableTiles.Add(tile.GetSerializableTile());
            }
            Debug.Log(serializableTiles.Count);
        }
    }

}
