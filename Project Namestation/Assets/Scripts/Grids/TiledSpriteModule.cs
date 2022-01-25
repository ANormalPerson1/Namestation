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

        public void TileSpriteServer(bool updateConnected)
        {
            Tile tile = GetComponentInParent<Tile>();
            TileObject tileObject = GetComponent<TileObject>();
            Tile[] adjacentTiles = tile.GetAdjacentTiles(); //Top, left, bottom, right

            bool hasTop = CheckHasSameLayer(tileObject, adjacentTiles[0]);
            bool hasLeft = CheckHasSameLayer(tileObject, adjacentTiles[1]);
            bool hasBottom = CheckHasSameLayer(tileObject, adjacentTiles[2]);
            bool hasRight = CheckHasSameLayer(tileObject, adjacentTiles[3]);

            if(updateConnected)
            {
                foreach(Tile adjacentTile in adjacentTiles)
                {
                    if(adjacentTile != null)
                    {
                        TileObject objectOfSameType = adjacentTile.GetPlacedByName(tileObject.name);
                        if (objectOfSameType != null)
                        {
                            TiledSpriteModule connectedTiledSpriteModule = objectOfSameType.GetComponent<TiledSpriteModule>();
                            connectedTiledSpriteModule.TileSpriteServer(false);
                        }

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

        private bool CheckHasSameLayer(TileObject ownObject, Tile objectToCheck)
        {
            return objectToCheck != null && objectToCheck.ContainsPlacedName(ownObject.name);
        }

        private void SetOwnSpriteServer(Sprite newSprite, float zRotation)
        {
            TileObject tileObject = GetComponent<TileObject>();
            tileObject.SetRotationServer(zRotation);
            tileObject.SetSpriteServer(newSprite.name);
        }
    }


}
