using System.Collections;
using System.Collections.Generic;
using Multiplayer.Networking;
using UnityEngine;

namespace Multiplayer.Gameplay
{
    /* In this class we will similate the bullet velocity on client side
        because the taking update from server will 
     */
    public class Projectile : MonoBehaviour
    {
        private Vector3 direction;
        private float speed;

        public Vector3 Direction
        {
            set
            {
                //print("-=-=-direction" + value);
                direction = value;
            }
            get
            {
                return direction;
            }
        }

        public float Speed
        {
            set
            {
                //print("-=-=-speed" + speed);
                speed = value;
            }
            get
            {
                return speed;
            }
        }

        public void Update()
        {

            print("-=-=-direction" + direction +"-=0=-=-=-=speed-=-"+speed);
            Vector3 pos = direction * speed * Networkclient.SERVER_UPDATE_TIME *Time.deltaTime;
            transform.position += new Vector3(pos.x, pos.y, 0);
        }
    }
}
