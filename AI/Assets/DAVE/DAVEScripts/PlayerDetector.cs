using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This should live on the player itself to act as a target for the AI to recognize a player collision within its own colliders
// Since it is an event system, it assumes only one player, 
public class PlayerDetector : MonoBehaviour
{
    public delegate void PlayerDetected(Vector3 position);
    public static event PlayerDetected OnDetection;

    float cooldown = 1f;
    float timeOfImpact; 
    // Start is called before the first frame update
    void Start()
    {
        
    }
	private void OnTriggerEnter(Collider other)
	{
        if (Time.time >= timeOfImpact + cooldown)
		{   
            if (other.tag == "DAVEVision")
            {
                timeOfImpact = Time.time;
                Debug.Log("DAVE has found you. Run!");
                OnDetection(transform.position);
            }
        }
        // This acts as a clean way to create an event for when the player is detected.
		
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
