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


        public SerializableTile GetSerializableTile()
        {
            return new SerializableTile(this);
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


            Debug.Log("Comparison: " + transform.parent);
            Tile middleTile = GetAdjacentTile(Vector3.zero);
            bool test = middleTile != null;
            Debug.Log("Test: " + test);

            Tile topTile = GetAdjacentTile(currentParent.up);
            Tile bottomTile = GetAdjacentTile(-currentParent.up);
            Tile rightTile = GetAdjacentTile(currentParent.right);
            Tile leftTile = GetAdjacentTile(-currentParent.right);

            connectedTiles[0] = topTile;
            connectedTiles[1] = bottomTile;
            connectedTiles[2] = rightTile;
            connectedTiles[3] = leftTile;

            return connectedTiles;
        }

        private Tile GetAdjacentTile(Vector3 direction)
        {
            float zRotation = transform.eulerAngles.z;

            Vector3 checkPosition = transform.position + direction;

            LayerMask floorLayerMask = 1 << 7;
            RaycastHit2D[] hit = Physics2D.LinecastAll(checkPosition + Vector3.forward, checkPosition + Vector3.back);
            if(hit.Length > 0)
            {
                Debug.Log("Raycast item count: " + hit.Length + " First item: " + hit[0].transform.name);
            }
         


            Collider2D[] candidates = Physics2D.OverlapBoxAll(checkPosition, Vector2.one * 0.1f, zRotation, floorLayerMask);
            Debug.Log(candidates.Length);

            Collider2D candidateCollider = Physics2D.OverlapBox(checkPosition, Vector2.one * 0.1f, zRotation, floorLayerMask);
            if (candidateCollider != null)
            {
                Debug.Log("Item: " + candidateCollider.name + " Position: " + candidateCollider.transform.position + "Parent: " + candidateCollider.transform.parent);
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
