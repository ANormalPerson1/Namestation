using UnityEngine;
using System.Collections.Generic;

namespace Namestation.Grids
{
    /// <summary>
    /// Grids used by the namestation building system (Examples: Station, asteroid, planet, ship)
    /// </summary>
    public class BuildingGrid : MonoBehaviour
    {
        public string gridName;
        public List<GridObject> gridObjects;

        public BuildingGrid (string newName)
        {
            gridName = newName;
        }

        /// <summary>
        /// Converts a world position to a position in the grid.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2Int ConvertPositionToGridPosition(Vector2 position)
        {
            //As the BuildingGrid is attached to the parent object, we can use inversetransformpoint to get a local position.
            Vector2 gridPosition = transform.InverseTransformPoint(position);
            Vector2Int gridPositionInt = new Vector2Int(Mathf.RoundToInt(gridPosition.x), Mathf.RoundToInt(gridPosition.y));
            return gridPositionInt;
        }
    }
}