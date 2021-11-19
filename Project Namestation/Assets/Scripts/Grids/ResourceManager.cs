using UnityEngine;
using System.IO;

namespace Namestation.Grids
{
    public static class ResourceManager
    {
        private static string gridPrefabPath = "GridPrefabs";

        public static GameObject GetGridPrefab(string name)
        {
            return Resources.Load<GameObject>(Path.Combine(gridPrefabPath, name));
        }
    }
}

