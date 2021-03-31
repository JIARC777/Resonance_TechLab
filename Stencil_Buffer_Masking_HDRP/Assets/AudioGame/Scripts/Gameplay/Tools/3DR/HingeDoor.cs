using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeDoor : MonoBehaviour
{
    HingeJoint hinge;
    bool isClosed = false;
    // Start is called before the first frame update
    void Start()
    {
        hinge = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(hinge.connectedBody.transform.localEulerAngles.y);
        if (Mathf.Abs(hinge.connectedBody.transform.localEulerAngles.y - 90) < 0.5f)
        {
            Close();
        }
    }

    void Close()
    {
        if (!isClosed)
        {
            hinge.connectedBody.constraints = RigidbodyConstraints.FreezePosition;
            isClosed = true;
        }
    }
}
