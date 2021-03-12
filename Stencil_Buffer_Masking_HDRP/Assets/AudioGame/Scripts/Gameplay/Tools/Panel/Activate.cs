using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate : MonoBehaviour
{
    public float doorEndPos;
    public float doorOpenTime;

    public bool vertical;
    public bool horizontal;

    public bool lockedPanel;

    public Transform upDoor;

    public Transform doorLeft;
    public Transform doorRight;

    public Unlock_Requirements requirements;

    private void Start()
    {
        if (vertical)
        {
            upDoor = transform.GetChild(0);
        }
        
        if(horizontal)
        {
            doorLeft = transform.GetChild(0);
            doorRight = transform.GetChild(1);
        }
    }

    public void Activation()
    {
        if(gameObject.tag == "Door" && vertical)
        {
            StartCoroutine(VerticalDoor());
        }
        if(gameObject.tag == "Door" && horizontal)
        {
            StartCoroutine(HorizonalDoor());
        }
        if (lockedPanel)
        {
            requirements.PanelActivated();
        }

    }

    IEnumerator VerticalDoor()
    {

        float timeTillUp = 0;

        while(timeTillUp < doorOpenTime)
        {
            upDoor.localPosition = new Vector3(upDoor.localPosition.x, Mathf.Lerp(upDoor.localPosition.y, doorEndPos, timeTillUp), upDoor.localPosition.z);

            timeTillUp += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator HorizonalDoor()
    {
        float timeTillUp = 0;

        float newLeftEnd = doorLeft.localPosition.x - doorEndPos;
        float newRightEnd = doorRight.localPosition.x + doorEndPos;

        while (timeTillUp < doorOpenTime)
        {
            doorLeft.position = new Vector3(Mathf.Lerp(doorLeft.localPosition.x, newLeftEnd, timeTillUp), doorLeft.localPosition.y, doorLeft.localPosition.z);
            doorRight.position = new Vector3(Mathf.Lerp(doorRight.localPosition.x, newRightEnd, timeTillUp), doorRight.localPosition.y, doorRight.localPosition.z);

            timeTillUp += Time.deltaTime;
            yield return null;
        }
    }
}
