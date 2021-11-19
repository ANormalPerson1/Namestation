using Mirror;
using System;
using UnityEngine;

namespace Namestation.Grids
{
    [Serializable]
    public class Toilet : TileObject
    {
        [SyncVar] public float pissAmount;
    }
}

