using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpToRestPosition : MonoBehaviour
{
    public Transform restingPosition;

    private Vector3 restTargetPosition;

    public bool bShouldMove = false;

    public float returntToRestTravelDurationSeconds = 5f;
    /// <summary>
    /// Time release info
    /// </summary>
    [SerializeField]
    private float timeToWaitAfterReleaseBeforeMoving = 1.2f;

    private float releasedFromHandTime;

    
    // Start is called before the first frame update
    void Start()
    {
        restTargetPosition = restingPosition.localPosition; //Save the target position
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float fracComplete = (Time.time - (releasedFromHandTime - timeToWaitAfterReleaseBeforeMoving)) / (returntToRestTravelDurationSeconds * 100f);

        if ( (bShouldMove == true) && (Time.time >= releasedFromHandTime + timeToWaitAfterReleaseBeforeMoving))
        {
            //Debug.LogFormat("Moving from {0} to {1} ", transform.position, restTargetPosition);
            transform.localPosition = Vector3.Slerp(transform.localPosition, restTargetPosition, fracComplete);
        }
    }


    public void Released()
    {
        Debug.Log("Released");
        releasedFromHandTime = Time.time;
        bShouldMove = true;
        StartCoroutine(StopMove());
    }

    IEnumerator StopMove()
    {
        yield return new WaitForSecondsRealtime(returntToRestTravelDurationSeconds + timeToWaitAfterReleaseBeforeMoving);
        bShouldMove = false;
    }

    public void Grabbed()
    {
        Debug.Log("Grabbed");
        bShouldMove = false;
    }
}
