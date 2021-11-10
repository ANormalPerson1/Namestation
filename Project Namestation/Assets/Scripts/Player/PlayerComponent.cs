using UnityEngine;
using Namestation.Player;
using Mirror;

public class PlayerComponent : NetworkBehaviour
{
    public PlayerManager playerManager;
    protected bool initialized = false;

    public virtual void Initialize()
    {
        if (!isLocalPlayer)
        {
            InitializeNotLocalPlayer();
            return;
        }
        initialized = true;
    }

    protected virtual void InitializeNotLocalPlayer()
    {

    }
}
