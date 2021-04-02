using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroVideo : MonoBehaviour
{
    public IntroManager introManager;
    VideoPlayer player;
    float videoLength = 69f;
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<VideoPlayer>();
        player.Play();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime + videoLength)
        {
            introManager.EndMovie(0);
        }
        
    }
}
