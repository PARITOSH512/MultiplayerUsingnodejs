using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer.Utilies
{
    // this class is use to setup the cooldown in the game;
    public class CoolDown
    {
        private float lenght;
        private float currenttime;
        private bool onCooldown;

        public CoolDown(float Lenght = 1, bool StartWithCoolDown = false)
        {
            currenttime = 0;
            lenght = Lenght;
            onCooldown = StartWithCoolDown;
        }
        // Hence it is not MonoBehaviour class so we can access update method 
        // So we have to write our own update method ie CoolDownUpdate Method
        public void CoolDownUpdate()
        {
            if (onCooldown)
            {
                currenttime += Time.deltaTime;
                if (currenttime >= lenght)
                {
                    currenttime = 0;
                    onCooldown = false;
                }
            }
        }

        public bool IsOnCooldown()
        {
            return onCooldown;
        }
        public void StartCooldown()
        {
            onCooldown = true;
            currenttime = 0;
        }
    }
}
