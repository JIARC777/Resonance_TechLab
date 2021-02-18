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
    //public BoxCollider daveBox;

	// Start is called before the first frame update
	void Start()
	{
        PlayerDetector.OnDetection += DetectedPlayer;
        patrolSystem = GetComponent<Pathfollower>();
        investigationSystem = GetComponent<Investigator>();
        patrolSystem.active = true;
        investigationSystem.active = false;
	}
	// Update is called once per frame
	void Update()
    {
	    Debug.Log("Dave is working");
        // A quick check to make sure the patrol system takes over when the investigation system has finished
        if (investigationSystem.active == false && patrolSystem.active == false)
		{
            ToggleSystems(0);
        }
            
    }
    void DetectedPlayer(Vector3 playerPos)
	{
        ToggleSystems(1);
	}

    // This only works if we make sure that one system is initialized as active and another isnt on start
    void ToggleSystems(int sysIndex)
	{
        Debug.Log("Systems Toggled");
        switch(sysIndex)
		{
            case 0:
                patrolSystem.active = true;
                patrolSystem.ReInitialize();
                investigationSystem.active = false;
                break;
            case 1:
                patrolSystem.active = false;
                investigationSystem.active = true;
                break;
		}
	}
	public void OnParticleCollision(GameObject noise)
	{  
		Debug.Log("Dave hit a particle");
		if (noise.tag == "Noise")
		{
            int noiseID = noise.GetComponent<ActiveSound>().Id;
            // While this hash does increase the chance of collision it offers a simple method of self-updating that always insures that a new sound is identified correctly before a potential collision case 
            if (soundIdHashes[noiseID % 100] != noiseID)
			{
                soundData = noise.GetComponent<ActiveSound>();
                soundIdHashes[noiseID % 100] = noiseID;
                Debug.Log("Beware! D.A.V.E. heard you");
                if (investigationSystem.active == false)
                    ToggleSystems(1);
                investigationSystem.AddSound(soundData);
            }
        }
	}
}
