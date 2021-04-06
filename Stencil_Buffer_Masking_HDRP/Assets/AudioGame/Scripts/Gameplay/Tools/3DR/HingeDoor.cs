using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeDoor : MonoBehaviour
{
    private HingeJoint hinge;
    private JointMotor motor;
    
    bool isClosed = false;
    public  bool isLoaded = true;
    // Start is called before the first frame update
    void Start()
    {
        hinge = GetComponent<HingeJoint>();

        motor.force = 20;
        motor.targetVelocity = -50;
        motor.freeSpin = false;

        hinge.motor = motor;
        hinge.useMotor = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(hinge.connectedBody.transform.localEulerAngles.y);
        if (Mathf.Abs(hinge.connectedBody.transform.localEulerAngles.y - 90) < 0.5f && isLoaded)
        {
            Locked();
        }
    }

    public void Locked()
    {
        if (!isClosed)
        {
            hinge.connectedBody.constraints = RigidbodyConstraints.FreezePosition;
            isClosed = true;
        }
    }

    public void UnlockDoor()
    {
        StartCoroutine(Unlock());
    }

    IEnumerator Unlock()
    {
        if (isClosed)
        {
            hinge.connectedBody.constraints = RigidbodyConstraints.None;

            hinge.useMotor = true;

            yield return new WaitForSeconds(1);
            
            isClosed = false;

            hinge.useMotor = false;
        }
    }
}
