using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Namestation.Grids.Utilities
{
    [RequireComponent(typeof(TileObject))]
    public class TiledSpriteModule : MonoBehaviour
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

        void TileOwnSprite()
        {
            Tile tile = GetComponentInParent<Tile>();
            Tile[] adjacentTiles = tile.GetAdjacentTiles(); //Top, left, bottom, right
            bool hasTop = adjacentTiles[0] != null;
            bool hasLeft = adjacentTiles[1] != null;
            bool hasBottom = adjacentTiles[2] != null;
            bool hasRight = adjacentTiles[3] != null;

            //Standalone sprite
            if(!hasTop && !hasLeft && !hasBottom && !hasRight)
            {
                SetOwnSprite(standaloneSprite, 0f);
            }
            //End sprites
            else if(hasTop && !hasLeft && !hasBottom && !hasRight)
            {
                SetOwnSprite(endSprite, 180f);
            }
            else if (!hasTop && hasLeft && !hasBottom && !hasRight)
            {
                SetOwnSprite(endSprite, 270f);
            }
            else if (!hasTop && !hasLeft && hasBottom && !hasRight)
            {
                SetOwnSprite(endSprite, 0f);
            }
            else if (!hasTop && !hasLeft && !hasBottom && hasRight)
            {
                SetOwnSprite(endSprite, 90f);
            }
            //Corner sprites
            else if(hasTop &&  hasLeft && !hasBottom && !hasRight)
            {
                SetOwnSprite(cornerSprite, 180f);
            }
            else if(!hasTop && hasLeft && hasBottom && !hasRight)
            {
                SetOwnSprite(cornerSprite, 270f);
            }
            else if(!hasTop && !hasLeft && hasBottom && hasRight)
            {
                SetOwnSprite(cornerSprite, 0f);
            }
            else if(hasTop && !hasLeft && !hasBottom && hasRight)
            {
                SetOwnSprite(cornerSprite, 90f);
            }
            //Straight sprites
            else if(hasTop && !hasLeft && hasBottom && !hasRight)
            {
                SetOwnSprite(straightSprite, 0f);
            }
            else if(!hasTop && hasLeft && !hasBottom && hasRight)
            {
                SetOwnSprite(straightSprite, 90f);
            }
            //3-section sprites
            else if(hasTop && hasLeft && hasBottom && !hasRight)
            {
                SetOwnSprite(edgeSprite, 180f);
            }
            else if(!hasTop && hasLeft && hasBottom && hasRight)
            {
                SetOwnSprite(edgeSprite, 270f);
            }
            else if(hasTop && !hasLeft && hasBottom && hasRight)
            {
                SetOwnSprite(edgeSprite, 0f);
            }
            else if(hasTop && hasLeft && !hasBottom && hasRight)
            {
                SetOwnSprite(edgeSprite, 90f);
            }
            //4-section sprite
            else if(hasTop && hasLeft && hasBottom && hasRight)
            {
                SetOwnSprite(centerSprite, 0f);
            }
        }

        void SetOwnSprite(Sprite newSprite, float zRotation)
        {
            TileObject tileObject = GetComponent<TileObject>();
            tileObject.SetSpriteServer(newSprite.name);
        }

        void TileConnectedSprites()
        {

        }
    }


}
