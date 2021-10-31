using UnityEngine;

namespace Namestation.Grids
{
    [CreateAssetMenu(menuName = "Namestation/Grids/GridObject")]
    public class GridObject : ScriptableObject
    {
        public string structureName;
        public float health;
        public Sprite sprite;
        public int width;
        public int height;
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