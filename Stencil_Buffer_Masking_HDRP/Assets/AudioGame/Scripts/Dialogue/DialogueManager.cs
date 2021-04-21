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

    [Header("The clip to play when the terminal is activated")]
    public VideoClip levelEndClip;

    private List<ResonanceVideo> clipsToPlay = new List<ResonanceVideo>();
    private ResonanceVideo videoCurrentlyPlaying;

    //Play the level load clip, if there is one
    void Start()
    {
        if (!player)
        {
            player = GameObject.Find("DialogueVideoPlayer").GetComponent<VideoPlayer>();
        }
        if (levelLoadClip)
        {
            ResonanceVideo startVideo = new ResonanceVideo(levelLoadClip, levelLoadClipDelay, 1);
            AddToList(startVideo);
            player.loopPointReached += PlayNext;
        }
    }

    //Handles adding and playing by priority
    void AddToList(ResonanceVideo video)
    {
        clipsToPlay.Add(video);
        //After adding clip, play if there is nothing playing
        if (!player.isPlaying)
        {
            PlayNext();
        }
        //If something with a higher priority comes in, play it now
        else if (video.priority > videoCurrentlyPlaying.priority)
        {
            //We need to set the delay to 0 if this clip is to play over a different one
            clipsToPlay[0].delayBeforePlaying = 0f;
            PlayNext();
        }
    }

    void PlayNext(UnityEngine.Video.VideoPlayer vp = null)
    {
        if (clipsToPlay.Count == 0)
        {
            videoCurrentlyPlaying = null;
            return;
        }
           

        IEnumerator playVideo = PlayClip(clipsToPlay[0]);
        StartCoroutine(playVideo);
        clipsToPlay.RemoveAt(0);
    }

    public void QueueDialogue(VideoClip newClip, float delayTime, float priority)
    {
        ResonanceVideo newVideo = new ResonanceVideo(newClip, delayTime, priority);
        AddToList(newVideo);
    }

    IEnumerator PlayClip(ResonanceVideo videoToPlay)
    {
        player.clip = videoToPlay.clip;
        videoCurrentlyPlaying = videoToPlay;
        yield return new WaitForSeconds(videoToPlay.delayBeforePlaying);
        player.Play();
    }

    public void PlayEndClip()
    {
        QueueDialogue(levelEndClip, 0f, 5f);
    }
}

public class ResonanceVideo
{
    public VideoClip clip;
    public float priority;
    public float delayBeforePlaying;

    public ResonanceVideo(VideoClip newClip, float delay, float newPriority)
    {
        clip = newClip;
        delayBeforePlaying = delay;
        priority = newPriority;   
    }
}

