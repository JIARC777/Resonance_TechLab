using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFirstBumpDialogue : DialogueTrigger
{
    private void OnTriggerEnter(Collider other)
    {
        return;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("Hit tag is: " + hit.gameObject.tag);
        if (hit.gameObject.tag == "PhysicsObj" || hit.gameObject.tag == "Map Hazard")
        {
            dialogueManager.QueueDialogue(thisClip, delayBeforeStarting, clipPriority);
            if (destroyOnPlay)
                Destroy(this);
        }
        
    }
}
