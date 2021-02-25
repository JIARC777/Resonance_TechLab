using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    // Baseline volume  - does not take material properties as defined in an Objects NoiseData into account so final volume will be higher
    [Tooltip("Default volume, can become louder after material property processing")]
    public float defaultVolumeBaseline = 5f;
    // echo factor and volume multiplier act as controls for the distance and speed sound travels - keep at one to inherit these properties from the object collided with
    [Tooltip("'Distance' multiplier of the sound travel. \nSet to '1' to inherit from collided object")]
    public float echoFactor = 1f;
    [Tooltip("General volume multiplier. \nSet to '1' to inherit from collided object")]
    public float volumeMultiplier = 1f;
    // Length of time before next OnCollision Call in the case of continued collision
    [Tooltip("Time to wait before registering another collision \n Helps stop spam if continually colliding")]
    public float impactCooldown = 1f;
    // by what factor we can modify the particle simulation speed to work with better magnitudes
    // Recording time object was hit
    float timeOfImpact = 0f;
    // control whether or not the object should consider the floor an audible collision
    [Tooltip("Whether this object is a PhysObj, or something like a wall/desk")]
    public bool PhysicsObject;
    // Data structure for communicating information to the AI
    //Reference to particle gameobject to spawn on collision
    GameObject ParticlePrefab;
    // Reference to the particle system
  //  ParticleSystem pSystem;

    void Start()
    {
        //pSystem = ParticlePrefab.GetComponent<ParticleSystem>()
    }

    
	private void OnCollisionEnter(Collision collision)
	{
//		Debug.Log("Collision");
        if (Time.time >= timeOfImpact + impactCooldown)
		{
			if (PhysicsObject || collision.gameObject.tag == "Map Hazard")
            {
                Debug.Log("You Ran Into Something");
                timeOfImpact = Time.time;
                // Calculate the volume of the noise at the source 
                EmitSound(collision);
            }
        }
	}
    
    
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
//		Debug.Log("Collision");
		if (Time.time >= timeOfImpact + impactCooldown)
		{
			if (PhysicsObject || hit.gameObject.tag == "Map Hazard")
			{
				Debug.Log("You Ran Into Something");
				timeOfImpact = Time.time;
				// Calculate the volume of the noise at the source 
				EmitSound(hit);
			}
	    }
	}

	void EmitSound(Collision col)
	{
		float initialVolume = defaultVolumeBaseline;//; * (col.relativeVelocity.magnitude * 100000000);
		float echo = echoFactor;
		float lifeTime = echo * defaultVolumeBaseline;
		if (col.gameObject.GetComponent<NoiseData>() != null)
		{
			float materialSoundProperty = col.gameObject.GetComponent<NoiseData>().loudnessFactor;
			initialVolume *= materialSoundProperty;
			// calculate initial volume ** ADD FACTOR FOR VELOCITY OF OBJECT ON IMPACT **
			echoFactor *= col.gameObject.GetComponent<NoiseData>().echoFactor;
		}
		ParticlePrefab = Instantiate(Resources.Load("ParticleSound", typeof(GameObject)), transform.position, transform.rotation) as GameObject;
		ActiveSound sound = ParticlePrefab.GetComponent<ActiveSound>();
		sound.TransferVolumeData(initialVolume, lifeTime, transform.position, Time.time);
		// if something weird breaks uncomment? 
		//sound = null;

	}
	void EmitSound(ControllerColliderHit col)
    {
	    float initialVolume = defaultVolumeBaseline;//; * (col.relativeVelocity.magnitude * 100000000);
		float echo = echoFactor;
		float lifeTime = echo * defaultVolumeBaseline;
		if (col.gameObject.GetComponent<NoiseData>() != null)
        {
	        float materialSoundProperty = col.gameObject.GetComponent<NoiseData>().loudnessFactor;
	        initialVolume *= materialSoundProperty;
	        // calculate initial volume ** ADD FACTOR FOR VELOCITY OF OBJECT ON IMPACT **
	        echoFactor *= col.gameObject.GetComponent<NoiseData>().echoFactor;
        }
        ParticlePrefab = Instantiate(Resources.Load("ParticleSound", typeof(GameObject)), transform.TransformPoint(this.GetComponent<CharacterController>().center), transform.rotation) as GameObject;
        ActiveSound sound = ParticlePrefab.GetComponent<ActiveSound>();
        sound.TransferVolumeData(initialVolume, lifeTime, transform.position, Time.time);
        // if something weird breaks uncomment? 
		//sound = null;

	}

	// Update is called once per frame
	void Update()
    {

    }
}
