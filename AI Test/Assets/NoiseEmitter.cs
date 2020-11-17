using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this is a data container that is passed to the enemy class on collision with an enemy
public class Noise
{
    public float volume;
    public Vector3 emittedLocation;
    public int noiseID;

    public Noise(Vector3 emitterLoc, float particleSize, float volumeFactor, int noiseID)
    {
        //This is the allgorithm to determine exactly how "loud" something is considered
        volume = particleSize * volumeFactor;
        emittedLocation = emitterLoc;
        this.noiseID = noiseID; 

    }
}
public class NoiseEmitter : MonoBehaviour
{
    // Factor that lifetime is cut by if a particle impacts a door
    float doorInhibitor; 
    //Decide Max distance to travel
    public float travelDistance = 10f;
    //Set speed so that It can re-time the system based on distance; 
    public float travelSpeed = 10f;
    // How Size is normalized - convert to volume units (TBD) based on modifier
    public float volumeModifier = 2f;
    int numParticles = 1000;
    float lifeTime;
    // Just in case we want some nicer searching or indexing - Gets passed into the noise as noiseID
    int emitterID;
    // holds the public variable the Enemy will access 
    public Noise noise; 
    // Reference to emitter
    ParticleSystem pSystem;
    public float currentMagnitude;
    

    void Start()
    {

        pSystem = this.GetComponent<ParticleSystem>();
        pSystem.Emit(numParticles);
        emitterID = Random.Range(0, 100);
        var main = pSystem.main;
        main.startSpeed = travelSpeed;
        main.startColor = Random.ColorHSV() + new Color(0.5f,0.5f,0.5f);
        // particle lifetime determined by distance/speed
        lifeTime = travelDistance / travelSpeed;
        main.startLifetime = lifeTime;

       // pSystem.Stop();
        
        
        
    }
    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Drone")
        {
           // Debug.Log("Collision Detected");
            // doesnt really matter which particles reach it, if a drone is detected by a particle, populate the noise field so that the drone can access it
            if (noise == null)
                noise = new Noise(this.transform.position, currentMagnitude, volumeModifier, emitterID);
        } 
    }

    private void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> exitDoor = new List<ParticleSystem.Particle>();
        int numParticles = pSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exitDoor);
        for (int i = 0; i < numParticles; i++)
        {
            ParticleSystem.Particle p = exitDoor[i];
            p.startLifetime = lifeTime / 2;
            exitDoor[i] = p;
        }
    }
    // Update is called once per frame
    void Update()
    {

        /*
        emitterID = Random.Range(0, 100);
        var main = pSystem.main;
        main.startSpeed = travelSpeed;
        // particle lifetime determined by distance/speed
        lifeTime = travelDistance / travelSpeed;
        main.startLifetime = lifeTime;
        */

        if (pSystem.time >= lifeTime)
            Destroy(this.gameObject);
           // pSystem.time = 0;
        currentMagnitude = 1 - (pSystem.time / lifeTime);
    }
    
}
