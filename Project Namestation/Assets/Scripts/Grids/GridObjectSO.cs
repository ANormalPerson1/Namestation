using UnityEngine;
using System;

namespace Namestation.Grids
{
    [CreateAssetMenu(menuName = "Namestation/GridObject")]
    public class GridObjectSO : ScriptableObject
    {
        public float health;
        public int spriteIndex;
        public ObjectType type;

        public override string ToString()
        {
            return name;
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