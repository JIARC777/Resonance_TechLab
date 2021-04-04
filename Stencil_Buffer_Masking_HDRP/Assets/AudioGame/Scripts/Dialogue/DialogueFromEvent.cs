using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueFromEvent : DialogueTrigger
{ 
    public void FireDialogue()
    {
        dialogueManager.QueueDialogue(thisClip, delayBeforeStarting, clipPriority);
        if (destroyOnPlay)
            Destroy(this);
    
    }
}
