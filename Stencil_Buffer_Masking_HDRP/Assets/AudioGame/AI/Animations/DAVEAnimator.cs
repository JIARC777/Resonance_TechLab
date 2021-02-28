using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is meant to live on the DAVE model itself 
public class DAVEAnimator : MonoBehaviour
{
    Rigidbody rb;
    Animator movementAnim;
    Transform parentWorldTransform;
    float deltaVAng;
    float maxdeltaVAng;
    Vector3 lastFramePosition;
    Quaternion lastFrameRotation;
    // Start is called before the first frame update
    void Start()
    {
        movementAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        parentWorldTransform = GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RotateForward();
        RotateSideways();
    }

    void RotateForward()
	{
        /*
        float deltaV = (parentWorldTransform.position - lastFramePosition).magnitude * Time.deltaTime;
        deltaV = Mathf.Round(deltaV * 10000) / 10000;
        deltaV *= 1000f;
        if (deltaV > 1)
            deltaV = 1;
        movementAnim.SetFloat("ForwardSpeedFactor", deltaV);
        lastFramePosition = parentWorldTransform.position;
        */
	}
    void RotateSideways()
    {/*
        float deltaVAng = Mathf.DeltaAngle(parentWorldTransform.rotation.y, lastFrameRotation.y) * Time.deltaTime;
        deltaVAng *= 5f;
        movementAnim.SetFloat("RotationFactor", deltaVAng);
        lastFrameRotation = parentWorldTransform.rotation;
        Debug.Log(deltaVAng);*/
    }
}
