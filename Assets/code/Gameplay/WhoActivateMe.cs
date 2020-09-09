using System;
using Multiplayer.Utilies.Attribute;
using UnityEngine;
namespace Multiplayer.Gameplay
{
    public class WhoActivateMe : MonoBehaviour
    {
        [SerializeField]
        [GreyOut]
        private string whoActivateMe;

        public void SetActivator(string ID)
        {
            whoActivateMe = ID;
        }
        public string GetActivator()
        {
            return whoActivateMe;
        }
    }
}