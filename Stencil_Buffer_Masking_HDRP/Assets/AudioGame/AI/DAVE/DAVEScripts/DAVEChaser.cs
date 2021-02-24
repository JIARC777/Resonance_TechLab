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
    bool bIsWaitingAtLocation = false;
    float pingWaitTime = 1f;
    private float noiseStartWaitTime;
    public void Initialize(DAVE dave)
    {
        Debug.Log("Player Found: Initialized Chaser");
        thisDave = dave;
        TravelToSuspectedPlayerPos(thisDave.lastKnownPlayerLocation);
        thisDave.ArrivedAtDestination += ReachedOldPlayerPosPing;
    }

    // Update is called once per frame
    public void UpdateCycle(DAVE dave)
    {
        var doneWaitingOnInvestigation = bIsWaitingAtLocation && Time.time >= noiseStartWaitTime + pingWaitTime;
        if (doneWaitingOnInvestigation)
        {
          //  Debug.Log("Done Investiagting");
            // Only after this waiting period, check to see if the ping hit anything
            if (thisDave.pingFoundPlayer)
            {
                Debug.Log("Found Player Again");
                bIsWaitingAtLocation = false;
                // As soon as its true, set false
                thisDave.pingFoundPlayer = false;
                TravelToSuspectedPlayerPos(thisDave.lastKnownPlayerLocation);
            }
        }
        
    }

    public void TravelToSuspectedPlayerPos(Vector3 suspectedPlayerLocation)
    {
        Debug.Log("Set Player Destination");
        thisDave.SetDestination(suspectedPlayerLocation);
    }

    public void ReachedOldPlayerPosPing(DAVE dave)
    {
        Debug.Log("Reached old player location");
        
        // We asssume that if this ping returns a hit, we will reassign the position
        
        if (!bIsWaitingAtLocation)
        {
            thisDave.PingSurroundings();
            Debug.Log("Arrived and Waiting");
            bIsWaitingAtLocation = true;
            noiseStartWaitTime = Time.time;
        }
        
    }

    // Someone brave can look at fitting this into the execution loop for DAVE 
    public void Exit()
    {
        thisDave.ArrivedAtDestination -= ReachedOldPlayerPosPing;
    }
}
