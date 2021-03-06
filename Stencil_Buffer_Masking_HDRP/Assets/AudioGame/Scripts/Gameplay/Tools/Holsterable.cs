using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holsterable : MonoBehaviour
{
    //Is the weapon currently sitting in a holster?
    bool isHolstered = false;
    //Is the weapon currently in a holster triggerBox?
    bool isInHolsterTrigger = false;
    //The holster the weapon is currently intersecting with
    ShoulderHolster currentHolster;

    //When the weapon enters a triggerbox
    private void OnTriggerEnter(Collider other)
    {
        //If the triggerbox is a holster
        if (other.tag == "Holster")
        {
            isInHolsterTrigger = true;
            currentHolster = other.GetComponent<ShoulderHolster>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Holster")
        {
            isInHolsterTrigger = false;
            currentHolster = null;
        }
    }

    //Called every time the weapon is dropped from Throwable event
    public void OnHolsterDrop()
    {
        //Holster the weapon when it is dropped in the holster box
        if (isInHolsterTrigger && !currentHolster.isFull)
        {
            isHolstered = true;
            Debug.Log("Holstered " + name);

            //Tell holster it is now full
            currentHolster.isFull = true;

            //Turn it invisible, expensive the way it is but dependent on how the models are set up
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].enabled = false;

            //Set its parent to the holster
            transform.SetParent(currentHolster.transform);
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
            
    }

    //Called every time the weapon is picked up from Throwable event
    public void OnHolsterablePickup()
    {
        //If the weapon is being picked up from the holster
        if (isHolstered)
        {
            isHolstered = false;

            //tell holster it is empty
            currentHolster.isFull = false;

            //Turn it visible again, this is bad copied code
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].enabled = true;


            // May need this line, depending on how VR Interactable flags are set
            //GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
