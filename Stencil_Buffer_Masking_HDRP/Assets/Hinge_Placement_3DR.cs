using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hinge_Placement_3DR : MonoBehaviour
{

    public HingeJoint doorHinge;

    // Start is called before the first frame update
    void Start()
    {
        doorHinge = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        doorHinge.connectedAnchor = new Vector3(transform.position.x - 0.2f, transform.position.y, transform.position.z);
    }
}
