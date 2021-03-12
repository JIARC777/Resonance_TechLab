using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckPositioner : MonoBehaviour
{
    public LayerMask groundMask;
    public Transform headPos;
    public float maxHeight;

    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CheckPos", 0f, 0.2f);
    }

    void CheckPos()
    {
        if (Physics.Raycast(headPos.position, Vector3.down, out hit, maxHeight, groundMask))
        {
            transform.position = hit.point;
        }
    }
}
