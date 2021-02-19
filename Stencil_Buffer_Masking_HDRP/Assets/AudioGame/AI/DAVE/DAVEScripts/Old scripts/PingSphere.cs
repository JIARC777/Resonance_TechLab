using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingSphere : MonoBehaviour
{
    public float maxRadius = 5f;
    //public float lifeTime = 5f;
    public bool active;
    float curScale;
    public bool foundPlayer = false;
    public delegate void PlayerDetected(Vector3 position);
    public static event PlayerDetected DetectedPlayer;
    // Start is called before the first frame update
    void Awake()
    {
        transform.localScale = new Vector3(0, 0, 0);
       // PlayerDetector.OnDetection += FoundPlayer;
        active = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        transform.localScale = new Vector3(curScale,curScale,curScale);
        curScale += .5f;
        if (curScale >= maxRadius)
		{
            active = false;
            Destroy(this.gameObject);
		}
    }

    void OnTriggerEnter(Collider other)
    {
	    if (other.tag == "Player")
	    {
		    DetectedPlayer(other.transform.position);
	    }
    }
}
