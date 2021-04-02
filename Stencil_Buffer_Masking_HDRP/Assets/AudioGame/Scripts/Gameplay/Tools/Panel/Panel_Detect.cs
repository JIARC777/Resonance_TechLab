using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_Detect : MonoBehaviour
{
    public bool activated;
    public bool lockedPanel;

    public Material activePanelMat;
    public Material inactivePanelMat;

    public GameObject[] linkedObjects;

    private void Start()
    {
        SetMaterial(inactivePanelMat);

        activated = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ammo" && !activated)
        {
            for(int i = 0; i < linkedObjects.Length; i++)
            {
                Destroy(other.gameObject);

                SetMaterial(activePanelMat);

                activated = true;

                if (linkedObjects[i] && !lockedPanel)
                    linkedObjects[i].gameObject.GetComponent<Activate>().Activation(true);

                if (linkedObjects[i] && lockedPanel)
                    linkedObjects[i].gameObject.GetComponent<Unlock_Requirements>().PanelActivated();

            }

            //Insert sound emitting code here

        }
        if(other.tag == "Beam" && !activated)
        {
            for (int i = 0; i < linkedObjects.Length; i++)
            {

                SetMaterial(activePanelMat);

                activated = true;

                if (linkedObjects[i] && !lockedPanel)
                    linkedObjects[i].gameObject.GetComponent<Activate>().Activation(true);

                if (linkedObjects[i] && lockedPanel)
                    linkedObjects[i].gameObject.GetComponent<Unlock_Requirements>().PanelActivated();
            }
        }
    }

    public void SetMaterial(Material mat)
    {
        transform.parent.GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().material = mat;
    }
}
