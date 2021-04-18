using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [Header("The VideoPlayer for the player")]
    public VideoPlayer player;

    [Header("The one dialogue manager in the scene")]
    public DialogueManager dialogueManager;

    [Header("For this dialouge trigger")]
    public VideoClip thisClip;
    public float delayBeforeStarting = 0f; //after hitting the trigger, wait for this time
    public float clipPriority;
    public bool destroyOnPlay = true;

    public UnityEvent onEnteredTriggerEvent;

    //[Header("If this dialogue should just string into another")]
    //public DialogueTrigger_v2 nextDialogue;
    //public float delayBeforeNextDialogue = 0f;

    private void Start()
    {
        if (!player)
            player = GameObject.Find("DialogueVideoPlayer").GetComponent<VideoPlayer>();
        if (!dialogueManager)
            dialogueManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            onEnteredTriggerEvent.Invoke();
            dialogueManager.QueueDialogue(thisClip, delayBeforeStarting, clipPriority);
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
