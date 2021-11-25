using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Namestation.Player
{
    public class MovementManager : PlayerComponent
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        private void Update()
        {
            if (!initialized || !isLocalPlayer) return;
            HandleMovement();
        }

        void HandleMovement()
        {
            Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
            transform.position += movement * Time.deltaTime * 2.5f;
        }
    }
}

