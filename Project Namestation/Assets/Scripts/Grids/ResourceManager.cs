using UnityEngine;
using System.IO;

namespace Namestation.Grids
{
    public static class ResourceManager
    {
        private static string gridPrefabPath = "GridPrefabs";
        private static string gridScriptableObjectPath = "GridObjects";

        public static GameObject GetGridPrefab(string name)
        {
            return Resources.Load<GameObject>(Path.Combine(gridPrefabPath, name));
        }

        public static GridObjectSO GetGridObjectSO (string name)
        {
            return Resources.Load<GridObjectSO>(Path.Combine(gridScriptableObjectPath, name));
        }
    }
}

