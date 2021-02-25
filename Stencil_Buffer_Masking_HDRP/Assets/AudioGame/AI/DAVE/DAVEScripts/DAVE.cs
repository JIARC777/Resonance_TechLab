using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
// We definetly want to remove this but I want to experiment with gameOver states with DAVE so for now DAVE has the power to restart the level
using UnityEngine.SceneManagement;
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
    // Reference where dave wants to head at any given moment
    Vector3 currentDestination;
    // Reference the real AI as opposed to whatever weird stuff we've written
    NavMeshAgent agent;
    // Store Sound Ids so that particles from the same sound are not called twice
    int[] soundIdHashes = new int[100];
    // Check whether or not DAVE has stopped at a given location - make sure to set correctly in states
    [HideInInspector]
    public bool waitingAtLocation = false;

    [Header("Patrol State")]
    // Set transform nodes in inspector
    public GameObject[] patrolPathNodes;
    // when Exiting Patrol, Patrol can access and set this as a marker, so it knows where to go when it starts patrolling again
    [HideInInspector]
    public int currentPatrolPathIndex;

    [Header("Engagement Parameters")]
    public float attackRadius = 0.1f;
    // How long do we "deactivate" DAVE after he attacks;
    public float postAttackWaitTime = 10f;
    // Whenever we get to determining how damage affects DAVE we can implement this flag to increase the speed and other factors - set to public in inspector for debugging/balancing 
    public bool AgroMode;
    // We need to move this but for simplicity I decrement this to create a game over state - how many times the player can get shot;
    public float playerHealth = 3f;
    // How fast does DAVE travel when he's angry
    public float AgroSpeedIncrease = 1f;
    // Switchs to update parameters easily inside update - should be related to when your toggle Agro mode
    bool engageAgro;
    bool disengageAgro;

    [Header("Pinging & Chasing")]
    public float pingRadius = 7f;
    [HideInInspector]
    // The chaser class needs to communicate with DAVE to know where to assign the agent's definition, so this is the point of access for that information
    public Vector3 lastKnownPlayerLocation;
    // This acts as a switch to tell an initialized DAVEChaser class to update its target position
    [HideInInspector]
    public bool pingFoundPlayer = false;
    // Reference to the current pinging object to subscribe to it's event
    GameObject currentPing;

    [Header("Investiagtion")]
    //this ignores the case where dave investigates each of the x number of sound emissions occuring in a relativly tight radius within a small time frame
    public float ignoreCloselyPackedSoundsRadius = 1.5f;
    // We need to know the position of the last noise to know whether or not the noise we just heard was from an area close enough to the old sound to be ignored
    Vector3 lastNoisePosition = Vector3.zero;

    [Header("Detectors")]
    // If we choose to go with a trigger based approach for detecting the player that extends past pings, assign the appropriate collider with a PlayerDetector script here so that we can subscribe to its event
    public PlayerDetector surroundingAreaDetector;
    
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

        // Check the distance between target to notify any listeners that DAVE has arrived - also check to make sure you are not waiting
        if ((currentDestination - transform.position).magnitude <= agent.stoppingDistance && !waitingAtLocation)
        {
            Debug.Log("Arrived");
            // DAVE is waiting, no need to tell anything DAVE arrived
            waitingAtLocation = true;
            // Call Event letting any states know destination has been reached
//             Debug.Log("Reached Destination");
             ArrivedAtDestination(this);
        }
        // Any state update code is run here
        currentState.UpdateCycle(this);
//      Debug.Log(currentState.GetType().ToString());
        
    }

    public void SetDestination(Vector3 newPointOfInterest)
	{
        // New destination incoming, no longer waiting - make sure to call set destination after any wait periods (I think we already do this)
        waitingAtLocation = false;
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
                //We've acknowledged the sound is a new sound
                ActiveSound soundData = noise.GetComponent<ActiveSound>();
                // Store the sound in the array
                soundIdHashes[noiseID % 100] = noiseID;
                //Debug.Log("Beware! D.A.V.E. heard you");
                // Check to make sure that the sound is not too close to the last sound we heard
                var bSoundTooCloseToLastSound = (soundData.soundLocation - lastNoisePosition).magnitude <= ignoreCloselyPackedSoundsRadius;
                lastNoisePosition = soundData.soundLocation;
                if (bSoundTooCloseToLastSound)
				{
                    // If the sound is too close to the last just exit the function
                    Debug.Log("Sound too close to last known sound to be different");
                    return;
				}
                // Debug.Log(currentState.GetType().ToString());
                // Check to see if the current State is not a DAVEInvestigator initialize state - This lets us add a transition case in DAVE itself while not creating a new state every time DAVE hears a sound
                var bNotInInvestigatorState = currentState.GetType().ToString() != "DAVEInvestigator";
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
        PingInfo.maxRadius = pingRadius;
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
        var bCanAttackPlayer = (this.transform.position - knownPlayerLocation).magnitude <= attackRadius;
        Debug.Log("Player Distance Away: " + (this.transform.position - knownPlayerLocation).magnitude);
        if (bCanAttackPlayer)
        {
            // Entering Attack Condition
            Debug.Log("Attack Player");
            playerHealth--;
            if (playerHealth <= 0)
			{
                RestartLevel();
			}
            StartCoroutine(PauseActionAfterAttack());
            
        }
        else
        {
            var bNotInChaseState = currentState.GetType().ToString() != "DAVEChaser";
            if (bNotInChaseState)
            {
                // Bit of a fun little bug I found here that explains alot of behavior we've seen - So we call exit on DAVEInvestigator in most cases which in turn by default activates a DAVEPatroller in the background referenced to the current state, so that is why we have a tendency to return to the patrol state all the time;
                // Since its initialized and no one ever calls exit on it before the reference to the current state is changed, whenever it becomes Initialized I assume it will stay initialized for the durration of the run
                currentState.Exit();
                // Check to assume we hopped into this hidden patrol state and call exit on the patroller
                if (currentState.GetType().ToString() == "DAVEPatroller")
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

    // We might have to implement a DAVEInvestigator style waiting system if we move this logic, but for now we can leverage a coroutine to pause DAVE after he attacks a player, giving them a chance to escape
    IEnumerator PauseActionAfterAttack()
	{
        yield return new WaitForSeconds(postAttackWaitTime);
        currentState = new DAVEPatroller();
        currentState.Initialize(this);
    }

    // remove this when we have a nicer player health system
    void RestartLevel()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
