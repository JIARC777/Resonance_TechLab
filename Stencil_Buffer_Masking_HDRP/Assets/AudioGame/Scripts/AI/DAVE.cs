using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public interface IDaveState
{
    void StateEnter(DAVE dave);

    //At some point we should verify that a local reference in each state (thisDave) makes a reference in the updateCycle unecessary
    void StateUpdate();
    void StateExit();
}

public class DAVE : MonoBehaviour
{
    #region Fields - BigDave

    [HideInInspector]
    // Reference to the current state of Dave
    public IDaveState currentState;

    // Reference the real AI as opposed to whatever weird stuff we've written
    public NavMeshAgent agent;

    // Reference where dave wants to head at any given moment
    Vector3 currentDestination;

    // reference to the starting height we can use to LERP DAVE back to position after being deactivated 
    float startingYHeight;

    #endregion Fields - BigDave


    #region Fields - Sound Detection

    // Store Sound Ids so that particles from the same sound are not called twice
    int[] soundIdHashes = new int[100];

    #endregion Fields - Sound Detection


    #region Fields - Weapon References

    /// <summary>
    /// Weapon positions & references
    /// </summary>
    [Header("Model Connections")]
    // Reference the transform of the model itself
    public Transform daveModelTransform;


    public Transform gunPivot;

    // reference the rigidbody of the model
    public Rigidbody modelRB;


    public ParticleSystem gunFX;

    #endregion Fields - Weapon References


    #region Fields - NavMeshAgent / AI

    [Header("Patrol State")]
    // Set transform nodes in inspector
    public GameObject[] patrolPathNodes;

    // when Exiting Patrol, Patrol can access and set this as a marker, so it knows where to go when it starts patrolling again
    [HideInInspector] public int currentPatrolPathIndex;

    // Check whether or not DAVE has stopped at a given location - make sure to set correctly in states
    [HideInInspector] public bool waitingAtLocation = false;

    #endregion Fields - NavMeshAgent / AI


    #region Fields - Attack Timings

    [Header("Engagement Parameters")] public float attackRadius = 0.1f;

    // How long do we "deactivate" DAVE after he attacks;
    public float postAttackWaitTime = 10f;

    // Since we rely on Triggers, which can sometimes get called more than once, we need to make sure that we know if the player has been engaged or not to ignore any extra event calls
    private float attackTimestamp = 0f;

    private float attackPlayerDelaySeconds = 0.6f;
    public float agroSpeedBoost = 1.5F;
    public bool isAgro = false;

    #endregion Fields - Attack Timings

    /// <summary>
    /// Ping stats & chasing info
    /// </summary>

    #region Pinging & Chasing

    [Header("Pinging & Chasing")]

    // The chaser class needs to communicate with DAVE to know where to assign the agent's definition, so this is the point of access for that information
    [HideInInspector]
    public Vector3 lastKnownPlayerLocation;

    public bool engagedPlayer = false;


    // This acts as a switch to tell an initialized DAVEChaser class to update its target position
    [HideInInspector] public bool pingFoundPlayer = false;

    // Reference to the current pinging object to subscribe to it's event
    GameObject currentPing;
    private float timeSinceLastPing = 0;
    private float nearbySoundTimeout = 5F;

    #endregion Pinging & Chasing

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
    //public OnDavePhysicsImpact daveModelCollider;

    // How long do we wait after being "quietly" deactivated
    public float weakSpotDeactivatedWaitTime = 30f;

    // How long do we wait after being hit by a physics object
    public float physicsImpactDeactivationWaitTime = 5f;

    // How long do we wait after being hit by a projectile
    public float projectileImpactDeactivationWaitTime = 100f;

    [Header("Light Indicators")] public Light statusLight;
    public Color patrolModeColor;
    public Color investigationModeColor;
    public Color chaserModeColor;
    public Color AttackModeColor;
    public Color deactivateModeColor;

    // DAVE has taken some form of damage
    bool damaged;

    bool needToLERPModel;

    // How fast Lerping goes;
    float lerpSpeed = 1f;
    bool isStopped = false;

    [Header("Audio")] public AudioSource ping;
    public AudioSource daveHeardPlayer;
    public AudioSource daveHeardNoise;
    public AudioSource fireAtPlayer;
    public AudioSource hoverSoundFX;

    // Events

    #region Events

    // Called On OnParticleCollsion
    public delegate void SoundHeard(ActiveSound suspiciousNoise);

    public event SoundHeard HeardNoise;

    // Called on how we want to handle damage players
    public delegate void Damaged(bool hitWithAMT);


    // Called when agent reaches destination
    public delegate void ReachedDestination(DAVE dave);

    public event ReachedDestination ArrivedAtDestination;

    // Called when agent reaches destination
    public delegate void PingExit(DAVE dave);

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        // Subscribe to the triggering Events living on different DAVE children
        // Sub to weak spot monitor
        daveWeakSpot.amtHitWeakSpot += BeamHitWeakPointDeactivate;
        // Sub to General Physics collisions on the DAVE collider
        // daveModelCollider.SomethingHitDAVE += loudDeactivate;
        // Sub The event that triggers the chase/attack sequence to the proximity detector 
        closeProximityDetector.DetectedPlayer += EngagePlayer;

