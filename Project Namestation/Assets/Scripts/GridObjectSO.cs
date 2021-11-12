using UnityEngine;

namespace Namestation.Grids
{
    [CreateAssetMenu(menuName = "Namestation/GridObject")]
    public class GridObjectSO : ScriptableObject
    {
        public string structureName;
        public float health;
        public int spriteIndex;
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