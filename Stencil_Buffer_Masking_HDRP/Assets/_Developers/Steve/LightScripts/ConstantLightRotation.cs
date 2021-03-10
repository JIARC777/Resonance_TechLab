using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantLightRotation : MonoBehaviour
{
    public float rotationAmount;

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = transform.eulerAngles + new Vector3(0, rotationAmount * Time.deltaTime, 0);
    }
}
