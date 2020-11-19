using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
    public class HeadCheatManagement : MonoBehaviour
    {
        public GameObject blinder;
        //public SteamVR_Fade fader;

        private void OnTriggerEnter(Collider other)
        {
            blinder.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            blinder.SetActive(false);
        }
    }
}

