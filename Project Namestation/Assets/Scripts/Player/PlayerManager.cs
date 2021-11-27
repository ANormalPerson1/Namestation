using UnityEngine;
using Mirror;

namespace Namestation.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager localPlayerManager;

        public CameraManager cameraManager;
        public InputManager inputManager;
        public BuildingManager buildingManager;
        public MovementManager movementManager;
        public SoundManager soundManager;

        private void Start()
        {
            if(isLocalPlayer)
            {
                if (localPlayerManager != null)
                {
                    Debug.LogError("More than 1 instance of PlayerManager found!");
                    return;
                }
                localPlayerManager = this;
            }
           

            PlayerComponent[] playerComponents = GetComponents<PlayerComponent>();
            foreach(PlayerComponent playerComponent in playerComponents)
            {
                playerComponent.playerManager = this;
                playerComponent.Initialize();
            }
        }
    }
}
