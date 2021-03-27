using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Activate : MonoBehaviour
{

    public bool pressed;

    public Open_Door doorActivate;

    public void OnTriggerEnter(Collider other)
    {
        if (!pressed)
        {
            doorActivate.buttonPressed = true;
            pressed = true;
        }        
    }
}
