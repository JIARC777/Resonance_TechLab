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
    // List of noises - Prioritiy on louder noises 
    List<Noise> noisesDetected;
    // Use to determine if collisions are originating from the same source or not - Constantly refreshed so that non-current sounds are not considered
    public Queue<int> SoundIDs;
    // used in determining collisions when 
    float curNoiseID;
    // Amount of time before dequeuing the SoundID queue to keep it organized 
    float timeSinceLastIDQueueRefresh;
    // Time between refreshing the list
    float idQueueRefreshRate = 10f;
    //bool locationReached = false;
    bool investigating;
    public float investigationRadius = 3f;
    Vector3 locationToInvestigate;

    public void Start()
    {
        noisesDetected = new List<Noise>();
        SoundIDs = new Queue<int>();

    }
    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        //   {
        //     DirectWithMouseInput();
        //   }
        float distanceToTarget;
        if (investigating)
        {
           // Debug.Log("Investigating");
            distanceToTarget = (this.transform.position - locationToInvestigate).magnitude;
            if (distanceToTarget <= investigationRadius)
            {
                Debug.Log("No Longer Investiagting");
                investigating = false;
                // if there are any other targets, prioritize them
                if (!Prioritize())
                {
                    agent.SetDestination(this.transform.position);
                    Debug.Log("Logic for returning to path");
                }
            }
        }
        // Dequeue the list of detected sounds every so often
        if (Time.time >= timeSinceLastIDQueueRefresh + idQueueRefreshRate && SoundIDs.Count > 0)
            SoundIDs.Dequeue();
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
        foreach (Noise detectedNoise in noisesDetected)
        {
            if (detectedNoise.volume > loudestNoise.volume)
                loudestNoise = detectedNoise;
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


