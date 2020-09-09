using System.Collections;
using System.Collections.Generic;
using Multiplayer.Networking;
using UnityEngine;

namespace Multiplayer.Gameplay
{
    [RequireComponent(typeof(WhoActivateMe))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class ColliisionDestory : MonoBehaviour
    {
        [SerializeField]
        private NetworkIdentity networkIdentity;
        [SerializeField]
        private WhoActivateMe whoActivateMe;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {

        }
        /// <summary>
        /// Sent when an incoming collider makes contact with this object's
        /// collider (2D physics only).
        /// </summary>
        /// <param name="other">The Collision2D data associated with this collision.</param>
        void OnCollisionEnter2D(Collision2D other)
        {
            NetworkIdentity ni = other.gameObject.GetComponent<NetworkIdentity>();
            if (ni == null || ni.GetID() != whoActivateMe.GetActivator())
            {
                networkIdentity.GetSocket().Emit("collisionDestory", new JSONObject(JsonUtility.ToJson(new IDdata()
                {
                    id = networkIdentity.GetID()
                })));

            }
        }

    }
}
