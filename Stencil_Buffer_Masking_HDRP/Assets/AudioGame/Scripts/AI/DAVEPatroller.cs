using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAVEPatroller : IDaveState
{
    GameObject[] patrolPathNodes;
    bool deactivateAndPassDaveReference = false;
    DAVE thisDave;
    // Start is called before the first frame update
    public void StateEnter(DAVE dave)
    {
        Debug.Log("<color=blue>Entering Patroller</color>");
        thisDave = dave;
        thisDave.waitingAtLocation = false;
        patrolPathNodes = dave.patrolPathNodes;
        dave.ArrivedAtDestination += GoToNextPatrolNode;
        thisDave.statusLight.color = thisDave.patrolModeColor;
        GoToNextPatrolNode(thisDave);
        
    }

    // Update Cycle looks for events to deactivate itself and pass dave the new current state if it needs to
    public void StateUpdate()
    {
       // Debug.Log("Patroller Is Running");
    }
    public void GoToNextPatrolNode(DAVE dave)
	{
        Debug.Log("Patroller: Go To Next Patrol Node");
        dave.currentPatrolPathIndex = ((dave.currentPatrolPathIndex + 1) % (patrolPathNodes.Length));
        dave.SetDestination(patrolPathNodes[dave.currentPatrolPathIndex].transform.localPosition);
	}
    public void StateExit()
    {
        
        Debug.Log("<color=blue>Exit Patroller</color>");
        // Make sure to unsub from events on destruction
        thisDave.ArrivedAtDestination -= GoToNextPatrolNode;
    }
}
