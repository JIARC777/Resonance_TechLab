using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoolHolder : MonoBehaviour
{
    public Transform playerHeadPos;
    public Vector3 offsetFromHead;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = playerHeadPos.localPosition + offsetFromHead;
    }
}
