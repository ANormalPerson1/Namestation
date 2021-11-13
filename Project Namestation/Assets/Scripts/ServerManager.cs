using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Namestation.Saving;

namespace Namestation.Server
{
    public class ServerManager : NetworkBehaviour
    {
        private void Start()
        {
            if (!isServer) return;
            SaveManager.Load();
        }

        private void OnApplicationQuit()
        {
            SaveManager.Save();
        }
    }
}

