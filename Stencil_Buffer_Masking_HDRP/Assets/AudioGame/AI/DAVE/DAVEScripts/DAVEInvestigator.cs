using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DAVEInvestigator : IDaveState
{
    List<ActiveSound> soundsToInvestigate = new List<ActiveSound>();
    DAVE thisDave;

    private float noiseWaitTime = 1f;
    private float noiseStartWaitTime;

    private bool bIsInvestigatingAndWaiting = false;

    // Start is called before the first frame update
    public void Initialize(DAVE dave)
    {
        dave.HeardNoise += Add;
        thisDave = dave;
        thisDave.ArrivedAtDestination += InvestigateSound;
    }

    // Update is called once per frame
    public void UpdateCycle(DAVE dave)
    {
        var doneWaitingOnInvestigation = bIsInvestigatingAndWaiting && Time.time >= noiseStartWaitTime + noiseWaitTime;
        if (doneWaitingOnInvestigation)
        {
            //Debug.Log("Removed sound");
            soundsToInvestigate.RemoveAt(0);
            bIsInvestigatingAndWaiting = false;
            PrioritizeSounds();
            //Exit();
        }
    }

    void Add(ActiveSound noise)
    {
        //Debug.Log("Noise Heard" + noise.soundLocation);
        soundsToInvestigate.Add(noise);
        if (soundsToInvestigate.Count > 0)
        {
            Debug.Log("Total Noises Heard " + soundsToInvestigate.Count);
            PrioritizeSounds();
        }
    }

    void PrioritizeSounds()
    {
        if (soundsToInvestigate.Count >= 1)
        {
            soundsToInvestigate = soundsToInvestigate.OrderBy(s => s.curVolume).ToList<ActiveSound>();
        }

        if (soundsToInvestigate.Count > 0)
        {
            thisDave.SetDestination(soundsToInvestigate[0].soundLocation);
        }
    }

    void InvestigateSound(DAVE dave)
    {
        if (soundsToInvestigate.Count == 0) //No sounds to investigate
        {
            Exit();
        }
        else
        {
            if (!bIsInvestigatingAndWaiting)
            {
                //There's a sound, we're at it, and we should investigate
                //Debug.Log("Arrived");
                dave.PingSurroundings();
                bIsInvestigatingAndWaiting = true;
                noiseStartWaitTime = Time.time;
            }
        }
    }
    
    

    public void Exit()
    {
        Debug.Log("Exting Investigator");
        thisDave.HeardNoise -= Add;
        thisDave.ArrivedAtDestination -= InvestigateSound;
        // This is where we transition into the patroller state instead of in the DAVE base  class
        thisDave.currentState = new DAVEPatroller();
        thisDave.currentState.Initialize(thisDave);
    }
}