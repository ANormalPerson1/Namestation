using Mirror;
using System;
using UnityEngine;

namespace Namestation.Grids
{
    [Serializable]
    public class Toilet : GridObject
    {
        [SyncVar] public float pissAmount;
    }
}

