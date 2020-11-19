﻿using System.Collections;
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
            for (int i = 0; i < gunParts.Length; i++)
                gunParts[i].layer = 11;
        }

        public void DetachedFromHand()
        {
            isAttachedToHand = false;
            for (int i = 0; i < gunParts.Length; i++)
                gunParts[i].layer = 10;
        }
    }
}

