using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingSphere : PlayerDetector
{
    public float maxRadius = 5f;
    //public float lifeTime = 5f;
    float curScale;
    // Start is called before the first frame update
    
    void Awake()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localScale = new Vector3(curScale,curScale,curScale);
        curScale += .1f;
        if (curScale >= maxRadius)
        {
            Destroy(this.gameObject);
		}
    }

     void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            Debug.Log("Ping Detected Player");
        base.OnTriggerEnter(other);
    }
}
