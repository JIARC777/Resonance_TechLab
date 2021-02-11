using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Investigator : DAVESys
{
    List<ActiveSound> activeSounds = new List<ActiveSound>();
    public Transform playerPosRef;
    public float damageRadius = 3f;
    public Transform pingSource;
    bool chasingPlayer = false;
    // Start is called before the first frame update
    void Start()
    {
        InitializeSystems();
        PlayerDetector.OnDetection += ChasePlayer;
    }

    public void AddSound(ActiveSound sound)
    {
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
        if (active)
		{
            
            base.Update();
            if (updateDestination && activeSounds.Count >= 1)
            {
                activeSounds.RemoveAt(0);
                PingSurroundings();
                if (activeSounds.Count > 0)
                    Prioritize();
                
            }
            else if (activeSounds.Count == 0 && !chasingPlayer)
            {
                Debug.Log("Reactivate Patrol");
                active = false;
            }
        }
        
    }

    void PingSurroundings()
	{
       GameObject pingSphere = Instantiate(Resources.Load("PingSphere", typeof(GameObject)), pingSource.position, transform.rotation) as GameObject;
        if (pingSphere.GetComponent<PingSphere>().foundPlayer == false)
            chasingPlayer = false;
	}

    void ChasePlayer(Vector3 playerPos)
	{
        chasingPlayer = true;
        Debug.Log(chasingPlayer);
        SetTarget(playerPos);
        if ((this.transform.position - playerPosRef.position).magnitude <= damageRadius)
		{
            AttackPlayer();
		} else if (updateDestination)
		{
            PingSurroundings();
		} 
	}

    void AttackPlayer()
	{
        Debug.Log("Attacking");
	}
}
