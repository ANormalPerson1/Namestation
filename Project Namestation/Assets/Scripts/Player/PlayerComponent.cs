using UnityEngine;
using Namestation.Player;
using Mirror;

public class PlayerComponent : NetworkBehaviour
{
    public PlayerManager playerManager;
    protected bool initialized = false;

    protected virtual void Awake()
    {
        if (!isLocalPlayer || !initialized) return;
    }

    protected virtual void OnEnable()
    {
        if (!isLocalPlayer || !initialized) return;
    }

    protected virtual void Start()
    {
        if (!isLocalPlayer || !initialized) return;
    }

    public virtual void Initialize()
    {
        if (!isLocalPlayer)
        {
            InitializeNotLocalPlayer();
            return;
        }
    }

    protected virtual void InitializeNotLocalPlayer()
    {

    }

    protected virtual void Update()
    {
        if (!isLocalPlayer || !initialized) return;
    }

    protected virtual void FixedUpdate()
    {
        if (!isLocalPlayer || !initialized) return;
    }

    protected virtual void OnDisable()
    {
        if (!isLocalPlayer || !initialized) return;
    }
}
