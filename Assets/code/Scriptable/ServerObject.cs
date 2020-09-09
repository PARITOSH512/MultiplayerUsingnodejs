using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Multiplayer.ScriptableObjects
{
    //by using the Scriptable object the that data and the object can be serlizable in unity
    [CreateAssetMenu(fileName = "Server_Object", menuName = "Scriptable Objects/Server Objects", order = 3)]
    public class ServerObject : ScriptableObject
    {
        public List<ServerObjectData> Objects;

        public ServerObjectData GetObjectByName(string Name)
        {
            // here making a link to the similar object
            return Objects.SingleOrDefault(x => x.Name == Name);
        }
    }
    [Serializable]
    public class ServerObjectData
    {
        public string Name = "New Object";
        public GameObject Prefabs;
    }
}