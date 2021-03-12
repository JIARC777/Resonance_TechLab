using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin_Emitter : MonoBehaviour
{

    public float maxSpinSpeed = 5f;
    public float spinUpSpeedMultiplier = 1f;
    public float spinUpSpeedInc = 2f;

    public Transform emitter;

    public float currSpeed;

    // Start is called before the first frame update
    void Start()
    {
        emitter = transform;
    }

    public void SpinUp(float speed)
    {        
        currSpeed = spinUpSpeedMultiplier * speed;        

        if(currSpeed <= maxSpinSpeed)
        {
            emitter.Rotate(currSpeed, 0, 0);
            spinUpSpeedMultiplier += spinUpSpeedInc;
        }            
    }

    public void SpinDown(float speed)
    {
        currSpeed = spinUpSpeedMultiplier * speed;       

        if(currSpeed >= 0)
        {
            emitter.Rotate(currSpeed, 0, 0);
            spinUpSpeedMultiplier -= spinUpSpeedInc;
        }            
    }
}
