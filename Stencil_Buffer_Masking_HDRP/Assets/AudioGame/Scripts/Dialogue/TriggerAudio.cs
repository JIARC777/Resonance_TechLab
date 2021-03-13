using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Speakers
{
    bios,
    hyperlynx,
    linerider
}

public class TriggerAudio : MonoBehaviour
{
    [Header("SET IN THIS ORDER: bios, hyperlynx, linerider")]
    public GameObject[] templates;
    public Speakers speakerName;
    //The template UI prefab that has picture, name, isActive light

    //Must live on this gameObject already
    AudioSource audioClipToPlay;
    float lengthOfClip;
    float startPlayTime = -1f;
    //The acutal instance of characterTemplate to be instantiated
    SpeakerTemplate createdTemplate;

    private void Start()
    {
        audioClipToPlay = GetComponent<AudioSource>();
        lengthOfClip = audioClipToPlay.clip.length;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the player enters the triggerbox
        if (other.gameObject.tag == "Player")
        {
            //don't play again if the player re-hits the triggerbox
            if (!audioClipToPlay.isPlaying)
            {
                //play the audio and enable the speaker's isActive icon
                audioClipToPlay.Play();
                startPlayTime = Time.time;
                createdTemplate = templates[(int)speakerName].GetComponent<SpeakerTemplate>();
                createdTemplate.toggleIndicator();
            }
        }
    }

    private void Update()
    {
        //if startPlayTime has been set in game and the clip has completed
        if (startPlayTime != -1f && Time.time > startPlayTime + lengthOfClip)
        {
            //disable the base canvas, destroy the template and the triggerbox
            createdTemplate.toggleIndicator();
            Destroy(gameObject);
        }
    }
}
