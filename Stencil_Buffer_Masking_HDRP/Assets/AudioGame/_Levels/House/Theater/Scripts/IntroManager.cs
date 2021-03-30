using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public GameObject player;
    public GameObject teleportPos;
    public VideoPlayer introPlayer;
    public VideoPlayer discordPlayer;

    bool endMovieCalled = false;

    public void EndMovie()
    {
        if (!endMovieCalled)
        {
            introPlayer.Stop();
            player.transform.position = teleportPos.transform.position;
            endMovieCalled = true;
            Invoke("StartDiscordMovie", 4f);
        }  
    }

    void StartDiscordMovie()
    {
        discordPlayer.Play();
    }

}
