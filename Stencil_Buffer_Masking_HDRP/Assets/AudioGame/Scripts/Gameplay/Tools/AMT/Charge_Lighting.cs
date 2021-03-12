using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge_Lighting : MonoBehaviour
{
    public Light emit;

    public float maxIntensity = 3f;
    public float minIntensity = 0.1f;

    public float intensityMult = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        emit.intensity = minIntensity;
    }
    
    public void ChargeUp(float percentage)
    {
        emit.intensity += percentage * intensityMult;

        if (percentage >= maxIntensity)
            emit.intensity = maxIntensity;
    }

    public void ChargeDown(float percentage)
    {
        emit.intensity = minIntensity;
    }
}