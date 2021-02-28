using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAMTTrigger : MonoBehaviour
{
    // Event thats called when audio multi-tool impacts the weak spot in the back of the rover 
    public delegate void OnAMTHit();

    public event OnAMTHit amtHitWeakSpot;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
      if (other.tag == "Beam")
      {
            Debug.Log("Weak Spot Hit");
            amtHitWeakSpot();
      }
    }
}
