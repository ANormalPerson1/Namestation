using System.Collections.Generic;
using UnityEngine;

namespace Namestation.Interactables
{
    public class BuildableCollection : MonoBehaviour
    {
        public List<GameObject> buildables;

        public static BuildableCollection instance;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("More than 1 instance of BuildableCollection found!");
                return;
            }
            instance = this;
        }
    }
}
