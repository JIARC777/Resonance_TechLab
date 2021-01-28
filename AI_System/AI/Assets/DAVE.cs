using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//eventually we may want to refactor this so that we use base class with inheritance
public class DAVE : MonoBehaviour
{
    int lastID = -1;
    ActiveSound soundData;
    // implement an array the length of 
    int[] detectedIDs = new int[100];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnParticleCollision(GameObject noise)
	{
        // This lastID == -1 acts as a "starter" case so that the system  
		if (noise.tag == "Noise" && (lastID == 0 || lastID)
		{
            ActiveSound soundData = noise.GetComponent<ActiveSound>();
            Debug.Log("Beware! D.A.V.E. is Nigh!");

		}
	}
}
