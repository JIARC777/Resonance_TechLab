using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceOnDrop : MonoBehaviour
{
    public Transform Origin;
    Quaternion rotation;

    private void Start()
    {
        rotation = transform.rotation;
    }
    public void OnDrop()
    {
        transform.position = Origin.position;
        transform.rotation = rotation;
    }
}