        //SoundFX init
        hoverSoundFX.loop = true;
        hoverSoundFX.Play();

        //NavMesh init
        agent = this.GetComponent<NavMeshAgent>();
        agent.enabled = true;
        currentDestination = Vector3.zero;
        startingYHeight = daveModelTransform.position.y;


        //First state init
        currentState = new DAVEPatroller();
        currentState.StateEnter(this);
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
            ArrivedAtDestination?.Invoke(this);
        }

        if (needToLERPModel)
        {
            LERPModelBackToCenter();
        }

        if (currentState != null && !damaged)
            currentState.StateUpdate();
        //
        var bIsWaitingOnAttackAnimation = Time.time > attackTimestamp + postAttackWaitTime;
//        if(bIsWaitingOnAttackAnimation)
//            
//      Debug.Log(currentState.GetType().ToString());
    }

    public void SetDestination(Vector3 newPointOfInterest)
    {
        agent.enabled = true;

        // New destination incoming, no longer waiting - make sure to call set destination after any wait periods (I think we already do this)
        waitingAtLocation = false;
        Debug.Log("New Destination" + newPointOfInterest);
        currentDestination = newPointOfInterest;
        Debug.Log(newPointOfInterest);
        agent.SetDestination(currentDestination);
    }


    // Let any states ping for surroundings with public function

    public void PingSurroundings()
    {
        Debug.Log("Ping");
        PlayAudio(ping);
        currentPing = Instantiate(Resources.Load("PingSphere", typeof(GameObject)),
            daveModelTransform.position + new Vector3(0, 0, 0.25f), transform.rotation) as GameObject;
        PingSphere PingInfo = currentPing.GetComponent<PingSphere>();

        // Immediately sub to the Ping's event in case of collision - assume unsub on destroyed gameobject
        PingInfo.DetectedPlayer += EngagePlayer;
        // if collision happens, this is quickly switched back on, if not it will stop the pinging process
    }


    // Fires on event, player detected by ping


    // Whenever we want to pause DAVE's processing, we can call this script and pass various related different wait times such as post attack waiting and deactivating times
    void EngagePlayer(Vector3 knownPlayerLocation)
    {
        lastKnownPlayerLocation = knownPlayerLocation;
        
        // Before chasing, check if we are close enough to attack
        var bCanAttackPlayer = (this.transform.position - knownPlayerLocation).magnitude <= attackRadius;
        Debug.Log(bCanAttackPlayer);
        // Debug.Log("Player Distance Away: " + (this.transform.position - knownPlayerLocation).magnitude);
        if (bCanAttackPlayer)
        {
            //Can attack player right now, do so
            ExitCurrentState();
            currentState = null;
            StartCoroutine(AttackPlayer());
        }
        else
        {
            Debug.Log("Entering Chase Phase");
            //Can't attack player right now, must move to them

            var bNotInChaseState = currentState != null && currentState.GetType().ToString() != "DAVEChaser";
            Debug.Log(bNotInChaseState);
            if (bNotInChaseState)
            {
                agent.speed += agroSpeedBoost;
                //Must transition to chaser state to go to player location
                // Bit of a fun little bug I found here that explains alot of behavior we've seen - So we call exit on DAVEInvestigator in most cases which in turn by default activates a DAVEPatroller in the background referenced to the current state, so that is why we have a tendency to return to the patrol state all the time;
                // Since its initialized and no one ever calls exit on it before the reference to the current state is changed, whenever it becomes Initialized I assume it will stay initialized for the durration of the run
                ExitCurrentState();
                currentState = new DAVEChaser();
                currentState.StateEnter(this);
                PlayAudio(daveHeardPlayer);
                pingFoundPlayer = false;
            }
            else
            {
                Debug.Log("Notify Chaser: Ping found player");
                //Currently in chase state
                //This re-ping found player, inform chaser to pathfind
                pingFoundPlayer = true;
            }
        }
    }

    IEnumerator temporarilyDeactivateProcessing(float waitTime)
    {
        
        hoverSoundFX.Pause();
        statusLight.color = deactivateModeColor;
        damaged = true;
        attackTimestamp = Time.time;
        agent.enabled = false;
        Debug.Log("<color=cyan>Deactivation Cooldown Start</color>");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("<color=cyan>Deactivation Cooldown Complete</color>");
        hoverSoundFX.Play();
        agent.enabled = true;
        engagedPlayer = false;
        damaged = false;

        ExitCurrentState();
        currentState = new DAVEPatroller();
        currentState.StateEnter(this);
    }

    IEnumerator deactivateAndReattachDave(float waitTime)
    {
        statusLight.color = deactivateModeColor;
        hoverSoundFX.Pause();
        damaged = true;
        attackTimestamp = Time.time;
        Debug.Log("<color=cyan>Deactivation Cooldown Start</color>");
        yield return new WaitForSeconds(waitTime);
        Debug.Log("<color=cyan>Deactivation Cooldown Complete</color>");
        hoverSoundFX.Play();

        ExitCurrentState();

        engagedPlayer = false;
        damaged = false;
        needToLERPModel = true;
        modelRB.isKinematic = true;
        modelRB.useGravity = false;
        
        // Dave is angry now that you shot him
        isAgro = true;
        if (agent.speed < 3.5)
            agent.speed += agroSpeedBoost;
    }


    void BeamHitWeakPointDeactivate()
    {
        // Debug.Log("Deactivated with Beam");
        // We deactivated dave for a tiny bit - keep him in the air, just freeze his location - maybe change lighting or material state
        StartCoroutine(temporarilyDeactivateProcessing(weakSpotDeactivatedWaitTime));
    }


    void ProjectileImpactDeactivate(bool projectileImpact)
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


        // Unsub sphere trigger from event
        // This is a bit weird, but I dont want any events interfering with being damaged, especially the player running into the detector box, which should be "deactivated"
        closeProximityDetector.DetectedPlayer -= EngagePlayer;
        if (!damaged)
        {
            if (projectileImpact)
            {
                modelRB.useGravity = true;
                StartCoroutine(deactivateAndReattachDave(physicsImpactDeactivationWaitTime));
            }
            else
            {
                StartCoroutine(deactivateAndReattachDave(physicsImpactDeactivationWaitTime));
            }
        }
    }

    #region attack

    public IEnumerator AttackPlayer()
    {
        Debug.Log("<color=Red> Attacking player</color>");

        //Go to player and change color
        SetDestination(this.transform.position); //+ new Vector3(-1, 0, -1));
        statusLight.color = AttackModeColor;

        yield return new WaitForSeconds(attackPlayerDelaySeconds);

        //Play FX for attack
        ResonancePlayerHealthRespawn.instance.DamagePlayer();
        PlayAudio(fireAtPlayer);
        var playerTargetPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position + new Vector3(0, .7f, 0);
        FireGun(playerTargetPos);

        //Delay after attack
        StartCoroutine(temporarilyDeactivateProcessing(postAttackWaitTime));
    }

    void FireGun(Vector3 targetPos)
    {
        Debug.Log("Gun Fired");
        gunPivot.transform.LookAt(targetPos);
        gunFX.Emit(1000);
    }

    #endregion

    void LERPModelBackToCenter()
    {
        daveModelTransform.position = Vector3.Lerp(daveModelTransform.position, new Vector3(transform.position.x, startingYHeight, transform.position.z), Time.deltaTime * lerpSpeed);
        daveModelTransform.localRotation = Quaternion.Slerp(daveModelTransform.localRotation, Quaternion.identity, Time.deltaTime * lerpSpeed);

        // We've Reached the correct position - stop lerping;
        var bDaveBackAtRoot = (daveModelTransform.localPosition - new Vector3(0, startingYHeight, 0)).magnitude <= 0.25f;
        if (bDaveBackAtRoot)
        {
            needToLERPModel = false;
            // Now that we are safely reactivated, this listener can be reinitialized
            closeProximityDetector.DetectedPlayer += EngagePlayer;
            agent.enabled = true;
            damaged = false;

            ExitCurrentState();
            currentState = new DAVEPatroller();
            currentState.StateEnter(this);
        }
    }


    // Because of our strange transition patterns this makes it easy to ensure we dont enter a situation where more than one state is running
    void ExitCurrentState()
    {
        if (currentState != null)
        {
            currentState.StateExit();
            if (currentState.GetType().ToString() == "DAVEPatroller")
                currentState.StateExit();
        }
    }


    #region Collisions

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            Debug.Log("Projectile hit DAVE");
            // Call the event, set hardImpact bool to true so that DAVE knows to deactivate himself
            // SomethingHitDAVE(true);
            ProjectileImpactDeactivate(true);
        }

        if (collision.gameObject.tag == "PhysicsObj")
        {
            Debug.Log("You threw something at DAVE");
            // Call the event, set hardImpact bool to false so that DAVE might just stops for a quick second 
            ProjectileImpactDeactivate(false);
        }
    }

    public void OnParticleCollision(GameObject noise)
    {
        if (noise.tag == "Noise" && !damaged)
        {
            //Debug.Log("Sound Particles Detected");
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
                var bNotInInvestigatorState =
                    currentState != null && currentState.GetType().ToString() != "DAVEInvestigator";
                if (bNotInInvestigatorState)
                {
                    //Debug.Log("Changing State to Investigator");
                    // Its worth noting now that a new investigator is created each time it transitions, so any old sounds are ignorned. 
                    //This shouldnt change much because the exit condition for the investigator is to essentially clear the list or get distracted by finding the player
                    ExitCurrentState();
                    currentState = new DAVEInvestigator();
                    currentState.StateEnter(this);
                }

                PlayAudio(daveHeardNoise);
                // Call the event (after transitioning, so the investigator can pick up the event call)
                HeardNoise?.Invoke(soundData);
            }
        }
    }

    #endregion Collisions


    // Simple Audio Handler to make sure we do not accidently play a sound multiple times
    void PlayAudio(AudioSource soundToPlay)
    {
        if (!soundToPlay.isPlaying)
        {
            soundToPlay.Play();
        }
    }
}