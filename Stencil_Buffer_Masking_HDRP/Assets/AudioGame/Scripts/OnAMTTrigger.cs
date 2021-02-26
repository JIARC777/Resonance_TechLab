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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
       // if (other.tag == "AMT_Beam")
      //  {
            Debug.Log("Weak Spot Hit");
            amtHitWeakSpot();
      //  }
    }
}
