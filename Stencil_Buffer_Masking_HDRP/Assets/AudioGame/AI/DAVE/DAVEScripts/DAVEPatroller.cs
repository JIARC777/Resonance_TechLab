using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAVEPatroller : IDaveState
{
    GameObject[] patrolPathNodes;
    int curPathIndex;
    bool deactivateAndPassDaveReference = false;
    DAVE thisDave;
    // Start is called before the first frame update
    public void Initialize(DAVE dave)
    {
        thisDave = dave;
        patrolPathNodes = thisDave.patrolPathNodes;
        curPathIndex = thisDave.curPathIndex;
    }

    // Update Cycle looks for events to deactivate itself and pass dave the new current state if it needs to
    public void UpdateCycle(DAVE dave)
    {
       
    }

    void InitializeInvestigator(ActiveSound noiseHeard)
	{
        thisDave.currentState = new DAVEInvestigator();
        thisDave.crossStateData.postUpdatedSoundInfo(noiseHeard);
        thisDave.currentState.Initialize(thisDave);
	}
    void InitializeChasePlayer(Vector3 suspectedPlayerPosition)
    {
        //thisDave.currentState = new DAVEInvestigator();
    }
    public void GoToNextPatrolNode(DAVE dave)
	{
        thisDave.curPathIndex = (curPathIndex + 1 % patrolPathNodes.Length);
        thisDave.SetDestination(patrolPathNodes[curPathIndex].transform.position);
	}
}
