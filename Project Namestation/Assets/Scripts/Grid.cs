using UnityEngine;

//This is an absolute piece of shit and garbage code, as it does not work with differently rotated objects. REWORK.
//Ideas: Per objects grid system or expanding collider system.
namespace Namestation.Grids
{
    public class Grid
    {
        public int width;
        public int height;
        public float cellSize;

        public GridObject[,] grids;

        public Grid(int width, int height, float cellSize = 1f)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            grids = new GridObject[width, height];
        }

        public Vector2 vectorToMousePosition (Vector3 input)
        {
            Vector2 gridPosition;
            gridPosition.x = Mathf.Round(input.x);
            gridPosition.y = Mathf.Round(input.y);
            return gridPosition;
        }
    }
}