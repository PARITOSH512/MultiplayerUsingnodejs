using System;
using Multiplayer.Utilies.Attribute;
using UnityEngine;

namespace Multiplayer.Networking
{
    // This class will send are player transfrom position
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkingTranfrom : MonoBehaviour
    {
        [SerializeField]
        [GreyOut]
        private Vector3 OldPostion;
        private NetworkIdentity networkIdentity;
        private float StillCounter = 0;
        private Player player;

        void Start()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            OldPostion = transform.position;
            player = new Player();
            player.id = Networkclient.ClientID;
            player.position = new Position();
            player.position.x = 0;
            player.position.y = 0;
            player.position.z = 0;

            if (!networkIdentity.IsControlling())
            {
                enabled = false;
            }

        }
        private void Update()
        {
            if (networkIdentity.IsControlling())
            {
                if (OldPostion != transform.position)
                {
                    OldPostion = transform.position;
                    StillCounter = 0;
                    sendData();
                }
                else
                {
                    StillCounter += Time.deltaTime;
                    if (StillCounter >= 1)
                    {
                        StillCounter = 0;
                        sendData();
                    }
                }
            }
        }

        private void sendData()
        {
            player.position.x = Mathf.Round(transform.position.x * 1000.0f) / 1000.0f;
            player.position.y = Mathf.Round(transform.position.y * 1000.0f) / 1000.0f;
            player.position.z = Mathf.Round(transform.position.z * 1000.0f) / 1000.0f;
            //here we perserve the 3decimal point by rounding of by multiplying
            //trasfrom by 1000. and divide by 1000
            networkIdentity.GetSocket().Emit("updatepostion", new JSONObject(JsonUtility.ToJson(player)));
            // print("-=-=-=-=>" + new JSONObject(JsonUtility.ToJson(player)));
        }
    }
}