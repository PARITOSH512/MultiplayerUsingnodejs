using System;
using System.Collections;
using System.Collections.Generic;
using Multiplayer.Networking;
using Multiplayer.Utilies;
using UnityEngine;

namespace Multiplayer.Playermanager
{
    public class PlayerManager : MonoBehaviour
    {
        const float BARREL_PIVOT_OFFSET = 90.0f;

        [Header("Data")]
        [SerializeField] private float speed = 2;
        [SerializeField] private float rotated = 60; // rotation speed of the tank body ,not the barrel

        [Header("Object Reference")]
        [SerializeField] private Transform barrelPivot;
        [SerializeField] private Transform BulletSpawnPoint;

        [Header("Class Reference")]
        [SerializeField]
        NetworkIdentity networkIdentity;
        private float lastRotation;// this is to handle the updating version of the rotating in networking 
        //Shooting data / shooting related stuff;
        private BulletData bulletData;
        private CoolDown ShootingCooldown; // this the cooldown class with will help us to set up the cooldown processs

        // Update is called once per frame

        private void Start()
        {
            ShootingCooldown = new CoolDown(1);
            bulletData = new BulletData();
            bulletData.position = new Position();
            bulletData.Direction = new Position();
        }
        void Update()
        {
            if (networkIdentity.IsControlling())
            {
                checkmovement();
                checkAiming();
                checkShooting();
            }
        }

        private void checkShooting()
        {
            ShootingCooldown.CoolDownUpdate();

            if (Input.GetMouseButton(0) && !ShootingCooldown.IsOnCooldown())
            {
                ShootingCooldown.StartCooldown();
                //Define the Bullet
                bulletData.activator = Networkclient.ClientID;
                bulletData.position.x = BulletSpawnPoint.position.x.TwoDecimals();
                bulletData.position.y = BulletSpawnPoint.position.y.TwoDecimals();
                bulletData.position.z = BulletSpawnPoint.position.z.TwoDecimals();

                bulletData.Direction.x = BulletSpawnPoint.up.x.TwoDecimals();
                bulletData.Direction.y = BulletSpawnPoint.up.y.TwoDecimals();
                bulletData.Direction.z = 0;
                //Send the Bullet
                networkIdentity.GetSocket().Emit("fireBullet", new JSONObject(JsonUtility.ToJson(bulletData)));
                // print(new JSONObject(JsonUtility.ToJson(bulletData)));
            }
        }


        // We required the "GetLastRotation()" and "SetRotation()" for the networking part of the game
        // to send the data and to change the other client gameobject values
        public float GetLastRotation()
        {
            return lastRotation;
        }
        public void SetRotation(float value)
        {
            barrelPivot.rotation = Quaternion.Euler(0, 0, value + BARREL_PIVOT_OFFSET);
        }

        private void checkmovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            // here we set the "vertical axis to move the body of the tank up and down"
            // so according graphic is facing downing to so we will inverse the input thats way we use "-" before "transfrom.up"
            transform.position += -transform.up * vertical * speed * Time.deltaTime;

            //here we will use horizontal axis to rotated;
            // so according graphic is facing downing to so we will inverse the input thats way we use "-" before "horizontal * rotated * Time.deltaTime"
            transform.Rotate(0, 0, -horizontal * rotated * Time.deltaTime);
        }

        private void checkAiming()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            direction.Normalize();
            float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            lastRotation = rot;

            barrelPivot.rotation = Quaternion.Euler(0, 0, rot + BARREL_PIVOT_OFFSET);
        }
    }
}
