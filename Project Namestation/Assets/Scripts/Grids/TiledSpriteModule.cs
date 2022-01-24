using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Namestation.Grids.Utilities
{
    [RequireComponent(typeof(TileObject))]
    public class TiledSpriteModule : NetworkBehaviour
    {
        public Sprite standaloneSprite;
        public Sprite cornerSprite;
        public Sprite edgeSprite;
        public Sprite straightSprite;
        public Sprite centerSprite;
        public Sprite endSprite;

        // Start is called before the first frame update
        void Start()
        {

        }

        //TileSpriteServer should only be called on initial placement, not on start since load invokes that and only by the server!

        public void TileSpriteServer(bool updateConnected)
        {
            Tile tile = GetComponentInParent<Tile>();
            TileObject tileObject = GetComponent<TileObject>();
            Tile[] adjacentTiles = tile.GetAdjacentTiles(); //Top, left, bottom, right
            bool hasTop = adjacentTiles[0].ContainsPlacedName(tileObject.name);
            bool hasLeft = adjacentTiles[1].ContainsPlacedName(tileObject.name);
            bool hasBottom = adjacentTiles[2].ContainsPlacedName(tileObject.name);
            bool hasRight = adjacentTiles[3].ContainsPlacedName(tileObject.name);

            if(updateConnected)
            {
                foreach(Tile adjacentTile in adjacentTiles)
                {
                    TileObject objectOfSameType = adjacentTile.GetPlacedByName(tileObject.name);
                    if (objectOfSameType != null)
                    {
                        TiledSpriteModule connectedTiledSpriteModule = objectOfSameType.GetComponent<TiledSpriteModule>();
                        connectedTiledSpriteModule.TileSpriteServer(false);
                    }
                }
            }

            //Standalone sprite
            if (!hasTop && !hasLeft && !hasBottom && !hasRight)
            {
                SetOwnSpriteServer(standaloneSprite, 0f);
            }
            //End sprites
            else if(hasTop && !hasLeft && !hasBottom && !hasRight)
            {
                SetOwnSpriteServer(endSprite, 180f);
            }
            else if (!hasTop && hasLeft && !hasBottom && !hasRight)
            {
                SetOwnSpriteServer(endSprite, 270f);
            }
            else if (!hasTop && !hasLeft && hasBottom && !hasRight)
            {
                SetOwnSpriteServer(endSprite, 0f);
            }
            else if (!hasTop && !hasLeft && !hasBottom && hasRight)
            {
                SetOwnSpriteServer(endSprite, 90f);
            }
            //Corner sprites
            else if(hasTop &&  hasLeft && !hasBottom && !hasRight)
            {
                SetOwnSpriteServer(cornerSprite, 180f);
            }
            else if(!hasTop && hasLeft && hasBottom && !hasRight)
            {
                SetOwnSpriteServer(cornerSprite, 270f);
            }
            else if(!hasTop && !hasLeft && hasBottom && hasRight)
            {
                SetOwnSpriteServer(cornerSprite, 0f);
            }
            else if(hasTop && !hasLeft && !hasBottom && hasRight)
            {
                SetOwnSpriteServer(cornerSprite, 90f);
            }
            //Straight sprites
            else if(hasTop && !hasLeft && hasBottom && !hasRight)
            {
                SetOwnSpriteServer(straightSprite, 0f);
            }
            else if(!hasTop && hasLeft && !hasBottom && hasRight)
            {
                SetOwnSpriteServer(straightSprite, 90f);
            }
            //3-section sprites
            else if(hasTop && hasLeft && hasBottom && !hasRight)
            {
                SetOwnSpriteServer(edgeSprite, 180f);
            }
            else if(!hasTop && hasLeft && hasBottom && hasRight)
            {
                SetOwnSpriteServer(edgeSprite, 270f);
            }
            else if(hasTop && !hasLeft && hasBottom && hasRight)
            {
                SetOwnSpriteServer(edgeSprite, 0f);
            }
            else if(hasTop && hasLeft && !hasBottom && hasRight)
            {
                SetOwnSpriteServer(edgeSprite, 90f);
            }
            //4-section sprite
            else if(hasTop && hasLeft && hasBottom && hasRight)
            {
                SetOwnSpriteServer(centerSprite, 0f);
            }
        }

        void SetOwnSpriteServer(Sprite newSprite, float zRotation)
        {
            TileObject tileObject = GetComponent<TileObject>();
            tileObject.transform.localEulerAngles = Vector3.forward * zRotation;
            tileObject.zRotation = zRotation;
            tileObject.SetSpriteServer(newSprite.name);
        }
    }


}
