using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload_3DR_Spool : MonoBehaviour
{

    //When a spool is placed within the 3DR's Spool_Loading trigger, the 3DR's internal spool is replenished, along with its charges
    public void OnTriggerEnter(Collider other)
    {
        //If the tag of the collider's gameobject is of the "Spool" variety, call the NewSpool() function within the Fire_3DR script, then destroy the collider's gameobject
        if (other.tag == "Spool")
        {
            Fire_3DR.threeDRInstance.NewSpool();
            Destroy(other.gameObject);
        }
    }
}
