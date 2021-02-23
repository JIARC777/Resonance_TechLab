using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class DAVESys : MonoBehaviour
{
    [HideInInspector]
    public bool active;
    [FormerlySerializedAs("updateDestination")] [HideInInspector]
    public bool arrivedAtDestination;
    protected NavMeshAgent agent;
    protected Transform trans;
    protected Vector3 targetPos;
    bool initialized = false;

    protected void InitializeSystems()
	{
        initialized = true;
        agent = this.GetComponent<NavMeshAgent>();
        trans = this.GetComponent<Transform>();
	}

    public void SetTarget(Vector3 target)
    {
        arrivedAtDestination = false;
        targetPos = target;
        agent.SetDestination(targetPos);
    }

    protected virtual void Update()
	{ 
        if (initialized)
		{
            // This float is kinda funky - just adjust it as necessary to make sure that the ping sphere comes after the Drone stopped
            if ((trans.position - targetPos).magnitude + 0.75f <= agent.stoppingDistance)
            {
                arrivedAtDestination = true;
            }
        }
    }
}
