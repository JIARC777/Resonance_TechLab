using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{

    private GameObject parentDrone;

    private Transform playerTracker;
    private Transform turningObject;
    private Transform otherObject;

    public float lookTowards;
    public float lookAway;
    
    private bool lookAtTracker = false;

    private Apply_Basic_Force forces;

    private void Awake()
    {

        parentDrone = GameObject.Find("Drone_Body");

        turningObject = GameObject.Find("Turning_Object").GetComponent<Transform>();
        playerTracker = GameObject.Find("Player_Tracker").GetComponent<Transform>();

        forces = turningObject.GetComponent<Apply_Basic_Force>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.tag == "Player")
        {

            otherObject = other.transform;            
            LookAt();

        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "Player")
        {
            
            ReturnBack();
                       
        }
    }

    private void Update()
    {

        if (lookAtTracker)
            PlayerTracker();
        else
            Turning();

    }

    private void LookAt()
    {

        StartCoroutine(Arriving());

        forces.pausing = true;
        lookAtTracker = true;

        IEnumerator Arriving()
        {

            float beginX = playerTracker.transform.position.x;
            float beginY = playerTracker.transform.position.y;
            float beginZ = playerTracker.transform.position.z;

            float endX = otherObject.transform.position.x;
            float endY = otherObject.transform.position.y;
            float endZ = otherObject.transform.position.z;

            float time = 0;

            while (time < lookTowards)
            {

                playerTracker.transform.position = new Vector3(Mathf.Lerp(beginX, endX, time / lookTowards), Mathf.Lerp(beginY, endY, time / lookTowards), Mathf.Lerp(beginZ, endZ, time / lookTowards));
                parentDrone.transform.LookAt(playerTracker);
                time += Time.deltaTime;
                yield return null;

            }

            playerTracker.transform.position = otherObject.transform.position;            

        }       
    }

    private void Turning()
    {
        
        parentDrone.transform.LookAt(turningObject);

    }

    private void PlayerTracker()
    {

        playerTracker.transform.position = otherObject.transform.position;
        parentDrone.transform.LookAt(playerTracker);

    }

    private void ReturnBack()
    {

        StartCoroutine(Returning());        

        IEnumerator Returning()
        {

            float beginX = playerTracker.transform.position.x;
            float beginY = playerTracker.transform.position.y;
            float beginZ = playerTracker.transform.position.z;

            float endX = turningObject.transform.position.x;
            float endY = turningObject.transform.position.y;
            float endZ = turningObject.transform.position.z;

            float time = 0;

            while(time < lookAway)
            {

                playerTracker.transform.position = new Vector3(Mathf.Lerp(beginX, endX, time / lookAway), Mathf.Lerp(beginY, endY, time / lookAway), Mathf.Lerp(beginZ, endZ, time / lookAway));
                time += Time.deltaTime;
                parentDrone.transform.LookAt(playerTracker);
                yield return null;

            }

            otherObject = null;

            playerTracker.transform.position = turningObject.transform.position;

            lookAtTracker = false;
            forces.pausing = false;
        }
    }
}
