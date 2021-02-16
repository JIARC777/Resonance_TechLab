using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Updated Version of the original "Noise" Class - Communicates data about active sound on collision with an listening AI controllers
public class ActiveSound: MonoBehaviour
{
    // Accessible data for inquiring AI
    [HideInInspector]
    public float curVolume;
    [HideInInspector]
    public Vector3 soundLocation;
    [HideInInspector]
    public int Id;

    public float speedFactorMultiplier = 5f;

    float lifeTime;
    float duration;
    float volume;
    float timeSoundOccured;
    int particleRes = 1000;
    
    ParticleSystem pSystem;

    //A pseudo-constructor for communicating data between the noisemaker and the active sound
    public void TransferVolumeData(float volume, float lifeTime, Vector3 soundLocation, float timeSoundOccured)
    {
        this.volume = volume;
        this.soundLocation = this.transform.position;
        this.lifeTime = lifeTime;
        this.timeSoundOccured = timeSoundOccured;
        Id = Mathf.RoundToInt(Random.Range(0, 1000000));
        pSystem = GetComponent<ParticleSystem>();
        EmitNoise();
    }

    public void EmitNoise()
	{
        Debug.Log("Active Sound Created");
        // This uses a new system for calculating volume based on the single default volume parameter. It uses the material loudness and echo parameters to modify either speed or lifetime
        // Louder volumes have an increased speed thus a higher chance of being quickly detected by nearby AI
        // sounds with an echo have a chance to travel further with a lower volume
        var main = pSystem.main;
        main.startLifetime = lifeTime;
        main.startSpeed = (volume * speedFactorMultiplier);
        curVolume = volume;
        pSystem.Emit(particleRes);

    }
    public void Update()
    {
        
        // calculate duration 
        duration = Time.time - timeSoundOccured;
        if (duration >= lifeTime)
            Destroy(this.gameObject);
        else
		{

            // We need a nicer equation for falloff - try to take into account echo versus volume
            curVolume *= 1 - (duration / lifeTime);
        }   
    }
}
