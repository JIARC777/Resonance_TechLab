using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class Gun : MonoBehaviour
    {
        [Header("Custom Gun variables")]
        public SteamVR_Action_Boolean triggerPull;
        public ParticleSystem particleSystem;
        public GameObject[] gunParts;

        private bool isAttachedToHand = false;


        //DOTS
        private void Awake()
        {
        }

        //\\DOTS
        public void Fire()
        {
            if (isAttachedToHand)
                particleSystem.Play();
        }

        public void StopFiring()
        {

            particleSystem.Stop();
        }

        public void AttachedToHand()
        {
            isAttachedToHand = true;
            foreach(GameObject go in gunParts)
            {
                go.layer = 13;
                go.GetComponent<Collider>().enabled = false;
            }
        }

        public void DetachedFromHand()
        {
            isAttachedToHand = false;
            foreach (GameObject go in gunParts)
            {
                go.layer = 10;
                go.GetComponent<Collider>().enabled = true;
            }
        }
    }
}
