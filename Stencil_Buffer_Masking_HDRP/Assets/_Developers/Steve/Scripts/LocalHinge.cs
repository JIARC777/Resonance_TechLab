using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalHinge : MonoBehaviour
{
    public HingeJoint hinge;
    public Transform rotatorReference;

    private void FixedUpdate()
    {
        hinge.axis = rotatorReference.forward;
        Debug.DrawRay(rotatorReference.position, rotatorReference.forward);
    }
}
