using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Namestation.Grids
{
    [Serializable]
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
    public class TileWrapper
    {
        public List<Tile> tiles;

        public TileWrapper(List<Tile> tiles)
        {
            foreach (Tile tile in tiles)
            {
                this.tiles.Add(tile);
            }
        }
    }

}
