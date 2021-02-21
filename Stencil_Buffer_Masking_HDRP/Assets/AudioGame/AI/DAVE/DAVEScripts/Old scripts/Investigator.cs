using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Investigator : DAVESys
{
    List<ActiveSound> activeSounds = new List<ActiveSound>();
    public Vector3 playerPosRef;
    public float damageRadius = 2f;
    public Transform pingSource;
    bool chasingPlayer = false;
    bool tryAttackPlayer = false;
    GameObject pingSphere;
    PingSphere pingsphereInfo;

    // Start is called before the first frame update
    void Start()
    {
        InitializeSystems();
        //PlayerDetector.OnDetection += AttackPlayer;
        //PingSphere.DetectedPlayer += ChasePlayer;
    }

    public void AddSound(ActiveSound sound)
    {
        Debug.Log("Added Sounds");
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
            if (tryAttackPlayer)
            {
                AttackPlayer(playerPosRef);
            }
            base.Update();
            // Check if there is an active ping scanning for the player
            if (arrivedAtDestination)
            {
                if (chasingPlayer && tryAttackPlayer)
                {
                    PingSurroundings();
                    
                }
                else if (activeSounds.Count > 0)
                {
                    Prioritize();
                    activeSounds.RemoveAt(0);
                    PingSurroundings();
                }
                else if (activeSounds.Count == 0)
                {
                    Debug.Log("Reactivate Patrol");
                    active = false;
                }
            }
        }
    }

    void PingSurroundings()
    {
        pingSphere = Instantiate(Resources.Load("PingSphere", typeof(GameObject)), pingSource.position, transform.rotation) as GameObject;
        // if collision happens, this is quickly switched back on, if not it will stop the pinging process
    }

    // This is sort of a quick patch for an issue that causes the
    void AttackPlayer(Vector3 playerPos)
    {
        if ((playerPos - trans.position).magnitude <= damageRadius)
        {
            Debug.Log("Attack");
            tryAttackPlayer = false;
        }
    }

    public void ChasePlayer(Vector3 playerPos)
    {
        playerPosRef = playerPos;
        SetTarget(playerPosRef);
        chasingPlayer = true;
        tryAttackPlayer = true;
    }

    void AttackPlayer()
    {
        Debug.Log("Attacking");
    }
}