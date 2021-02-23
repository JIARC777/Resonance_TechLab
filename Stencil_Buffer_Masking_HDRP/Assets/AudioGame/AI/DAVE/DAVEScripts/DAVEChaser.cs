using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is very similar to DAVEInvestigator - we might consider combining if we have time. Big difference is triggering event does not live on dave but any trigger colliders
public class DAVEChaser : IDaveState
{
    // Start is called before the first frame update
    DAVE thisDave;
    float timeSinceLastPing;

    private float timeOfArrival;
    // Since a ping takes a noticable finite amount of time, this keeps us from killing the state until we assume the ping returned void 
    float timeTime = 5F;
    public void Initialize(DAVE dave)
    {
        Debug.Log("Player Found: Initialized Chaser");
        thisDave = dave;
        ChasePlayer(thisDave.lastKnownPlayerLocation);
        thisDave.ArrivedAtDestination += Investigate;
    }

    // Update is called once per frame
    public void UpdateCycle(DAVE dave)
    {
        if (thisDave.pingFoundPlayer)
        {
            // As soon as its true, set false
            thisDave.pingFoundPlayer = false;
            ChasePlayer(thisDave.lastKnownPlayerLocation);
        }
    }

    public void ChasePlayer(Vector3 suspectedPlayerLocation)
    {
        
        thisDave.SetDestination(suspectedPlayerLocation);
    }

    public void Investigate(DAVE dave)
    {
        timeSinceLastPing = Time.time;
        // We asssume that if this ping returns a hit, we will reassign the position
        thisDave.PingSurroundings();
    }


    public void Exit()
    {
        thisDave.ArrivedAtDestination -= Investigate;
    }
}
