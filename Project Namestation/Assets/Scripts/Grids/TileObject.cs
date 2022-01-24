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
        [SyncVar] public float zRotation;
        [SyncVar, HideInInspector, SerializeField] private string currentSpriteName;
        [SyncVar, HideInInspector, NonSerialized] public Transform currentParent;
        [SyncVar] public Layer layer;

        private void Start()
        {
            TryAssignValues();
        }

        public void TryAssignValues() //As transform, ect. can not be networked directly, it is done via this function.
        {
            if(transform.parent == null && currentParent != null)
            {
                LoadTransform();
                LoadGameObject();
                LoadSprite();
                AddToTile();
            }
        }

        private void LoadTransform()
        {
            transform.parent = currentParent;
            transform.localPosition = Vector2.zero;
            transform.localEulerAngles = Vector3.forward * zRotation;
        }

        private void LoadGameObject()
        {
            gameObject.name = tileName;
        }

        private void LoadSprite()
        {
            Sprite sprite = ResourceManager.GetSprite(currentSpriteName);
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
        }

        private void AddToTile()
        {
            Tile tile = currentParent.GetComponent<Tile>();
            tile.tileObjects.Add(this);
        }

        public bool HasStoredSprite()
        {
            return !currentSpriteName.Equals("");
        }

        public string GetStoredSprite()
        {
            return currentSpriteName;
        }

        public void SetSpriteServer(string newSpriteName)
        {
            currentSpriteName = newSpriteName;
            FinalizeSpriteChange(newSpriteName);
            SetSpriteClient(newSpriteName);
            //Make only server/only client call this?
        }

        [ClientRpc]
        private void SetSpriteClient(string newSpriteName)
        {
            FinalizeSpriteChange(newSpriteName);
        }

        private void FinalizeSpriteChange(string newSpriteName)
        {
            Sprite newSprite = ResourceManager.GetSprite(newSpriteName);
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = newSprite;
        }

        public void SetRotationServer(float zRotation)
        {
            this.zRotation = zRotation;
            transform.localEulerAngles = Vector3.forward * zRotation;
            SetRotationClient(zRotation);
        }

        [ClientRpc]
        private void SetRotationClient(float zRotation)
        {
            transform.localEulerAngles = Vector3.forward * zRotation;
        }

        public bool CanPlaceOn(Tile tile)
        {
            if (tile == null)
            {
                if (layer == Layer.Floor) return true;
                return false;
            }

            if (tile.ContainsPlacedLayer(layer)) return false;


            switch (layer)
            {
                case Layer.Floor:
                    return false;
                case Layer.Wire:
                case Layer.Pipe:
                    return tile.ContainsPlacedLayer(Layer.Floor);
                case Layer.Wall:
                case Layer.Furniture:
                    bool containsSimilarObjects = tile.ContainsPlacedLayer(Layer.Wall) || tile.ContainsPlacedLayer(Layer.Furniture);
                    if (tile.ContainsPlacedLayer(Layer.Floor) && !containsSimilarObjects) return true;
                    return false;
                case Layer.WallMount:
                    return tile.ContainsPlacedLayer(Layer.Wall);
                default:
                    return false;

            }
        }
    }

    public enum Layer
    {
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
            foreach (TileObject tileObject in tileObjects)
            {
                tileObjectsJSON.Add(JsonUtility.ToJson(tileObject));
                tileObjectNames.Add(tileObject.tileName);
            }
        }
    }
}