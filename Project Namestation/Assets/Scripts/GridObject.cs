using UnityEngine;

namespace Namestation.Grids
{
    public class GridObject : MonoBehaviour
    {
        public string structureName;
        public float health;
        public int spriteIndex;
        [HideInInspector] public Vector2Int position;
        public ObjectType type;

        public override string ToString()
        {
            return structureName;
        }
    }

    public enum ObjectType
    {
        Wall,
        Underfloor,
        Wire,
        Pipe,
        Floor
    }
}