using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDavePhysicsImpact : MonoBehaviour
{
    public delegate void OnImpact();
    
    public event OnImpact SomethingHitDAVE;
    // We want to have this script on the DAVE model itself to communicate with the collider and call an event when it's been hit

    private void OnCollisionEnter(Collision other)
    {
        // Check if its projectile or physics object, call event
        //if (other.gameObject.tag == "Projectile" || other.gameObject.tag == "PhysicsObject") {
        
        // for some reason this collides with the floor - no big deal once we nail down the tags to filter
        Debug.Log("Something Hit Dave");
        Debug.Log(other.gameObject.name);
        SomethingHitDAVE();
            
       // }
    }
}
