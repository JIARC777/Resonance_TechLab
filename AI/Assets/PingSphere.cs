using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingSphere : MonoBehaviour
{
    public float maxRadius = 5f;
    float curScale;
    public bool foundPlayer = false;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        PlayerDetector.OnDetection += FoundPlayer;
    }

    // Update is called once per frame
    void Update()
    {

        transform.localScale = new Vector3(curScale,curScale,curScale);
        curScale += .05f;
        if (curScale >= maxRadius)
		{
            Destroy(this.gameObject);
		}
    }

     void FoundPlayer(Vector3 pos)
	 {
        foundPlayer = true;
	 }
}
