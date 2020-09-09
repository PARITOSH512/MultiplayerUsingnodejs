using Multiplayer.Utilies.Attribute;
using UnityEngine;
using System;
using Multiplayer.Utilies;
using Multiplayer.Playermanager;

namespace Multiplayer.Networking
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkingRotation : MonoBehaviour
    {

        [Header("Reference Values")]
        [SerializeField]
        [GreyOut]
        private float oldTankRotation;

        [SerializeField]
        [GreyOut]
        private float oldBarrelRotation;

        [Header("Class reference")]
        [SerializeField]
        private PlayerManager playerManager;
        private NetworkIdentity networkIdentity;
        private PlayerRotation Player;
        private float Stillcounter = 0;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            playerManager = GetComponent<PlayerManager>();
            Player = new PlayerRotation();
            Player.tankRotation = 0;
            Player.barrelRotation = 0;

            if (!networkIdentity.IsControlling())
            {
                enabled = false;
            }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (networkIdentity.IsControlling())
            {
                if (oldTankRotation != transform.localEulerAngles.z || oldBarrelRotation != playerManager.GetLastRotation())
                {
                    oldTankRotation = transform.localEulerAngles.z;
                    oldBarrelRotation = playerManager.GetLastRotation();
                    Stillcounter = 0;
                    Senddata();
                }
                else
                {
                    Stillcounter += Time.deltaTime;
                    if (Stillcounter >= 1.0f)
                    {
                        Stillcounter = 0;
                        Senddata();
                    }
                }
            }
        }

        private void Senddata()
        {
            // Player.tankRotation = transform.localEulerAngles.z.TwoDecimals();
            // Player.barrelRotation = playerManager.GetLastRotation().TwoDecimals();
            Player.tankRotation = Mathf.Round(transform.localEulerAngles.z * 1000.0f) / 1000.0f;
            Player.barrelRotation = Mathf.Round(playerManager.GetLastRotation() * 1000.0f) / 1000.0f;
            networkIdentity.GetSocket().Emit("updaterotation", new JSONObject(JsonUtility.ToJson(Player)));
            // print("-=-=-=-=>" + new JSONObject(JsonUtility.ToJson(Player)));

        }
    }
}