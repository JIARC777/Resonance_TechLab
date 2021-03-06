using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload_3DR_Ammo : MonoBehaviour
{

    //When a projectile is placed within the 3DR's Breach trigger, the ammo is placed within the 3DR and is ready to be fired
    public void OnTriggerEnter(Collider other)
    {
        //If the tag of the collider's gameobject is of the "Ammo" variety, call the Reload() function within the Fire_3DR script
        if(other.tag == "Projectile" && !Fire_3DR.threeDRInstance.currentProjectile)
        {
            Fire_3DR.threeDRInstance.Reload(other.transform);
        }
    }
}
