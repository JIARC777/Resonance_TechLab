using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DAVEInvestigator : IDaveState
{
    List<ActiveSound> soundsToInvestigate = new List<ActiveSound>();
    DAVE thisDave;
    // Start is called before the first frame update
    public void Initialize(DAVE dave)
    {
        dave.HeardNoise += Add;
        thisDave = dave;
    }

    // Update is called once per frame
    public void UpdateCycle(DAVE dave)
    {
        
    }

    void Add(ActiveSound noise)
    {
        soundsToInvestigate.Add(noise);
        if (soundsToInvestigate.Count > 1)
            PrioritizeSounds();
    }

    void PrioritizeSounds()
    {
        if (soundsToInvestigate.Count >= 1)
        {
            soundsToInvestigate = soundsToInvestigate.OrderBy(s => s.curVolume).ToList<ActiveSound>();
        }
        thisDave.SetDestination(soundsToInvestigate[0].soundLocation);
    }
}


