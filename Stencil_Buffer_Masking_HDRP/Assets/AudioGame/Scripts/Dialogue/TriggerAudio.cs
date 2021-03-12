using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudio : MonoBehaviour
{
    //The canvas on which the dialoge elements will go
    public GameObject uiCanvas;
    public string speakerName;
    //The template UI prefab that has picture, name, isActive light
    public GameObject characterTemplate;

    //Must live on this gameObject already
    AudioSource audioClipToPlay;
    float lengthOfClip;
    float startPlayTime = -1f;
    //The acutal instance of characterTemplate to be instantiated
    GameObject createdTemplate;

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
                //play the audio and enable the base canvas
                audioClipToPlay.Play();
                startPlayTime = Time.time;
                uiCanvas.SetActive(true);

                //create and configure the specific speaker template here
                createdTemplate = Instantiate(characterTemplate, uiCanvas.transform);
                createdTemplate.GetComponent<SpeakerTemplate>().name.text = speakerName;
            }
        }
    }

    private void Update()
    {
        //if startPlayTime has been set in game and the clip has completed
        if (startPlayTime != -1f && Time.time > startPlayTime + lengthOfClip)
        {
            //disable the base canvas, destroy the template and the triggerbox
            uiCanvas.SetActive(false);
            Destroy(createdTemplate);
            Destroy(gameObject);
        }
    }
}
