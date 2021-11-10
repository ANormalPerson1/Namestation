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

        private void Start()
        {
            PlayerComponent[] playerComponents = GetComponents<PlayerComponent>();
            foreach(PlayerComponent playerComponent in playerComponents)
            {
                playerComponent.playerManager = this;
                playerComponent.Initialize();
            }
        }
    }
}
