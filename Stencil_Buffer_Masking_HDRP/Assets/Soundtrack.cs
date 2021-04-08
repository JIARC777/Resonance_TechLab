using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundtrack : MonoBehaviour
{
    AudioSource source;
    float timeToPlayNext;
    bool updateTime = true;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying && updateTime)
		{
            timeToPlayNext = Time.time + Random.Range(100, 200);
            updateTime = false;
		}
        if (Time.time >= timeToPlayNext && !source.isPlaying)
		{
            source.Play();
            updateTime = true;
		}
    }
}
