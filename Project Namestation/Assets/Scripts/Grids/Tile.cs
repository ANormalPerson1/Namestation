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
                GetAdjacentTiles();
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


        public Tile[] GetAdjacentTiles()
        {
            Tile[] connectedTiles = new Tile[4];

            Vector3 topPosition = transform.position + currentParent.up;
            Vector3 bottomPosition = transform.position - currentParent.up;
            Vector3 rightPosition = transform.position + currentParent.right;
            Vector3 leftPosition = transform.position - currentParent.right;

            Tile topTile = GetTile(topPosition, currentParent);
            Tile bottomTile = GetTile(bottomPosition, currentParent);
            Tile rightTile = GetTile(rightPosition, currentParent);
            Tile leftTile = GetTile(leftPosition, currentParent);

            connectedTiles[0] = topTile;
            connectedTiles[1] = bottomTile;
            connectedTiles[2] = rightTile;
            connectedTiles[3] = leftTile;

            return connectedTiles;
        }

        public static Tile GetTile(Vector3 position, Transform buildingGrid)
        {
            float zRotation = buildingGrid.eulerAngles.z;

            LayerMask floorLayerMask = 1 << 7;
            Collider2D candidateCollider = Physics2D.OverlapBox(position, Vector2.one * 0.95f, zRotation, floorLayerMask);
            if (candidateCollider != null)
            {
                Debug.Log("The gotten is: " + candidateCollider.name + " " + candidateCollider.transform.position);
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
        }
    }

}
