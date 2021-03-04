using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
// We definetly want to remove this but I want to experiment with gameOver states with DAVE so for now DAVE has the power to restart the level
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

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
    [HideInInspector] public bool waitingAtLocation = false;

    // reference to the starting height we can use to LERP DAVE back to position after being deactivated 
    float startingYHeight;

    [Header("Model Specific Dependancies")]
    // Reference the transform of the model itself
    public Transform modelTransform;

    public Transform beamGunTip;

    // reference the rigidbody of the model
    public Rigidbody modelRB;

    [Header("Patrol State")]
    // Set transform nodes in inspector
    public GameObject[] patrolPathNodes;

    // when Exiting Patrol, Patrol can access and set this as a marker, so it knows where to go when it starts patrolling again
    [HideInInspector] public int currentPatrolPathIndex;


    [Header("Engagement Parameters")] public float attackRadius = 0.1f;

    // How long do we "deactivate" DAVE after he attacks;
    public float postAttackWaitTime = 10f;

    private float attackTimestamp = 0f;

    // We need to move this but for simplicity I decrement this to create a game over state - how many times the player can get shot;
    public float playerHealth = 3f;

    // Since we rely on Triggers, which can sometimes get called more than once, we need to make sure that we know if the player has been engaged or not to ignore any extra event calls
    bool engagedPlayer = false;
    // Switchs to update parameters easily inside update - should be related to when your toggle Agro mode

    // Place where we want the ray to detect the player to fire from. 


    //TODO: To be implemented post V/S
    // bool engageAgro;
    // bool disengageAgro;
    // How fast does DAVE travel when he's angry
    // public float AgroSpeedIncrease = 1f;
    // Whenever we get to determining how damage affects DAVE we can implement this flag to increase the speed and other factors - set to public in inspector for debugging/balancing 
    // public bool AgroMode;


    [Header("Pinging & Chasing")] public float pingRadius = 7f;

    [HideInInspector]
    // The chaser class needs to communicate with DAVE to know where to assign the agent's definition, so this is the point of access for that information
    public Vector3 lastKnownPlayerLocation;

    // This acts as a switch to tell an initialized DAVEChaser class to update its target position
    [HideInInspector] public bool pingFoundPlayer = false;

    // Reference to the current pinging object to subscribe to it's event
    GameObject currentPing;
    private float timeSinceLastPing = 0;
    private float nearbySoundTimeout = 5F;


    [Header("Investiagtion")]
    //this ignores the case where dave investigates each of the x number of sound emissions occuring in a relativly tight radius within a small time frame
    public float ignoreCloselyPackedSoundsRadius = 1.5f;

    // We need to know the position of the last noise to know whether or not the noise we just heard was from an area close enough to the old sound to be ignored
    Vector3 lastNoisePosition = Vector3.zero;


    [Header("Detectors & Taking Damage")]
    // Assign trigger sphere that is meant to detect the player at close range - ideally should be set slightly higher than attack radius 
    public PlayerDetector closeProximityDetector;

    // Assign the trigger box marked AMTTriggerZone, so we can sub to it's trigger event and know if the player has shot dave
    public OnAMTTrigger daveWeakSpot;

    // Assign Dave's model to this slot, making sure it has an OnDavePhysicsImpact assocaited
    public OnDavePhysicsImpact daveModelCollider;

    // How long do we wait after being "quietly" deactivated
    public float weakSpotDeactivatedWaitTime = 30f;

    // How long do we wait after being hit by a physics object
    public float physicsImpactDeactivationWaitTime = 5f;

    // How long do we wait after being hit by a projectile
    public float projectileImpactDeactivationWaitTime = 100f;

    // DAVE has taken some form of damage
    bool damaged;

    bool needToLERPModel;

    // How fast Lerping goes;
    float lerpSpeed = 0.1f;
    bool isStopped = false;

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
        startingYHeight = modelTransform.position.y;
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
        if ((currentDestination - transform.position).magnitude <= agent.stoppingDistance && !waitingAtLocation && !isStopped)
        {
            Debug.Log("Arrived");
            
            // DAVE is waiting, no need to tell anything DAVE arrived
            waitingAtLocation = true;
            
            // Call Event letting any states know destination has been reached
//             Debug.Log("Reached Destination");
            ArrivedAtDestination?.Invoke(this);
            
        }
        if (needToLERPModel)
        {
            LERPModelBackToCenter();
        }

        if (currentState != null)
            currentState.UpdateCycle(this);
        //
        var bIsWaitingOnAttackAnimation = Time.time > attackTimestamp + postAttackWaitTime;
