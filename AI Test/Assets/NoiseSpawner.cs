using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSpawner : MonoBehaviour
{
    public NoiseEmitter noiseEmitter;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Instantiate(noiseEmitter, hit.point, Quaternion.Euler(-90,0,0));
;            }
        }
        
    }
}
