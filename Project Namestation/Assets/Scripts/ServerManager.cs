using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Namestation.Saving;

namespace Namestation.Server
{
    public class ServerManager : NetworkBehaviour
    {
        private void Update()
        {
            if(Input.GetKey(KeyCode.Keypad5))
            {
                ServerSaveAndQuitGame();
            }
        }

        [Command]
        private void ServerSaveAndQuitGame()
        {
            Debug.Log("Saving game...");
            StartCoroutine(IE_Save());
        }

        IEnumerator IE_Save()
        {
            SaveManager.Save();
            while(SaveManager.isSaving)
            {
                yield return new WaitForEndOfFrame();
            }
            Debug.Log("Finished saving!");
            Application.Quit();
        }
    }
}

