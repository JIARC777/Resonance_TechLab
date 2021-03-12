using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is very similar to DAVEInvestigator - we might consider combining if we have time. Big difference is triggering event does not live on dave but any trigger colliders
public class DAVEChaser : IDaveState
{
    // Start is called before the first frame update
    DAVE thisDave;
    float timeSinceLastPing;
    //Vector3 last

    private float timeOfArrival;
    // Since a ping takes a noticable finite amount of time, this keeps us from killing the state until we assume the ping returned void 
    bool bIsWaitingAtLocation = false;
    float pingWaitTime = 2f;
    private float noiseStartWaitTime;
    public void Initialize(DAVE dave)
    {
        Debug.Log("<color=red>Entering: Chaser</color>");
        thisDave = dave;
        thisDave.waitingAtLocation = false;
        TravelToSuspectedPlayerPos(thisDave.lastKnownPlayerLocation);
        thisDave.statusLight.color = thisDave.chaserModeColor;
        thisDave.ArrivedAtDestination += ReachedOldPlayerPosPing;


       // thisDave.PingExited += PingExited;
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
            } else
			{
                Exit();
			}
        }
        
    }

    public void TravelToSuspectedPlayerPos(Vector3 suspectedPlayerLocation)
    {
        Debug.Log("Set Player Destination: " + suspectedPlayerLocation);
        thisDave.SetDestination(suspectedPlayerLocation);
    }

    public void ReachedOldPlayerPosPing(DAVE dave)
    {
        ///TODO: If the player ping didn't find anything, exit out to Patroller
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

    private void PingExited(DAVE dave)
    {//The ping Didn't find anything, exiting out to Patroller
        Debug.Log("<color=purple> Exiting Chaser through PingExit</color>");
        //Debug.Assert(1 == 2);
    }

    // Someone brave can look at fitting this into the execution loop for DAVE 
    public void Exit()
    {
        Debug.Log("<color=red> Exiting Chaser</color>");
        thisDave.engagedPlayer = false;
        thisDave.ArrivedAtDestination -= ReachedOldPlayerPosPing;
        thisDave.currentState = new DAVEPatroller();
        thisDave.currentState.Initialize(thisDave);
    }
}
