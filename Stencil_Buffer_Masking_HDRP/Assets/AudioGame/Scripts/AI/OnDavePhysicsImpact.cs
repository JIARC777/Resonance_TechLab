using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDavePhysicsImpact : MonoBehaviour
{
    public delegate void OnImpact(bool hardImpact);
    
    public event OnImpact SomethingHitDAVE;
    // We want to have this script on the DAVE model itself to communicate with the collider and call an event when it's been hit

    private void OnCollisionEnter(Collision other)
    {
        /*
        // Check if its projectile thats hit the player
        if (other.gameObject.tag == "Ammo") {
             Debug.Log("Projectile hit DAVE");
            // Call the event, set hardImpact bool to true so that DAVE knows to deactivate himself
             SomethingHitDAVE(true);
        }
        if (other.gameObject.tag == "PhysicsObj")
		{
            Debug.Log("You threw something at DAVE");
            // Call the event, set hardImpact bool to false so that DAVE might just stops for a quick second 
            SomethingHitDAVE(false);
        }
        */
    }
}
