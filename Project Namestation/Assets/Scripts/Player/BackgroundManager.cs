using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Namestation.Player
{
    public class BackgroundManager : PlayerComponent
    {
        [SerializeField] Material backgroundMaterial;
        [SerializeField] GameObject background;
        CameraManager cameraManager;
        public override void Initialize()
        {
            base.Initialize();
            cameraManager = playerManager.cameraManager;
        }

        protected override void InitializeNotLocalPlayer()
        {
            base.InitializeNotLocalPlayer();
            Destroy(background);
        }

        private void Update()
        {
            if (isLocalPlayer && initialized)
            {
                Vector3 cameraPosition = cameraManager.playerCamera.transform.position;
                cameraPosition.z = 0f;
                backgroundMaterial.SetVector("_Offset", new Vector2(cameraPosition.x * 0.002f, cameraPosition.y * 0.002f));
            }
        }
    }
}

