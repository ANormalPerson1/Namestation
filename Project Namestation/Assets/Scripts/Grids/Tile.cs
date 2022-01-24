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
            if (currentParent != null && transform.parent == null)
            {
                transform.parent = currentParent;
                transform.localPosition = new Vector2(position.x, position.y);
                currentParent.GetComponent<BuildingGrid>().tiles.Add(this);
            }
        }


        public SerializableTile GetSerializableTile()
        {
            return new SerializableTile(this);
        }

        public TileObject GetPlacedByName(string name)
        {
            foreach (TileObject tileObject in tileObjects)
            {
                if (tileObject.name.Equals(name))
                {
                    return tileObject;
                }
            }

            return null;
        }

        public bool ContainsPlacedName(string name)
        {
            return GetPlacedByName(name) != null;
        }

        public bool ContainsPlacedLayer(Layer layer)
        {
            foreach (TileObject tileObject in tileObjects)
            {
                if (tileObject.layer == layer)
                {
                    return true;
                }
            }
            return false;
        }


        public Tile[] GetAdjacentTiles()
        {
            Tile[] connectedTiles = new Tile[4];

            Tile topTile = GetAdjacentTile(currentParent.up);
            Tile leftTile = GetAdjacentTile(-currentParent.right);
            Tile bottomTile = GetAdjacentTile(-currentParent.up);
            Tile rightTile = GetAdjacentTile(currentParent.right);
        

            connectedTiles[0] = topTile;
            connectedTiles[1] = leftTile;
            connectedTiles[2] = bottomTile;
            connectedTiles[3] = rightTile;

            return connectedTiles;
        }

        private Tile GetAdjacentTile(Vector3 direction)
        {
            float zRotation = transform.eulerAngles.z;

            Vector3 checkPosition = transform.position + direction;

            LayerMask floorLayerMask = 1 << 7;

            Collider2D candidateCollider = Physics2D.OverlapBox(checkPosition, Vector2.one * 0.1f, zRotation, floorLayerMask);
            if (candidateCollider != null)
            {
                if (candidateCollider.GetComponentInParent<Tile>())
                {
                    return candidateCollider.GetComponentInParent<Tile>();
                }
            }
            return null;
        }
    }

    [Serializable]
    public class SerializableTile
    {
        public Vector2Int position;
        public List<string> tileObjectNames = new List<string>();
        public List<string> tileObjectsJSON = new List<string>();

        public SerializableTile(Tile tile)
        {
            position = tile.position;
            foreach (TileObject tileObject in tile.tileObjects)
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
        }
    }

}
