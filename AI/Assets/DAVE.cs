using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//eventually we may want to refactor this so that we use base class with inheritance
public class DAVE : MonoBehaviour
{
    ActiveSound soundData;
    // implement an array the length of 
    int[] soundIdHashes = new int[100];
    Pathfollower patrolSystem;
    Investigator investigationSystem;
	// Start is called before the first frame update
	void Start()
	{
        patrolSystem = GetComponent<Pathfollower>();
        investigationSystem = GetComponent<Investigator>();
	}
	// Update is called once per frame
	void Update()
    {
        
    }
    // This only works if we make sure that one system is initialized as active and another isnt on start
    void ToggleSystems()
	{
        patrolSystem.active = !patrolSystem.active;
        investigationSystem.active = !investigationSystem.active;
	}
	public void OnParticleCollision(GameObject noise)
	{  
		if (noise.tag == "Noise")
		{
            int noiseID = noise.GetComponent<ActiveSound>().Id;
            // While this hash does increase the chance of collision it offers a simple method of self-updating that always insures that a new sound is identified correctly before a potential collision case 
            if (soundIdHashes[noiseID % 100] != noiseID)
			{
                soundData = noise.GetComponent<ActiveSound>();
                soundIdHashes[noiseID % 100] = noiseID;
                Debug.Log("Beware! D.A.V.E. is Nigh!");
                ToggleSystems();
                investigationSystem.AddSound(soundData);
            }
        }
	}
}
