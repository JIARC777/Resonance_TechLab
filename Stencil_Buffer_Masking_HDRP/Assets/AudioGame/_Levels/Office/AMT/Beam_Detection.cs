using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam_Detection : MonoBehaviour
{
    //This function scans to see if the beam or beamDetect gameObjects detect and Daves
    public void OnTriggerEnter(Collider other)
    {
        //If the tag of the colliding object is "Dave", run it's deactivate function
        if(other.tag == "Dave")
        {
            Debug.Log("Do something to him");
        }
    }
}
