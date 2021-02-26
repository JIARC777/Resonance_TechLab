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
    // Have dave stop
    public bool lockDownMode = true;
    // reference to the starting height we can use to LERP DAVE back to position after being deactivated 
    private float startingYHeight;
    
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

    private float attackTimestamp = 0f;
    // We need to move this but for simplicity I decrement this to create a game over state - how many times the player can get shot;
    public float playerHealth = 3f;
    // Switchs to update parameters easily inside update - should be related to when your toggle Agro mode
    

    //TODO: To be implemented post V/S
    // bool engageAgro;
    // bool disengageAgro;
    // How fast does DAVE travel when he's angry
    // public float AgroSpeedIncrease = 1f;
    // Whenever we get to determining how damage affects DAVE we can implement this flag to increase the speed and other factors - set to public in inspector for debugging/balancing 
    // public bool AgroMode;

    

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
    private float timeSinceLastPing = 0;
    private float nearbySoundTimeout = 5F;

    
    
    [Header("Investiagtion")]
    //this ignores the case where dave investigates each of the x number of sound emissions occuring in a relativly tight radius within a small time frame
    public float ignoreCloselyPackedSoundsRadius = 1.5f;
    // We need to know the position of the last noise to know whether or not the noise we just heard was from an area close enough to the old sound to be ignored
    Vector3 lastNoisePosition = Vector3.zero;

    
    [Header("Detectors")]
    // Assign trigger sphere that is meant to detect the player at close range - ideally should be set slightly higher than attack radius 
    public PlayerDetector closeProximityDetector;
    // Assign the trigger box marked AMTTriggerZone, so we can sub to it's trigger event and know if the player has shot dave
    public OnAMTTrigger daveWeakSpot;
    // Assign Dave's model to this slot, making sure it has an OnDavePhysicsImpact assocaited
    public OnDavePhysicsImpact daveModelCollider;

    
    // Events
    #region Events
    // Called On OnParticleCollsion
    public delegate void SoundHeard(ActiveSound suspiciousNoise);
    public event SoundHeard HeardNoise;
    
    // Called on how we want to handle damage players
    public delegate void Damaged(bool hitWithAMT);
    public event Damaged tookDamage;
    
    // Called when agent reaches destination
    public delegate void ReachedDestination(DAVE dave);
    public event ReachedDestination ArrivedAtDestination;
    
    // Called when agent reaches destination
    public delegate void PingExit(DAVE dave);
    public event PingExit PingExited;
    
    #endregion
    
    // Start is called before the first frame update
    void Awake()
    {
        // Subscribe to the triggering Events living on different DAVE children
        // Sub to weak spot monitor
        daveWeakSpot.amtHitWeakSpot += quietDeactivate;
        // Sub to General Physics collisions on the DAVE collider
        daveModelCollider.SomethingHitDAVE += loudDeactivate;
        // Sub The event that triggers the chase/attack sequence to the proximity detector 
        closeProximityDetector.DetectedPlayer += EngagePlayer;
        
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
        if ((currentDestination - transform.position).magnitude <= agent.stoppingDistance && !waitingAtLocation && !lockDownMode)
        {
            Debug.Log("Arrived");
            // DAVE is waiting, no need to tell anything DAVE arrived
            waitingAtLocation = true;
            // Call Event letting any states know destination has been reached
//             Debug.Log("Reached Destination");
             ArrivedAtDestination(this);
        }
        
        //
        var bIsWaitingOnAttackAnimation = Time.time > attackTimestamp + postAttackWaitTime;
//        if(bIsWaitingOnAttackAnimation)
//            currentState.UpdateCycle(this);
//      Debug.Log(currentState.GetType().ToString());
        
    }

    public void SetDestination(Vector3 newPointOfInterest)
	{
        // New destination incoming, no longer waiting - make sure to call set destination after any wait periods (I think we already do this)
        waitingAtLocation = false;
        // Debug.Log("New Destination" + newPointOfInterest);
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

                var bCloseSoundHasTimedOut = Time.time > timeSinceLastPing + nearbySoundTimeout;
                if (bCloseSoundHasTimedOut)
                {
                    lastNoisePosition = Vector3.positiveInfinity;
                }
                
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
                HeardNoise?.Invoke(soundData);
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

    
    
    
    // Fires on event, player detected by ping
    void EngagePlayer(Vector3 knownPlayerLocation)
    {
        // Update Drone's last known player position
        lastKnownPlayerLocation = knownPlayerLocation;
        // Before chasing, check if we are close enough to attack
        var bCanAttackPlayer = (this.transform.position - knownPlayerLocation).magnitude <= attackRadius;
        // Debug.Log("Player Distance Away: " + (this.transform.position - knownPlayerLocation).magnitude);
        if (bCanAttackPlayer)
        {//Can attack player right now, do so
            // Entering Attack Condition
            // Debug.Log("Attack Player");
            playerHealth--;
            if (playerHealth <= 0)
			{
                RestartLevel();
			}
            StartCoroutine(PauseActionAfterAttack());
            
        }
        else
        { //Can't attack player right now, must move to them
            var bNotInChaseState = currentState.GetType().ToString() != "DAVEChaser";
            if (bNotInChaseState)
            {//Must transition to chaser state to go to player location
                // Bit of a fun little bug I found here that explains alot of behavior we've seen - So we call exit on DAVEInvestigator in most cases which in turn by default activates a DAVEPatroller in the background referenced to the current state, so that is why we have a tendency to return to the patrol state all the time;
                // Since its initialized and no one ever calls exit on it before the reference to the current state is changed, whenever it becomes Initialized I assume it will stay initialized for the durration of the run
                currentState.Exit();
                ///FIX: Edge case, if exiting investigator, it automatically starts a patroller, exit that too
                if (currentState.GetType().ToString() == "DAVEPatroller")
                    currentState.Exit();

                currentState = new DAVEChaser();
                currentState.Initialize(this);

                pingFoundPlayer = false;
            }
            else
            {//Currently in chase state
                //This re-ping found player, inform chaser to pathfind
                pingFoundPlayer = true;
            }
        }
        
    }

    // We might have to implement a DAVEInvestigator style waiting system if we move this logic, but for now we can leverage a coroutine to pause DAVE after he attacks a player, giving them a chance to escape
    /// <summary>
    /// DAVE is attacking the player, wait for him to finish then exit to patroller
    /// </summary>
    /// <returns></returns>
    IEnumerator PauseActionAfterAttack()
    {
        attackTimestamp = Time.time;
        Debug.Log("<color=yellow> Attack time happens now</color>");
        yield return new WaitForSeconds(postAttackWaitTime);
        currentState.Exit();
        currentState = new DAVEPatroller();
        currentState.Initialize(this);
    }

    
    /// <summary>
    /// Player has died and the level should restart
    /// </summary>
    void RestartLevel()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

    // Call this function when dave needs to be quietly deactivated for the given amount of time;
    void quietDeactivate()
    {
        // We deactivated dave for a tiny bit - keep him in the air, just freeze his location - maybe change lighting or material state
    }

    void loudDeactivate()
    {
        // Dave was just hit with a projectile (or physics object?)
        // Turn off kinematics on his rigidbody, wait, then lerp to initial height/position
        // If we want an agro mode toggle it would likely be here 
    }
    
    
}
