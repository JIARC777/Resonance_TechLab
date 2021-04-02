using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class IntroManager : MonoBehaviour
{
    public GameObject player;
    public GameObject teleportPos;
    public VideoPlayer introPlayer;
    public VideoPlayer discordPlayer;
    public UnityEvent drawerEvent;
    public float drawerOpenDelay = 48f;

    bool endMovieCalled = false;

    public void EndMovie(float time = 4f)
    {
        if (!endMovieCalled)
        {
            introPlayer.Stop();
            player.transform.position = teleportPos.transform.position;
            endMovieCalled = true;
            Invoke("StartDiscordMovie", time);
        }  
    }

    void StartDiscordMovie()
    {
        discordPlayer.Play();
        Invoke("OpenDrawer", drawerOpenDelay);
    }

    void OpenDrawer()
    {
        drawerEvent.Invoke();
    }

}
