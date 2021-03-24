using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NoiseData is a small, easily accessible, data class for communicating information about an object when it has been impacted and sound needs to be generated 
// This should be added on just about everything that we want to make noise
// This is meant to be expandable - any object-centric information about a collision needs to go here for a noisemaker to generate sound
public class NoiseData : MonoBehaviour
{
	// Default loudness - this is multiplied to the NoiseMaker's volume in CalculateInitialVolume - keep at 1 to have no effect, +1 to amplify, between 0,1 to dampen
	public float loudnessFactor = 1;
	// The Echo Factor affects the lifespan of particles emitted on collision. It is multiplied by the emitter's base lifespan
	public float echoFactor = 1;
	// AudioClip for whenever sound plays
	//public AudioSource soundFXsource;
	public AudioClip soundFX;


	/*
	private void Awake()
	{
		// make sure everything with a noise data has an audio source component
		soundFXsource = this.GetComponent<AudioSource>();
	}
	*/
}


