using UnityEngine;

namespace Namestation.Grids
{
    public class GridObject : MonoBehaviour
    {
        public GridObjectSO gridObjectSO;
        [HideInInspector] public Vector2Int position;
        [HideInInspector] public float currentHealth;
    }
}