using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAVEPatroller : IDaveState
{
    GameObject[] patrolPathNodes;
    bool deactivateAndPassDaveReference = false;
    DAVE thisDave;
    // Start is called before the first frame update
    public void Initialize(DAVE dave)
    {
        // Debug.Log("We are here");
        thisDave = dave;
        patrolPathNodes = dave.patrolPathNodes;
        dave.ArrivedAtDestination += GoToNextPatrolNode;
    }

    // Update Cycle looks for events to deactivate itself and pass dave the new current state if it needs to
    public void UpdateCycle(DAVE dave)
    {
       
    }
    public void GoToNextPatrolNode(DAVE dave)
	{
        dave.currentPatrolPathIndex = ((dave.currentPatrolPathIndex + 1) % (patrolPathNodes.Length));
        dave.SetDestination(patrolPathNodes[dave.currentPatrolPathIndex].transform.localPosition);
	}
    public void Exit()
    {
        
        Debug.Log("Destroy Patroller");
        // Make sure to unsub from events on destruction
        thisDave.ArrivedAtDestination -= GoToNextPatrolNode;
    }
}
