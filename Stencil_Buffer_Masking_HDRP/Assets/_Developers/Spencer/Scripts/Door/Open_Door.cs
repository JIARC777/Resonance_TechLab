using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Open_Door : MonoBehaviour
{

    public bool needsButton;
    public bool buttonPressed;

    public bool activated;

    public Activate doorToActivate;

    public void OnTriggerEnter(Collider other)
    {
        if (!activated && other.tag == "Player")
        {
            if (!needsButton)
            {
                doorToActivate.Activation(true);
                activated = true;
            }
            else
            {
                if (buttonPressed)
                {
                    doorToActivate.Activation(true);
                    activated = true;
                }
            }
        }        
    }

    public void OnTriggerStay(Collider other)
    {
        if (!activated && other.tag == "Player")
        {
            if (!needsButton)
            {
                doorToActivate.Activation(true);
                activated = true;
            }
            else
            {
                if (buttonPressed)
                {
                    doorToActivate.Activation(true);
                    activated = true;
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "Dave" && other.tag == "Player")
        {
            doorToActivate.Activation(false);
        }
    }
}
