using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Namestation.Player
{
    public class CameraManager : PlayerComponent
    {
        public Camera playerCamera;

        protected override void InitializeNotLocalPlayer()
        {
            base.InitializeNotLocalPlayer();
            Debug.Log("Wryyy");
            Destroy(playerCamera.gameObject);
        }
    }
}
