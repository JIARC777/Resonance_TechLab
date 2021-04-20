using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TransitionCanvas : MonoBehaviour
{
    private void OnDestroy()
    {
        Destroy(this);
    }

    public Canvas transitionCanvas;

    void Start()
    {
        transitionCanvas.worldCamera = GameObject.Find("VRCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
