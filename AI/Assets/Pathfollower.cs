using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfollower : DAVESys
{
    public GameObject[] nodeList;
    GameObject nextNode;
    // Start is called before the first frame update
    void Start()
    {
        InitializeSystems();
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        // if this system is not active it means the system is investigating. In the background, search for the target node to assign next
        if (!active)
		{
          //  lookForClosestNode();
		}
    }

    
}
