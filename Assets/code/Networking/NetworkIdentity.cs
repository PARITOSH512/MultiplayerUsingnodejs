using Multiplayer.Utilies.Attribute;
using SocketIO;
using UnityEngine;

namespace Multiplayer.Networking
{   // this class lets that who we are
    public class NetworkIdentity : MonoBehaviour
    {
        [Header("Helpful Values")]
        [GreyOut]
        [SerializeField] private string id;
        [GreyOut]
        [SerializeField] private bool isControlling;
        private SocketIOComponent socket;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            isControlling = false;
        }
        public void SetControllerID(string ID)
        {
            id = ID;
            print("-=-=-=-=>" + gameObject.name + "-=-=Networkclient-=-=->" + Networkclient.ClientID);
            isControlling = (Networkclient.ClientID == ID) ? true : false; // Check incoming id versuses the one we have saved from the server
        }

        public void SetSocketReference(SocketIOComponent socket)
        {
            this.socket = socket;
        }

        public string GetID()
        {
            return id;
        }

        public bool IsControlling()
        {
            return isControlling;
        }

        public SocketIOComponent GetSocket()
        {
            return socket;
        }
    }
}