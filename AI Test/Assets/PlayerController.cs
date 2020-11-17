using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    public NavMeshAgent agent;
    public Camera cam;

    // value for determining whether or not a drone will treat a sound with suspicion and investigate
    public float threshold = 1f;
    public float investigationTime = 3f;
    // List of noises - Prioritiy on louder noises 
    List<Noise> noisesDetected;
    // Use to determine if collisions are originating from the same source or not - Constantly refreshed so that non-current sounds are not considered
    public Queue<int> SoundIDs;
    // used in determining collisions when 
    int curNoiseID;
    // Amount of time before dequeuing the SoundID queue to keep it organized 
    float timeSinceLastIDQueueRefresh;
    // Set it to an arbitrarily high value so that before any sound is detected it wont attempt to prioritize anything
    float timeReachedTarget = 100000;
    // Time between refreshing the list
    float idQueueRefreshRate = 10f;
    //bool locationReached = false;
    bool investigating;
    bool finishedInvestigating = true;
    bool patrolling;
    public float arrivalRadius = 3f;
    Vector3 locationToInvestigate;
    ParticleSystem pingDetection;
    public GameObject[] guardPathNode;
    int curPathNodeIndex;

    public delegate void Impact();
    public static event Impact ProbeDestroyed;

    public void Start()
    {
        noisesDetected = new List<Noise>();
        SoundIDs = new Queue<int>();
        pingDetection = GetComponent<ParticleSystem>();
        patrolling = true;
        curPathNodeIndex = 0;

        // Start the guard on a patrol route
        locationToInvestigate = guardPathNode[curPathNodeIndex].transform.position;
        agent.SetDestination(locationToInvestigate);
        curPathNodeIndex = (curPathNodeIndex + 1) % guardPathNode.Length;

    }
    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        //   {
        //     DirectWithMouseInput();
        //   }
        float distanceToTarget;
        if (investigating || patrolling)
        {
            //Debug.Log("Investigating");
            distanceToTarget = (this.transform.position - locationToInvestigate).magnitude;
            if (distanceToTarget <= arrivalRadius)
            {
                if (investigating)
                {
                    Debug.Log("Reached Location: Sending Ping");
                    investigating = false;
                    timeReachedTarget = Time.time;
                    if (noisesDetected.Count > 0)
                        noisesDetected.RemoveAt(curNoiseID);
                    // Debug.Log(noisesDetected.Count);
                    pingDetection.Emit(500);
                    // if there are any other targets, prioritize them
                } else
                {
                    locationToInvestigate = guardPathNode[curPathNodeIndex].transform.position;
                    agent.SetDestination(locationToInvestigate);
                    curPathNodeIndex = (curPathNodeIndex + 1) % guardPathNode.Length;

                }

            }
        } else
        {
            if (Time.time >= timeReachedTarget + investigationTime & !patrolling)
            {
                if (!Prioritize())
                {
                    agent.SetDestination(this.transform.position);
                    Debug.Log("Logic for returning to path");
                    findClosestPatrolNode();
                    patrolling = true;
                }
            }
        }
        // Dequeue the list of detected sounds every so often
        if (Time.time >= timeSinceLastIDQueueRefresh + idQueueRefreshRate && SoundIDs.Count > 0)
            SoundIDs.Dequeue();
    }
    void findClosestPatrolNode()
    {
        int closestNode = 0;
        float smallestDistance = 1000;
        for (int i = 0; i < guardPathNode.Length; i++)
        {
            float distance = (this.transform.position - guardPathNode[i].transform.position).magnitude;
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestNode = i;
            }
        }
        // Minus one because the logic for patrolling increments 
        locationToInvestigate = guardPathNode[closestNode].transform.position;
        agent.SetDestination(locationToInvestigate);
    }
    private void OnParticleCollision(GameObject soundPulse)
    {
        
        if (soundPulse.tag == "SuspiciousSound")
        {
            
            NoiseEmitter suspectedNoise = soundPulse.GetComponent<NoiseEmitter>();
            Noise detectedNoise = suspectedNoise.noise;
            
            bool idRecognized = false;
            if (SoundIDs.Count > 0)
            {
                foreach (int soundID in SoundIDs)
                {
                  //  Debug.Log(soundID);
                    if (detectedNoise.noiseID == soundID)
                        idRecognized = true;
                }
            }
            if (idRecognized)
            {
               // Debug.Log("Noticed Same ID, Ignoring");
                return;
            }
            else
            {
                
                float detectedVolume = detectedNoise.volume;
                if (detectedVolume > threshold)
                {
                    patrolling = false;
                    SoundIDs.Enqueue(detectedNoise.noiseID);
                    timeSinceLastIDQueueRefresh = Time.time;
                    noisesDetected.Add(detectedNoise);
                    Debug.Log("Heard Something: Move to investigate");
                    Prioritize();
                }
            }
        }
    }

    // Prioritize the loudest sound heard. Returns false if it is currently investigating a sound, or has no sounds to prioritize
    bool Prioritize()
    {
        if (noisesDetected.Count < 1)
        {
            Debug.Log("Cannot Prioritize");
            return false;
        }     
        Noise loudestNoise = noisesDetected[0];
        for (int i = 0; i < noisesDetected.Count; i++)
        {
            if (noisesDetected[i].volume > loudestNoise.volume)
            {
                loudestNoise = noisesDetected[i];
                curNoiseID = i;
            }
                
        }
        locationToInvestigate = loudestNoise.emittedLocation;
        Investigate();
        return true;
    }
    void Investigate()
    {
        investigating = true;
        agent.SetDestination(locationToInvestigate);
        
    }
    void DirectWithMouseInput()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            agent.SetDestination(hit.point);
        }
    }
}


