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

    private bool bIsWaitingOnInvestigation = false;

    // Start is called before the first frame update
    public void Initialize(DAVE dave)
    {
        Debug.Log("<color=green>Enterering: Investigator</color>");
        dave.HeardNoise += Add;
        thisDave = dave;
        thisDave.waitingAtLocation = false;
        thisDave.ArrivedAtDestination += PingAtInvestigateSound;
        thisDave.statusLight.color = thisDave.investigationModeColor;
    }

    // Update is called once per frame
    public void UpdateCycle(DAVE dave)
    {
        var doneWaitingOnInvestigation = bIsWaitingOnInvestigation && Time.time >= noiseStartWaitTime + noiseWaitTime;
        if (doneWaitingOnInvestigation)
        {
            bIsWaitingOnInvestigation = false;
            soundsToInvestigate.RemoveAt(0);
            if (soundsToInvestigate.Count == 0) //No sounds to investigate
            {
                Exit();
            } else
			{
                PrioritizeSounds();
			}
        }
    }

    void Add(ActiveSound noise)
    {
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

    
    void PrioritizeSounds()
    {
        if (soundsToInvestigate.Count >= 1)
        {   // Toggle between the Linq expression and our custom function 
            // soundsToInvestigate = soundsToInvestigate.OrderBy(s => s.curVolume).ToList<ActiveSound>();
            soundsToInvestigate = SortAndReorder(soundsToInvestigate);
            Debug.Log("Sound with Max Volume: " + soundsToInvestigate[0].Id + ": " + soundsToInvestigate[0].volumeAtImpact + "at location: " + soundsToInvestigate[0].soundLocation);

        }
        if (soundsToInvestigate.Count > 0)
        {
            thisDave.SetDestination(soundsToInvestigate[0].soundLocation);
        }
    }
    // the OrderBy function is clean but doesnt seem to provide the behavior we want - as a testing precaution, we can implement this more open manual function to more easily adjust priority behavior
    List<ActiveSound> SortAndReorder(List<ActiveSound> soundList)
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
    void PingAtInvestigateSound(DAVE dave)
    {
        //There's a sound, we're at it, and we should investigate
        //Debug.Log("Arrived At Sound");
        dave.PingSurroundings();
        bIsWaitingOnInvestigation = true;
        noiseStartWaitTime = Time.time;
    }
    
    public void Exit()
    {
        Debug.Log("<color=green>Exting Investigator</color>");
        thisDave.HeardNoise -= Add;
        thisDave.ArrivedAtDestination -= PingAtInvestigateSound;
        // This is where we transition into the patroller state instead of in the DAVE base class
        thisDave.currentState = new DAVEPatroller();
        thisDave.currentState.Initialize(thisDave);
    }
}