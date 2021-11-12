using UnityEngine;
using System.Collections.Generic;
using System.Text;

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

        public string ToJson()
        {
            if (gridObjects.Count <= 0) return null;
            //{ "gridName":"New Object","gridObjects":[{ "m_FileID":-172,"m_PathID":0}]}

            StringBuilder jsonString = new StringBuilder("{ \"gridName\":\"" + gridName+ "\",\"gridObjects\":[" + JsonUtility.ToJson(gridObjects[0]));
            for (int i = 1; i < gridObjects.Count; i++)
            {
                string buildingGridJson = JsonUtility.ToJson(gridObjects[i]);
                jsonString.Append("," + buildingGridJson);
            }
            jsonString.Append("]}");

            return jsonString.ToString();
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