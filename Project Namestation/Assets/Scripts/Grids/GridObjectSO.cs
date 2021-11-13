using UnityEngine;
using System;

namespace Namestation.Grids
{
    [CreateAssetMenu(menuName = "Namestation/GridObject")]
    public class GridObjectSO : ScriptableObject
    {
        public string gridObjectName;
        public float health;
        public int spriteIndex;
        public ObjectType type;

        public override string ToString()
        {
            return gridObjectName;
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