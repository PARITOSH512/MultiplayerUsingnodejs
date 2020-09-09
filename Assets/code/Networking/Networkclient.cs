using System.Collections;
using System.Collections.Generic;
using Multiplayer.Playermanager;
using UnityEngine;
using SocketIO;
using System;
using Multiplayer.ScriptableObjects;
using Multiplayer.Gameplay;
using Multiplayer.Utilies;

namespace Multiplayer.Networking
{
    public class Networkclient : SocketIOComponent
    {
        public const float SERVER_UPDATE_TIME = 10;

        [Header("Network Client")]
        [SerializeField]
        private Transform networkContainer;

        [SerializeField]
        private GameObject Swapingobject;

        // Give a reference of the scriptable object which ca be spwan by the Server
        [SerializeField]
        private ServerObject serverSpwanables;


        //This Very important , we need to acess it everywhere thus we making it static
        //We required this for network identifies
        public static string ClientID
        {
            get;
            private set;
        }
        public string Cliednt;
        private Dictionary<string, NetworkIdentity> serverObject;
        // This dictionary will keep the record of the player id with its spawned gameobject 
        // thats way our dictionary type is "string" which will be our playerId and Gameobject 
        // which will be our spwaned gameobject 

        public override void Start()
        {
            /* 
                base.Start(), what it will do that it will the base implementation of the method Start() from the Super/base
                class , like here it will "base.Start()" will implement the statement written in the SocketIOComponent which 
                is inhertiate by Networkclient

                but one thing is important is that in base class ,such method should be "virtual"  ex:- "public virtual Start()" 
            */
            base.Start();
            initialize();
            setupEvent();
        }

        private void initialize()
        {
            serverObject = new Dictionary<string, NetworkIdentity>();
        }

