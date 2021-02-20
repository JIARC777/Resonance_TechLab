using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
public interface IDaveState
{
    void Initialize(DAVE dave);
    void UpdateCycle(DAVE dave);
    void Exit();
}
public class DAVE : MonoBehaviour
{
    [HideInInspector]
    // Reference to the current state of Dave
    public IDaveState currentState;
    ///public DAVEData crossStateData;
    // Set transform nodes in inspector
    public GameObject[] patrolPathNodes;
    // when Exiting Patrol, Patrol can access and set this as a marker, so it knows where to go when it starts patrolling again
    [HideInInspector]
    public int currentPatrolPathIndex;
    NavMeshAgent agent;
    // Store Sound Ids so that particles from the same sound are not called twice
    int[] soundIdHashes = new int[100];
    Vector3 currentDestination;

    // Events
    // Called On OnTriggerEnter if tag == Player, indicating a player was found
    public delegate void PlayerDetected(Vector3 position);
    public event PlayerDetected DetectedPlayer;
    // Called On OnParticleCollsion
    public delegate void SoundHeard(ActiveSound suspiciousNoise);
    public event SoundHeard HeardNoise;
    // Called on how we want to handle damage players
    public delegate void Damaged(bool hitWithAMT);
    public event Damaged tookDamage;
    // Called when agent reaches destination
    public delegate void ReachedDestination(DAVE dave);
    public event ReachedDestination ArrivedAtDestination;
    // Start is called before the first frame update
    void Awake()
    {   
        currentDestination = Vector3.zero;
        agent = this.GetComponent<NavMeshAgent>();
        //crossStateData = new DAVEData();
        // Start DAVE off in a patrol mode
        currentState = new DAVEPatroller();
        currentState.Initialize(this);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log((currentDestination - transform.position).magnitude);
        //Debug.Log(agent.stoppingDistance);
        if ((currentDestination - transform.position).magnitude <= agent.stoppingDistance)
        {
//             Debug.Log("Reached Destination");
             ArrivedAtDestination(this);
        }
        currentState.UpdateCycle(this);
//        Debug.Log(currentState.GetType().ToString());
        
    }

    public void SetDestination(Vector3 newPointOfInterest)
	{
        Debug.Log("New Destination" + newPointOfInterest);
        currentDestination = newPointOfInterest;
        // Debug.Log(newPointOfInterest);
        agent.SetDestination(currentDestination);
	}

    
    public void OnParticleCollision(GameObject noise)
    {
        if (noise.tag == "Noise")
        {
            int noiseID = noise.GetComponent<ActiveSound>().Id;
            // Do a mini hash to store the id into a small array with 0(1) search while allowing a check of larger numbers
            if (soundIdHashes[noiseID % 100] != noiseID)
            {
                ActiveSound soundData = noise.GetComponent<ActiveSound>();
                soundIdHashes[noiseID % 100] = noiseID;
                //Debug.Log("Beware! D.A.V.E. heard you");
                
                // Check to see if the current State is not a DAVEInvestigator initialize state - This lets us add a transition case in DAVE itself while not creating a new state every time DAVE hears a sound
                var bNotInInvestigatorState = currentState.GetType().ToString() != "DAVEInvestigator";
                Debug.Log(currentState.GetType().ToString());
                if (bNotInInvestigatorState)
                {
                    currentState.Exit();
                    //Debug.Log("Changing State to Investigator");
                    // Its worth noting now that a new investigator is created each time it transitions, so any old sounds are ignorned. 
                    //This shouldnt change much because the exit condition for the investigator is to essentially clear the list or get distracted by finding the player
                    currentState = new DAVEInvestigator();
                    currentState.Initialize(this);
                }
                // Call the event (after transitioning, so the investigator can pick up the event call)
                HeardNoise(soundData);
            }
        }
    }
    

}
