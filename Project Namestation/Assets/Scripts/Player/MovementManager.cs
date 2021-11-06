using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Namestation.Player
{
    public class MovementManager : PlayerComponent
    {
        protected override void Update()
        {
            base.Update();
            HandleMovement();
        }

        void HandleMovement()
        {
            if(isLocalPlayer)
            {
                Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
                transform.position += movement * Time.deltaTime * 5f;
            }
        }
    }
}

