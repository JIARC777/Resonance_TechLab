using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class DialogueManager : MonoBehaviour
{
    [Header("The player's dialogue video player")]
    public VideoPlayer player;

    [Header("The clip and delay to play on level load")]
    public VideoClip levelLoadClip;
    public float levelLoadClipDelay;

    private IEnumerator coroutine;

    //Play the level load clip, if there is one
    void Start()
    {
        if (levelLoadClip)
        {
            coroutine = PlayClip(levelLoadClip, levelLoadClipDelay);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator PlayClip(VideoClip newClip, float delayTime = 0f)
    {
        player.clip = newClip;
        yield return new WaitForSeconds(delayTime);
        player.Play();
    }
}
