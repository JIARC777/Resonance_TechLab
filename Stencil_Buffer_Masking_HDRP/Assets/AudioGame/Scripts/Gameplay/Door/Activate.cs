using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate : MonoBehaviour
{
    public float doorEndPos;
    public float doorOpenTime;

    public bool vertical;
    public bool horizontal;
    public bool dualDoors;
    public bool terminal;

    public bool moveLeft;

    public Vector3 doorOrigPos;

    public Transform door;

    public Transform doorLeft;
    public Transform doorRight;

    private void Start()
    {
        if (vertical || horizontal)
        {
            door = transform.GetChild(0);
            doorOrigPos = door.localPosition;
        }
        else if(dualDoors)
        {
            doorLeft = transform.GetChild(0);
            doorRight = transform.GetChild(1);
        }
    }

    public void Activation(bool openOrClose)
    {
        if(vertical)
        {
            StartCoroutine(VerticalDoor(openOrClose));
        }
        if(horizontal)
        {
            StartCoroutine(HorizontalDoor(openOrClose));
        }
        if (dualDoors)
        {
            StartCoroutine(DualDoors(openOrClose));
        }
        if (terminal)
        {
            EndLevel();
        }
    }

    IEnumerator VerticalDoor(bool openOrClose)
    {

        float timeTillUp = 0;

        if (openOrClose)
        {
            while (timeTillUp < doorOpenTime)
            {
                door.localPosition = new Vector3(door.localPosition.x, Mathf.Lerp(door.localPosition.y, doorEndPos, timeTillUp), door.localPosition.z);

                timeTillUp += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (timeTillUp < doorOpenTime)
            {
                door.localPosition = new Vector3(door.localPosition.x, Mathf.Lerp(door.localPosition.y, doorOrigPos.y, timeTillUp), door.localPosition.z);

                timeTillUp += Time.deltaTime;
                Debug.Log("Time Till up: " + timeTillUp);
                yield return null;
            }
        }
        
    }

    IEnumerator DualDoors(bool openOrClose)
    {
        float timeTillUp = 0;

        float origLeft = doorLeft.localPosition.x + doorEndPos;
        float origRight = doorLeft.localPosition.x - doorEndPos;

        float newLeftEnd = doorLeft.localPosition.x - doorEndPos;
        float newRightEnd = doorRight.localPosition.x + doorEndPos;

        if (openOrClose)
        {
            while (timeTillUp < doorOpenTime)
            {
                doorLeft.localPosition = new Vector3(Mathf.Lerp(doorLeft.localPosition.x, newLeftEnd, timeTillUp), doorLeft.localPosition.y, doorLeft.localPosition.z);
                doorRight.localPosition = new Vector3(Mathf.Lerp(doorRight.localPosition.x, newRightEnd, timeTillUp), doorRight.localPosition.y, doorRight.localPosition.z);

                timeTillUp += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (timeTillUp < doorOpenTime)
            {
                doorLeft.localPosition = new Vector3(Mathf.Lerp(doorLeft.localPosition.x, origLeft, timeTillUp), doorLeft.localPosition.y, doorLeft.localPosition.z);
                doorRight.localPosition = new Vector3(Mathf.Lerp(doorRight.localPosition.x, origRight, timeTillUp), doorRight.localPosition.y, doorRight.localPosition.z);

                timeTillUp += Time.deltaTime;
                yield return null;
            }
        }
        
    }

    IEnumerator HorizontalDoor(bool openOrClose)
    {
        float timeTillUp = 0;

        if (openOrClose)
        {
            if (moveLeft)
            {
                while (timeTillUp < doorOpenTime)
                {
                    door.localPosition = new Vector3(-Mathf.Lerp(door.localPosition.x, doorEndPos, timeTillUp), door.localPosition.y, door.localPosition.z);

                    timeTillUp += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (timeTillUp < doorOpenTime)
                {
                    door.localPosition = new Vector3(Mathf.Lerp(door.localPosition.x, doorEndPos, timeTillUp), door.localPosition.y, door.localPosition.z);

                    timeTillUp += Time.deltaTime;
                    yield return null;
                }
            }
        }
        else
        {
            if (moveLeft)
            {
                while (timeTillUp < doorOpenTime)
                {
                    door.localPosition = new Vector3(-Mathf.Lerp(door.localPosition.x, doorOrigPos.x, timeTillUp), door.localPosition.y, door.localPosition.z);

                    timeTillUp += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (timeTillUp < doorOpenTime)
                {
                    door.localPosition = new Vector3(Mathf.Lerp(door.localPosition.x, doorOrigPos.x, timeTillUp), door.localPosition.y, door.localPosition.z);

                    timeTillUp += Time.deltaTime;
                    yield return null;
                }
            }
        }        
    }

    public void EndLevel()
    {
        Debug.Log("The Player has reached the end of the level!");
    }
}
