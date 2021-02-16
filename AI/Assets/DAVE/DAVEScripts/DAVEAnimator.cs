using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DAVEAnimator : MonoBehaviour
{
    Rigidbody rb;
    public Transform modelTrans;
    NavMeshAgent agent;
    // Start is called before the first frame update
    float lastFrameSpeed = 0;
    float lastFrameAngularSpeed = 0;
    float lastFrameDeltaV = 0;
    public float rotationFactor;
    public float flyForwardRotation = 3f;
    // To correct for really small deviations in velocity use this as a precision value to round to zero
    float floatingPointSmooth = .01f;
    float idleFlyBounds = 3f;
    float lerpCounter = 0;
    Vector3 idleFlyDirection = new Vector3(1, 1, 1);
    float maxAccel;
    float minAccel;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (agent.velocity.magnitude > 0)
		{
           // rotateForward();
        }
    }
    /*
    void rotateSideways()
	{
        float deltaVAng = (rb.angularVelocity.magnitude - lastFrameAngularSpeed);
        if (Mathf.Abs(deltaVAng) < floatingPointSmooth)
            deltaVAng = 0;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, deltaVAng * rotationFactor);
        lastFrameSpeed = rb.velocity.magnitude;
    }
    */
    /*
    void rotateForward()
	{
        float newBaseRotation = 0;
        float curSpeed = agent.velocity.magnitude;
        float maxSpeed = agent.speed;
        float maxAccel = agent.acceleration;
        // meausure the acceleration magnitude in order to create an angle to rotate the AI
        // This assumes accleration is primarily in the forward direction
        float deltaV = (curSpeed - lastFrameSpeed);
        if (Mathf.Abs(deltaV) < floatingPointSmooth)
            deltaV = 0;
        //Debug.Log("DeltaV: " + deltaV);
        if (deltaV > 0)
		{
            
        }
        float forwardRotation = Mathf.Lerp(0, maxAccel, 1 - (Mathf.Abs(deltaV)/maxAccel)) * rotationFactor;
        Debug.Log(forwardRotation);
        // Really exaggerate decceleration 
        if (deltaV < 0)
            //forwardRotation
        // apply the acceleration to rotation
        modelTrans.rotation = Quaternion.Euler(modelTrans.rotation.eulerAngles.x + forwardRotation, modelTrans.rotation.eulerAngles.y, modelTrans.rotation.eulerAngles.z);
        lastFrameSpeed = curSpeed;
        lastFrameDeltaV = deltaV;
    }
    */
}
