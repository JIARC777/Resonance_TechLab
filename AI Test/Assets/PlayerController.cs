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
    // Use to determine if collisions are originating from the same source or not
    public Queue<int> SoundIDs;
    // used in determining collisions when 
    float noiseID;
    bool locationReached = false;
    bool investigating;
    public float investigationRadius = 3f;
    Vector3 locationToInvestigate;

    public void Start()
    {
        noisesDetected = new List<Noise>();

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
            Debug.Log("Investigating");
            distanceToTarget = (this.transform.position - locationToInvestigate).magnitude;
            if (distanceToTarget <= investigationRadius)
            {
                Debug.Log("No Longer Investiagting");
                investigating = false;
                agent.SetDestination(this.transform.position);
            }
                
        }   
    }
    private void OnParticleCollision(GameObject soundPulse)
    {
        //Debug.Log("Sound");
        if (soundPulse.tag == "SuspiciousSound")
        {
            
            NoiseEmitter suspectedNoise = soundPulse.GetComponent<NoiseEmitter>();
            
            Noise detectedNoise = suspectedNoise.noise;
            float detectedVolume = detectedNoise.volume;
            //Debug.Log(detectedVolume);
            int curNoiseID = detectedNoise.noiseID;
            //foreach (Noise noise in noisesDetected)
            //{
               // Debug.Log("Inside Loop");
               // if (curNoiseID == noise.noiseID)
               // {
               //     Debug.Log("Culling Particle");
               //     return;
               // }
            //}
            if (detectedVolume > threshold)
            {
                locationToInvestigate = detectedNoise.emittedLocation;
                Debug.Log("Heard Something: Move to investigate");
                Investigate();
                
               // noisesDetected.Add(detectedNoise);
            }
                
           // Prioritize();
        }
    }

    void Prioritize()
    {
        if (investigating)
            return;
        Noise loudestNoise = noisesDetected[0];
        foreach (Noise detectedNoise in noisesDetected)
        {
            if (detectedNoise.volume > loudestNoise.volume)
                loudestNoise = detectedNoise;
        }
        locationToInvestigate = loudestNoise.emittedLocation;
        Investigate();
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


