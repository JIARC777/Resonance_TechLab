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

    }
}
public class NoiseEmitter : MonoBehaviour
{
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
    ParticleSystem.Particle[] particleList;
    public float currentSize;

  //  public List<ParticleCollisionEvent> collisionsWithEnemy;

    void Start()
    {
        pSystem = this.GetComponent<ParticleSystem>();
        pSystem.Emit(numParticles);
        pSystem.Stop();
        
        emitterID = Random.Range(0, 100);
        var main = pSystem.main;
        main.startSpeed = travelSpeed;
        // particle lifetime determined by distance/speed
        lifeTime = travelDistance / travelSpeed;
        main.startLifetime = lifeTime;
        //pSystem.GetParticles(particleList);
        // particle lifetime drives particle size based on graph set up in particle emitter
        // article size is used to determine normalized volume

        // Create list to store detectedCollisions
        // collisionsWithEnemy = new List<ParticleCollisionEvent>();

    }
    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Drone")
        {
           // Debug.Log("Collision Detected");
            // doesnt really matter which particles reach it, if a drone is detected by a particle, populate the noise field so that the drone can access it
            if (noise == null)
                noise = new Noise(this.transform.position, currentSize, volumeModifier, emitterID);
            
            //int numTimesCollided = pSystem.GetCollisionEvents(other, collisionsWithEnemy);
            //for (int i = 0; i < numTimesCollided; i++)
            //{
                //collisionsWithEnemy.ForEach
              //  NoiseParticle noise = new NoiseParticle(this.transform.position, currentSize, volumeModifier, emitterID);
                
             //   //Debug.Log("Collision Detected");
            //}
        }
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        // {
        //pSystem.Emit(numParticles);
        //pSystem.Stop();
        //pSystem.time = 0;
        //}
        //Debug.Log(pSystem.time);
        //Debug.Log(lifeTime);
        if (pSystem.time >= lifeTime -.01)
            Destroy(this.gameObject);
           // pSystem.time = 0;
        currentSize = 1 - (pSystem.time / lifeTime);
    }
    
}
