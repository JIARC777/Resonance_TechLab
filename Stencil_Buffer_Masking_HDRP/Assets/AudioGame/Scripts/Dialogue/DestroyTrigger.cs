using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTrigger : MonoBehaviour
{
    [Header("Have an event call this object's DestroyObject")]
    public GameObject objectToDestroy;
    
    public void DestroyObject()
    {
        Destroy(objectToDestroy);
        Destroy(this);
    }
}
