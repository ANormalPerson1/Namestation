using UnityEngine;
using Mirror;

namespace Namestation.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager localPlayerManager;

        public CameraManager cameraManager;
        public InputManager inputManager;
        public InteractionManager interactionManager;
        public MovementManager movementManager;
        public SoundManager soundManager;

        private void Start()
        {
            if (localPlayerManager != null)
            {
                Debug.LogError("More than 1 instance of PlayerManager found!");
                return;
            }
            localPlayerManager = this;

            PlayerComponent[] playerComponents = GetComponents<PlayerComponent>();
            foreach(PlayerComponent playerComponent in playerComponents)
            {
                playerComponent.playerManager = this;
                playerComponent.Initialize();
            }
        }
    }
}
