using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Plug : MonoBehaviour
{
    public float videoDelay = 0f;
    public VideoClip plugInVideo;
    public VideoPlayer discordPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Plug")
        {
            discordPlayer.clip = plugInVideo;
            discordPlayer.Play();
        }
    }
}