        private void setupEvent()
        {
            //Eventio is a callback give by the socket
            //Whatever event we are generate over the server all will be revicered our here

            /*
                Here On("Event name",(E)=>{}) will get the response from server to client 
                where as in Emit("event name",(e)=>{}) will send the response to the server from client

                Here event name in server and the script should be same 
            */
            On("open", (Eventio) =>
            {
                Debug.Log("Connection made to the server");
            });
            On("register", (Eventio) =>
            {
                string id = Eventio.data["id"].ToString().RemoveQuotes();
                Debug.LogFormat("Our Client's Id ({0})", id);

                ClientID = id; // setting the Client id of the player
                Cliednt = ClientID;
            });

            //This event will handle when the player spwan 
            On("spawn", (Eventio) =>
            {
                string id = Eventio.data["id"].ToString().RemoveQuotes();
                
                GameObject go;
                go = Instantiate(Swapingobject, Vector3.one, Quaternion.identity);
                go.name = "Service Id" + id;
                print("-=-=-=->" + go.name);
                NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
                ni.SetControllerID(id);
                ni.SetSocketReference(this);
                go.transform.SetParent(networkContainer);
                serverObject.Add(id, go.GetComponent<NetworkIdentity>());

            });

            // This event will update the position of the Player
            On("updatepostion", (Eventio) =>
            {
                string id = Eventio.data["id"].ToString().RemoveQuotes();
                float x = Eventio.data["position"]["x"].f;
                float y = Eventio.data["position"]["y"].f;
                float z = Eventio.data["position"]["z"].f;

                NetworkIdentity ni = serverObject[id];
                ni.transform.position = new Vector3(x, y, z);


            });

            // This event will update the rotation of the player
            On("updaterotation", (Eventio) =>
            {
                string id = Eventio.data["id"].ToString().RemoveQuotes();
                float tankRotation = Eventio.data["tankRotation"].f;
                float barrelRotation = Eventio.data["barrelRotation"].f;

                NetworkIdentity ni = serverObject[id];
                ni.transform.localEulerAngles = new Vector3(0, 0, tankRotation);
                ni.transform.GetComponent<PlayerManager>().SetRotation(barrelRotation);
            });

            On("serverSpawn", (Eventio) =>
            {
                string name = Eventio.data["name"].str;
                string id = Eventio.data["id"].ToString().RemoveQuotes();
                float x = Eventio.data["position"]["x"].f;
                float y = Eventio.data["position"]["y"].f;
                float z = Eventio.data["position"]["z"].f;
                Debug.LogFormat("Server want us to spawn a '{0}'", name);

                if (!serverObject.ContainsKey(id))
                {
                    ServerObjectData Sod = serverSpwanables.GetObjectByName(name);
                    var spawnobject = Instantiate(Sod.Prefabs, networkContainer);
                    spawnobject.transform.position = new Vector3(x, y, 0);

                    var ni = spawnobject.GetComponent<NetworkIdentity>();
                    ni.SetControllerID(id);
                    ni.SetSocketReference(this);

                    serverObject.Add(id, ni);
                    // If Bullet applt direction as well
                    // print("-=-=Bullet-=-=->");
                    if (name == "Bullet")
                    {

                        float directionX = Eventio.data["Direction"]["x"].f;
                        float directionY = Eventio.data["Direction"]["y"].f;
                        float directionZ = Eventio.data["Direction"]["z"].f;
                        string activator = Eventio.data["activator"].str;
                        float speed = Eventio.data["Speed"].f;

                        print("-=-=Bullet-=-=->");
                        float rot = Mathf.Atan2(directionY, directionX) * Mathf.Rad2Deg;
                        Vector3 currentRotation = new Vector3(0, 0, rot - 90);
                        print("currentRotation" + currentRotation+directionX);
                        spawnobject.transform.rotation = Quaternion.Euler(currentRotation);

                        WhoActivateMe whoActivateMe = spawnobject.transform.GetComponent<WhoActivateMe>();
                        whoActivateMe.SetActivator(activator);

                        Projectile projectile = spawnobject.GetComponent<Projectile>();
                        projectile.Direction = new Vector3(directionX, directionY, directionZ);
                        projectile.Speed = speed;
                    }

                }

            });

            On("serverUnSpawn", (Eventio) =>
            {
                string id = Eventio.data["id"].ToString().RemoveQuotes();
                NetworkIdentity ni = serverObject[id];
                serverObject.Remove(id);
                DestroyImmediate(ni.gameObject);

            });

            On("playerDied", (Eventio) =>
             {
                 print("-=-=PlayerDied-=-=->" + Eventio.data["id"].ToString());
                 string id = Eventio.data["id"].ToString().RemoveQuotes();
                 NetworkIdentity ni = serverObject[id];

                 ni.gameObject.SetActive(false);
             });

            On("playerRespawn", (Eventio) =>
             {
                 string id = Eventio.data["id"].ToString().RemoveQuotes();
                 NetworkIdentity ni = serverObject[id];

                 float x = Eventio.data["position"]["x"].f;
                 float y = Eventio.data["position"]["y"].f;
                 float z = Eventio.data["position"]["z"].f;

                 ni.transform.position = new Vector3(x, y, z);
                 ni.gameObject.SetActive(true);
             });

            On("disconnected", (Eventio) =>
            {
                print("-=-=-=-=->" + serverObject.Count);
                string id = Eventio.data["id"].ToString().RemoveQuotes();

                GameObject go = serverObject[id].gameObject;
                Destroy(go);
                serverObject.Remove(id);
                print("-=-=-=-=->" + serverObject.Count + " " + id);
            });
        }

        public void AttempttoJoinLobby()
        {
            Emit("joinGame");
        }
            

        public override void Update()
        {
            /* 
                base.Start(), what it will do that it will the base implementation of the method Start() from the Super/base
                class , like here it will "base.Update()" will implement the statement written in the SocketIOComponent which 
                is inhertiate by Networkclient

                but one thing is important is that in base class ,such method should be "virtual"  ex:- "public virtual Update()" 
           */
            base.Update();
        }
        public void disconnected()
        {
            Emit("disconnect");
        }
    }

    // this class are to send the data to the server
    [Serializable]
    public class Player
    {
        public string id;
        public Position position;
    }
    // this class are to send the tranfrom/position data of the player to the server    
    [Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;
    }
    [Serializable]
    public class PlayerRotation
    {
        public float tankRotation;
        public float barrelRotation;
    }
    [Serializable]
    public class BulletData
    {
        public string id;
        public Position position;
        public Position Direction;
        public string activator;
    }
    [Serializable]
    public class IDdata
    {
        public string id;

    }

}