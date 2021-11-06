using UnityEngine;
using Mirror;

namespace Namestation.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        public CameraManager cameraManager;
        public InputManager inputManager;
        public InteractionManager interactionManager;
        public MovementManager movementManager;

        public static PlayerManager instance;

        private void Start()
        {
            if (!isLocalPlayer) return;

            if (instance != null)
            {
                Debug.LogError("More than 1 instance of playercomponents found!");
                return;
            }

            instance = this;

            PlayerComponent[] playerComponents = GetComponents<PlayerComponent>();
            foreach(PlayerComponent playerComponent in playerComponents)
            {
                playerComponent.playerManager = this;
                playerComponent.Initialize();
            }
        }
    }
}
