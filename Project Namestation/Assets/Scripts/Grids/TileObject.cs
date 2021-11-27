using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
namespace Namestation.Grids
{
    [Serializable]
    public class TileObject : NetworkBehaviour
    {
        [SyncVar] public string tileName;
        [SyncVar] public float currentHealth;
        [SyncVar, HideInInspector, NonSerialized] public Transform currentParent;
        [SyncVar] public Layer layer;

        private void Start()
        {
            TryAssignValues();
        }

        public void TryAssignValues() //Basically syncvar, but for gameobject parent, position and name
        {
            if(currentParent != null && transform.parent == null)
            {
                transform.parent = currentParent;
                transform.localPosition = Vector2.zero;
                gameObject.name = tileName;
                currentParent.GetComponent<Tile>().tileObjects.Add(this);
            }
        }

        public bool CanPlaceOn(Tile tile)
        {
            if(tile == null)
            {
                if (layer == Layer.Scaffhold) return true;
                return false;
            }

            if (tile.ContainsPlacedLayer(layer)) return false;


            switch(layer)
            {
                case Layer.Scaffhold:
                    return false;
                case Layer.Plating:
                    if (tile.ContainsPlacedLayer(Layer.Scaffhold)) return true;
                    return false;
                case Layer.Wire:
                case Layer.Pipe:
                    if (tile.ContainsPlacedLayer(Layer.Plating) && !tile.ContainsPlacedLayer(Layer.Floor)) return true;
                    return false;
                case Layer.Floor:
                    if (tile.ContainsPlacedLayer(Layer.Plating)) return true;
                    return false;
                case Layer.Wall:
                case Layer.Furniture:
                    bool containsSimilarObjects = tile.ContainsPlacedLayer(Layer.Wall) || tile.ContainsPlacedLayer(Layer.Furniture);
                    if (tile.ContainsPlacedLayer(Layer.Floor) && !containsSimilarObjects) return true;
                    return false;
                default:
                    return false;

            }
        }
    }

    public enum Layer
    {
        Scaffhold,
        Plating,
        Floor,
        Wall,
        WallMount,
        Furniture,
        Wire,
        Pipe
    }

    [Serializable]
    public class TileObjectWrapper
    {
        public List<string> tileObjectNames = new List<string>();
        public List<string> tileObjectsJSON = new List<string>();

        public TileObjectWrapper(List<TileObject> tileObjects)
        {
            foreach(TileObject tileObject in tileObjects)
            {
                tileObjectsJSON.Add(JsonUtility.ToJson(tileObject));
                tileObjectNames.Add(tileObject.tileName);
            }
        }
    }
}