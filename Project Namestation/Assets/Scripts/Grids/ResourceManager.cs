using UnityEngine;
using System.IO;

namespace Namestation.Grids
{
    public static class ResourceManager
    {
        private static string gridPrefabPath = "GridPrefabs";
        private static string spritePath = "Sprites";

        public static GameObject GetGridPrefab(string name)
        {
            return Resources.Load<GameObject>(Path.Combine(gridPrefabPath, name));
        }

        public static Sprite GetSprite(string name)
        {
            return Resources.Load<Sprite>(Path.Combine(spritePath, name));
        }
    }
}

