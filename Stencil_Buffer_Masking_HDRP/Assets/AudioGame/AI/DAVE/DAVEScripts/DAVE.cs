using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
public interface IDaveState
{
    void Initialize(DAVE dave);
    //At some point we should verify that a local reference in each state (thisDave) makes a reference in the updateCycle unecessary
    void UpdateCycle(DAVE dave);
    void Exit();
}
public class DAVE : MonoBehaviour
{
    [HideInInspector]
    // Reference to the current state of Dave
    public IDaveState currentState;
    // Set transform nodes in inspector
    public GameObject[] patrolPathNodes;
    // How far do you want a ping to travel
    // public float pingRadius = 7f;
    public float attackRadius = 3f;
    // How far is AI willing to travel before
    public float chaseRadius = 5F;
    // when Exiting Patrol, Patrol can access and set this as a marker, so it knows where to go when it starts patrolling again
    [HideInInspector]
    public int currentPatrolPathIndex;
    NavMeshAgent agent;
    // Store Sound Ids so that particles from the same sound are not called twice
    int[] soundIdHashes = new int[100];
    Vector3 currentDestination;
    [HideInInspector]
    public Vector3 lastKnownPlayerLocation;

    // This acts as a switch to tell an initialized DAVEChaser class to update its target position
    public bool pingFoundPlayer = false;
    GameObject currentPing;

    // Events
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
            // Call Event letting any states know destination has been reached
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
    
    // Let any states ping for surroundings with public function
    public void PingSurroundings()
    {
        currentPing = Instantiate(Resources.Load("PingSphere", typeof(GameObject)), transform.position + new Vector3(0, 2, 0), transform.rotation) as GameObject;
        PingSphere PingInfo = currentPing.GetComponent<PingSphere>();
       // PingInfo.maxRadius = pingRadius;
        // Immediately sub to the Ping's event in case of collision - assume unsub on destroyed gameobject
        PingInfo.DetectedPlayer += EngagePlayer;
        // if collision happens, this is quickly switched back on, if not it will stop the pinging process
    }

    // We should 
    void EngagePlayer(Vector3 knownPlayerLocation)
    {
        // Update Drone's last known player position
        lastKnownPlayerLocation = knownPlayerLocation;
        // Before chasing, check if we are close enough to attack
        if ((knownPlayerLocation - this.transform.position).magnitude <= attackRadius)
        {
            // Entering Attack Condition
            Debug.Log("Attack Player");
        }
        else
        {
            var bNotInChaseState = currentState.GetType().ToString() != "DAVEChaser";
            if (bNotInChaseState)
            {
                currentState.Exit();
                currentState = new DAVEChaser();
                currentState.Initialize(this);
                
                pingFoundPlayer = false;
            }
            else
            {
                // pingFoundPlayer is not set to true until after state is initialized to avoid confusion with initialization function which by default sets the target
                pingFoundPlayer = true;
            }
        }
        
    }

}