//        if(bIsWaitingOnAttackAnimation)
//            
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


    // Let any states ping for surroundings with public function

    public void PingSurroundings()
    {
        currentPing = Instantiate(Resources.Load("PingSphere", typeof(GameObject)),
            transform.position + new Vector3(0, 2, 0), transform.rotation) as GameObject;
        PingSphere PingInfo = currentPing.GetComponent<PingSphere>();
        PingInfo.maxRadius = pingRadius;
        // Immediately sub to the Ping's event in case of collision - assume unsub on destroyed gameobject
        PingInfo.DetectedPlayer += EngagePlayer;
        // if collision happens, this is quickly switched back on, if not it will stop the pinging process
    }


    // Fires on event, player detected by ping

    void EngagePlayer(Vector3 knownPlayerLocation)
    {
        if (!engagedPlayer)
        {
            engagedPlayer = true;
            // Update Drone's last known player position
            lastKnownPlayerLocation = knownPlayerLocation;
            // Even though attack is handled by the DAVE script itself, we dont want any other states trying to tell DAVE what to do. This is a bit of a weird "No State" situation. We assume the temporarilyDeactivateProcessing couroutine will re-intialized a patroller after the proper waiting time;
            ExitCurrentState();
            // Before chasing, check if we are close enough to attack
            var bCanAttackPlayer = (this.transform.position - knownPlayerLocation).magnitude <= attackRadius;
            // Debug.Log("Player Distance Away: " + (this.transform.position - knownPlayerLocation).magnitude);
            if (bCanAttackPlayer)
            {
                //Can attack player right now, do so

                currentState = null;
                AttackPlayer();
                StartCoroutine(temporarilyDeactivateProcessing(postAttackWaitTime));
            }
            else
            {
                //Can't attack player right now, must move to them

                var bNotInChaseState = currentState.GetType().ToString() != "DAVEChaser";
                if (bNotInChaseState)
                {
                    //Must transition to chaser state to go to player location
                    // Bit of a fun little bug I found here that explains alot of behavior we've seen - So we call exit on DAVEInvestigator in most cases which in turn by default activates a DAVEPatroller in the background referenced to the current state, so that is why we have a tendency to return to the patrol state all the time;
                    // Since its initialized and no one ever calls exit on it before the reference to the current state is changed, whenever it becomes Initialized I assume it will stay initialized for the durration of the run
                    currentState = new DAVEChaser();
                    currentState.Initialize(this);

                    pingFoundPlayer = false;
                }
                else
                {
                    //Currently in chase state
                    //This re-ping found player, inform chaser to pathfind
                    pingFoundPlayer = true;
                }
            }
        }
    }

    // Whenever we want to pause DAVE's processing, we can call this script and pass various related different wait times such as post attack waiting and deactivating times

    IEnumerator temporarilyDeactivateProcessing(float waitTime)
    {
        attackTimestamp = Time.time;
        Debug.Log("<color=cyan>Deactivation Cooldown Start</color>");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("<color=cyan>Deactivation Cooldown Complete</color>");
        if (currentState != null)
            currentState.Exit();
        currentState = new DAVEPatroller();
        currentState.Initialize(this);
        engagedPlayer = false;
    }
    
    IEnumerator deactivateAndReattachDave(float waitTime)
    {
        attackTimestamp = Time.time;
        Debug.Log("<color=cyan>Deactivation Cooldown Start</color>");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("<color=cyan>Deactivation Cooldown Complete</color>");
        if (currentState != null)
            currentState.Exit();
        // currentState = new DAVEPatroller();
        // currentState.Initialize(this);
        // engagedPlayer = false;
        needToLERPModel = true;
        modelRB.isKinematic = true;
        modelRB.useGravity = false;
    }


    // Call this function when dave needs to be quietly deactivated for the given amount of time;

    void quietDeactivate()
    {
        // We deactivated dave for a tiny bit - keep him in the air, just freeze his location - maybe change lighting or material state
        temporarilyDeactivateProcessing(weakSpotDeactivatedWaitTime);
    }

    void loudDeactivate(bool hardImpact)
    {
        ExitCurrentState();

        // Stop navmesh here
        // SetDestination(transform.position);
        //Disable the navmesh agent
        agent.enabled = false;
        
        // unparent the model
       // modelTransform.parent = null;

        // deactivate kinematic so we can use physics
        modelRB.isKinematic = false;

        // Let the update checking for arrival at destination know to call the function to lerp the model.
        // damaged = true;
        // deactivate for the proper amount of time;
        // This is a bit weird, but I dont want any events interfering with being damaged, especially the player running into the detector box, which should be "deactivated"
        closeProximityDetector.DetectedPlayer -= EngagePlayer;
        if (hardImpact)
        {
            modelRB.useGravity = true;
            StartCoroutine(deactivateAndReattachDave(physicsImpactDeactivationWaitTime));
            // Make the NavMesh move to the model
        }
        else
        {
            StartCoroutine(temporarilyDeactivateProcessing(physicsImpactDeactivationWaitTime));
        }

        SetDestination(modelTransform.position);
        // If we want an agro mode toggle it would likely be here 
    }

    void AttackPlayer()
    {
        // We need a reference to the player's position
        Debug.Log("<color=Red> Attacking player</color>");
        // Find the player, get the position, add a bit of extra height assuming its floor position 
        // If we find that this static height leads to issues, we can look for a way to implement dynamic height

        Vector3 playerTargetPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position +
                                  new Vector3(0, 1.3f, 0);
        Debug.Log("Player's Location: " + playerTargetPos);
        SetDestination(playerTargetPos);
        Debug.DrawLine(beamGunTip.position, playerTargetPos, Color.red);
        // Add an actual object and/or line renderer ^
        playerHealth--;
        if (playerHealth <= 0)
        {
            RestartLevel();
        }
    }

    void LERPModelBackToCenter()
    {
        //Debug.Log("LERPing");
        // Assuming that we've parented correctly, we just need to fly to local position 0
        Debug.Log(startingYHeight);
        modelTransform.localPosition = Vector3.Lerp(modelTransform.position, new Vector3(0, startingYHeight, 0), Time.deltaTime * lerpSpeed);
        modelTransform.localRotation = Quaternion.Slerp(modelTransform.localRotation, Quaternion.identity, Time.deltaTime * lerpSpeed);


        // This essentially stops this function from being called;
        if ((modelTransform.localPosition - new Vector3(0, startingYHeight, 0)).magnitude <= 0.1f)
        {
            needToLERPModel = false;
            Debug.Log("Model Moved To Correct Position");
            // Now that we are safely reactivated, this listener can be reinitialized
            closeProximityDetector.DetectedPlayer += EngagePlayer;
            agent.enabled = true;
        }
    }


    // If we could move this to an event system that would be nice, but I am having issues with collision layers

    void ExitCurrentState()
    {
        if (currentState != null)
        {
            currentState.Exit();
            if (currentState.GetType().ToString() == "DAVEPatroller")
                currentState.Exit();
        }
    }
    
    
    #region Collisions

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit Dave");
        if (collision.gameObject.tag == "Projectile")
        {
            Debug.Log("Projectile hit DAVE");
            // Call the event, set hardImpact bool to true so that DAVE knows to deactivate himself
            // SomethingHitDAVE(true);
            loudDeactivate(true);
        }

        if (collision.gameObject.tag == "PhysicsObj")
        {
            Debug.Log("You threw something at DAVE");
            // Call the event, set hardImpact bool to false so that DAVE might just stops for a quick second 
            loudDeactivate(false);
        }
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

                var bSoundTooCloseToLastSound = (soundData.soundLocation - lastNoisePosition).magnitude <=
                                                ignoreCloselyPackedSoundsRadius;
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
    
    #endregion Collisions

    
    
    /// <summary>
    /// Player has died and the level should restart
    /// </summary>
    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}