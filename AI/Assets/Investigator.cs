using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Investigator : DAVESys
{
    List<ActiveSound> activeSounds = new List<ActiveSound>();
    // Start is called before the first frame update
    void Start()
    {
        InitializeSystems();
    }

    public void AddSound(ActiveSound sound)
    {
        active = true;
        activeSounds.Add(sound);
        Prioritize();
    }

    void Prioritize()
	{
        activeSounds = activeSounds.OrderBy(s => s.curVolume).ToList<ActiveSound>();
        SetTarget(activeSounds[0].soundLocation);
    }
    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (updateDestination && activeSounds.Count > 1)
		{
            activeSounds.RemoveAt(0);
            if (activeSounds.Count > 0)
                Prioritize();
		} else
		{
            active = false;
		}
    }
}
