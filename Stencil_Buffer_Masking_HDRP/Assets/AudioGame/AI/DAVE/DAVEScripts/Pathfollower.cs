using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfollower : DAVESys
{
    public GameObject[] nodeList;
    int curNodeIndex;
    // Start is called before the first frame update
    void Start()
    {
        InitializeSystems();
        active = true;
    }
    public void ReInitialize()
	{
        updateDestination = true;
	}
    // Update is called once per frame
    void Update()
    {
        if (active)
		{
            base.Update();
            // Check to see if the investigator passed a now irrelevant target position
            
            if (updateDestination)
			{
                curNodeIndex = (curNodeIndex + 1) % nodeList.Length;
                SetTarget(nodeList[curNodeIndex].transform.position);
			}
        }
    }

    
}
