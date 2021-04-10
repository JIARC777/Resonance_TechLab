using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is very similar to DAVEInvestigator - we might consider combining if we have time. Big difference is triggering event does not live on dave but any trigger colliders
public class DAVEChaser : IDaveState
{
    // Start is called before the first frame update
    DAVE thisDave;
    //Vector3 last


    bool bIsWaitingAtLocation = false;

    float pingWaitUntilFinishedTime = 2f;

    private float noiseStartWaitTime;

    public void StateEnter(DAVE dave)
    {
        Debug.Log("<color=red>Entering: Chaser</color>");
        thisDave = dave;
        thisDave.waitingAtLocation = false;
        TravelToSuspectedPlayerPos(thisDave.lastKnownPlayerLocation);
        thisDave.statusLight.color = thisDave.chaserModeColor;
        thisDave.ArrivedAtDestination += ReachedOldPlayerPosPing;


    }


    public void StateUpdate()
    {
        var doneWaitingOnInvestigation = bIsWaitingAtLocation && Time.time >= noiseStartWaitTime + pingWaitUntilFinishedTime;
        
        if (doneWaitingOnInvestigation)
        {
            
            if (thisDave.pingFoundPlayer)// Only after this waiting period, check to see if the ping hit anything
            {
                Debug.Log("Found Player Again");
                bIsWaitingAtLocation = false;
                // As soon as its true, set false
                thisDave.pingFoundPlayer = false;
                TravelToSuspectedPlayerPos(thisDave.lastKnownPlayerLocation);
            }
            else
            {
                StateExit();
            }
        }
    }

    public void TravelToSuspectedPlayerPos(Vector3 suspectedPlayerLocation)
    {
        suspectedPlayerLocation.y = .12f;
        // Debug.Log("Set Player Destination: " + suspectedPlayerLocation);
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
    

    // Someone brave can look at fitting this into the execution loop for DAVE 
    public void StateExit()
    {
        Debug.Log("<color=red> Exiting Chaser</color>");
        thisDave.engagedPlayer = false;
        thisDave.ArrivedAtDestination -= ReachedOldPlayerPosPing;
        thisDave.currentState = new DAVEPatroller();
        thisDave.currentState.StateEnter(thisDave);
    }
}