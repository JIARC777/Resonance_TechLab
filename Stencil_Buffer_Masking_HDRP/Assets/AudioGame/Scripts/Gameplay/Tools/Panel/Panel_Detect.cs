using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_Detect : MonoBehaviour
{
    public bool activated;

    public Material activePanelMat;
    public Material inactivePanelMat;
    public Material currentPanelMat;

    public GameObject[] linkedObjects;

    private void Start()
    {
        currentPanelMat = inactivePanelMat;

        activated = false;

        Debug.Log(currentPanelMat);
    }

    private void Update()
    {
        transform.parent.GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().material = currentPanelMat;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ammo" && !activated)
        {
            for(int i = 0; i < linkedObjects.Length; i++)
            {
                Destroy(other.gameObject);

                currentPanelMat = activePanelMat;

                Debug.Log("Activated");

                activated = true;

                if (linkedObjects[i])
                    linkedObjects[i].gameObject.GetComponent<Activate>().Activation();

            }

            //Insert sound emitting code here

        }
        if(other.tag == "Beam" && !activated)
        {
            for (int i = 0; i < linkedObjects.Length; i++)
            {                

                currentPanelMat = activePanelMat;

                Debug.Log("Activated");

                activated = true;

                linkedObjects[i].gameObject.GetComponent<Activate>().Activation();
            }
        }
    }
}
