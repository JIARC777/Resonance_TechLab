using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DAVESys : MonoBehaviour
{
    [HideInInspector]
    public bool active;
    [HideInInspector]
    public bool updateDestination;
    protected NavMeshAgent agent;
    protected Transform trans;
    Vector3 targetPos;
    bool initialized = false;

    protected void InitializeSystems()
	{
        initialized = true;
        agent = this.GetComponent<NavMeshAgent>();
        trans = this.GetComponent<Transform>();
	}

    public void SetTarget(Vector3 target)
    {
        updateDestination = false;
        targetPos = target;
        agent.SetDestination(targetPos);
    }

    protected virtual void Update()
	{
        if (initialized)
		{
            if ((trans.position - targetPos).magnitude <= agent.stoppingDistance)
            {
                updateDestination = true;
            }
        }
            
	}
}
