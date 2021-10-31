using UnityEngine;

namespace Namestation.Player
{
    public class PlayerComponents : MonoBehaviour
    {
        #region Singleton
        public static PlayerComponents instance;
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("More than 1 instance of playercomponents found!");
                return;
            }
            instance = this;
        }
        #endregion

        public CameraManager cameraManager;
        public InputManager inputManager;
        public InteractionManager interactionManager;
    }

}
