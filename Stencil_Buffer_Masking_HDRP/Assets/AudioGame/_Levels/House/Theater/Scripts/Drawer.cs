using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class Drawer : MonoBehaviour
    {
        public float openRate = 1f;
        LinearDrive drawerDriver;
        public bool isOpening = false;

        void Start()
        {
            drawerDriver = GetComponent<LinearDrive>();
        }

        private void Update()
        {
            if (isOpening)
            {
                transform.position += Vector3.left * openRate * Time.deltaTime;
                drawerDriver.linearMapping.value += openRate * Time.deltaTime;
                if (drawerDriver.linearMapping.value > 0.123f)
                {
                    isOpening = false;
                    drawerDriver.linearMapping.value = 1f;
                    Destroy(this);
                }
            }
            
            
        }
        public void StartOpening()
        {
            isOpening = true;
        }
    }
}


