using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Investigator : DAVESys
{
    List<ActiveSound> activeSounds = new List<ActiveSound>();
    public Transform playerPosRef;
    public float damageRadius = 2f;
    public Transform pingSource;
    bool chasingPlayer = false;
    bool needToPing = true;
    GameObject pingSphere;
    PingSphere pingsphereInfo;
    // Start is called before the first frame update
    void Start()
    {
        InitializeSystems();
        PlayerDetector.OnDetection += AttackPlayer;
    }

    public void AddSound(ActiveSound sound)
    {
        // immediately ignore player
        chasingPlayer = false;
        activeSounds.Add(sound);
        Prioritize();
    }

    void Prioritize()
	{
        if (activeSounds.Count >= 1)
        {
            activeSounds = activeSounds.OrderBy(s => s.curVolume).ToList<ActiveSound>();
        }
        SetTarget(activeSounds[0].soundLocation);
    }
    // Update is called once per frame
    void Update()
    {
        if (active)
		{
            base.Update();
            // Check if there is an active ping scanning for the player
            if (pingSphere != null)
            {
                if (pingSphere.GetComponent<PingSphere>().foundPlayer == true)
                    needToPing = true;
            }
            if (updateDestination)
            {
                if (needToPing)
                    PingSurroundings();
                else if (activeSounds.Count > 0)
				{
                    Prioritize();
                    activeSounds.RemoveAt(0);
                    PingSurroundings();           
                }
                else if (pingSphere == null && !needToPing)
                {
                    Debug.Log("Here");
                    chasingPlayer = false;
                    // Bit of weird place to end it, but I need to make sure that the investigator doesnt turn off until after the ping has been completed
                    if (activeSounds.Count == 0 && !chasingPlayer)
                    {
                        Debug.Log("Reactivate Patrol");
                        active = false;
                    }
                }
            }
            // This is to stop the case where the drone has reached its destinatiion but still thinks it needs to ping
            if (needToPing && agent.isStopped)
			{
                Debug.Log("Here");
                needToPing = false;
            }
                
        } 
    }

    void PingSurroundings()
	{
        pingSphere = Instantiate(Resources.Load("PingSphere", typeof(GameObject)), pingSource.position, transform.rotation) as GameObject;
        // if collision happens, this is quickly switched back on, if not it will stop the pinging process
        needToPing = false;
    }

    // This is sort of a quick patch for an issue that causes the 
    IEnumerator CheckIfStuck()
	{
        yield return new WaitForSeconds(2f);
	}
    void AttackPlayer(Vector3 playerPos)
	{
        if ((playerPos - trans.position).magnitude > damageRadius)
		{
            Debug.Log("Attack");
		} else
		{
            chasingPlayer = true;
            PingSurroundings();
		}
	}

    void AttackPlayer()
	{
        Debug.Log("Attacking");
	}
}
