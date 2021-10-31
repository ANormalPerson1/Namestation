using UnityEngine;

namespace Namestation
{
    public class MouseUtilities
    {
        public static Vector2 mouseToGridPosition(Vector3 mousePosition)
        {
            Vector2 gridPosition;
            gridPosition.x = Mathf.Round(mousePosition.x);
            gridPosition.y = Mathf.Round(mousePosition.y);
            return gridPosition;
        }
    }
}
