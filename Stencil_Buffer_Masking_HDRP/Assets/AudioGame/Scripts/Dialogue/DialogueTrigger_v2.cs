using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class DialogueTrigger_v2 : MonoBehaviour
{
    [Header("For this dialouge trigger")]
    public float delayBeforeStarting = 0f; //after hitting the trigger, wait for this time
    public bool destroyOnPlay = true;

    [Header("If this dialogue should just string into another")]
    public DialogueTrigger_v2 nextDialogue;
    public float delayBeforeNextDialogue = 0f;

    private VideoPlayer thisVideo;

    private void Start()
    {
        //TODO: Figure out where to put video player component, should it just switch video clips?
        thisVideo.loopPointReached += DialogueEnded;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Invoke("PlayDialogue", delayBeforeStarting);
            if (destroyOnPlay == true)
            {
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    public void PlayDialogue()
    {
        thisVideo.Play();
    }

    void DialogueEnded(UnityEngine.Video.VideoPlayer player)
    {
        if (nextDialogue)
        {
            Invoke("PlayNext", delayBeforeNextDialogue);
        }
    }

    void PlayNext()
    {
        Instantiate(nextDialogue.gameObject);
        if (destroyOnPlay == true)
        {
            Destroy(gameObject);
        }
    }
}
