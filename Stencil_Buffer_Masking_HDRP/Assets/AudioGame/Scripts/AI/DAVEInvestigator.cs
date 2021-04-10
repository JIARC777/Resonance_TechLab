using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Something happened, we got there, check for the player by pinging
/// </summary>
public class DAVEInvestigator : IDaveState
{
    DAVE thisDave;
    List<ActiveSound> soundsToInvestigate = new List<ActiveSound>();

    private float noiseWaitTime = 1f;
    private float noiseStartWaitTime;

    private bool bIsWaitingOnInvestigation = false;

    public void StateEnter(DAVE dave)
    {
        Debug.Log("<color=green>Entering: Investigator</color>");
        thisDave = dave;
        thisDave.waitingAtLocation = false;
        dave.HeardNoise += Add;
        thisDave.ArrivedAtDestination += PingAtInvestigateSound;
        thisDave.statusLight.color = thisDave.investigationModeColor;
    }

    public void StateUpdate()
    {
        var doneWaitingOnInvestigation = bIsWaitingOnInvestigation && Time.time >= noiseStartWaitTime + noiseWaitTime;
        if (doneWaitingOnInvestigation)
        {
            Debug.Log("Done waiting on investigation");
            bIsWaitingOnInvestigation = false;
            soundsToInvestigate.RemoveAt(0);
            if (soundsToInvestigate.Count == 0) //No sounds to investigate
            {
                StateExit();
            }
            else
            {
                PrioritizeSounds();
            }
        }
    }

    void Add(ActiveSound noise)
    {

        noise.soundLocation.y = 0.12f;
        // Assign the Active Sound's volume based on its now current volume (right at time of impact)
        noise.volumeAtImpact = noise.curVolume;
        //Debug.Log("Noise Heard" + noise.soundLocation);
        soundsToInvestigate.Add(noise);
        if (soundsToInvestigate.Count > 0)
        {
            Debug.Log("Total Noises Heard " + soundsToInvestigate.Count);
            PrioritizeSounds();
        }
    }


    /// <summary>
    /// More than one sound, prioritize by volume
    /// </summary>
    void PrioritizeSounds()
    {
        if (soundsToInvestigate.Count > 1)
        {
            // Toggle between the Linq expression and our custom function 
            // soundsToInvestigate = soundsToInvestigate.OrderBy(s => s.curVolume).ToList<ActiveSound>();
            soundsToInvestigate = SortAndReorderSounds(soundsToInvestigate);
            Debug.Log("Sound with Max Volume: " + soundsToInvestigate[0].Id + ": " + soundsToInvestigate[0].volumeAtImpact + "at location: " + soundsToInvestigate[0].soundLocation);
        }

        if (soundsToInvestigate.Count > 0)
        {
            thisDave.SetDestination(soundsToInvestigate[0].soundLocation);
            Debug.Log(soundsToInvestigate.Count + " sounds to investigate");
        }
    }

    List<ActiveSound> SortAndReorderSounds(List<ActiveSound> soundList)
    {
        // Reference to maximum volume of noise
        float maxVol = 0f;
        int loudestSoundIndex = 0;
        for (int i = 0; i < soundList.Count; i++)
        {
            if (soundList[i].volumeAtImpact > maxVol)
                loudestSoundIndex = i;
        }

        // I think that a simple swap function should be just fine - make sure to just set the loudest sound to the front of the array since that is what the code working with the Linq sorter assumes 
        // since we are working with such small lists on average, and this function is called on every new sound we do not need to sort the entire array each time
        // if we want to go all CS125, we can create a seperate function for the swap
        ActiveSound tempSound = null;
        tempSound = soundList[loudestSoundIndex];
        soundList[loudestSoundIndex] = soundList[0];
        soundList[0] = tempSound;

        // Not sure if we really need to null the tempSound or not but its quick to do
        tempSound = null;
        // Return the reordered list
        return soundList;
    }

    /// <summary>
    /// Arrived at sound source, ping to try and find player
    /// </summary>
    /// <param name="dave"></param>
    void PingAtInvestigateSound(DAVE dave)
    {
        dave.PingSurroundings();
        bIsWaitingOnInvestigation = true;
        noiseStartWaitTime = Time.time;
    }

    public void StateExit()
    {
        Debug.Log("<color=green>Exting Investigator</color>");
        thisDave.HeardNoise -= Add;
        thisDave.ArrivedAtDestination -= PingAtInvestigateSound;

        // This is where we transition into the patroller state instead of in the DAVE base class //yea but why
        thisDave.currentState = new DAVEPatroller();
        thisDave.currentState.StateEnter(thisDave);
    }
}