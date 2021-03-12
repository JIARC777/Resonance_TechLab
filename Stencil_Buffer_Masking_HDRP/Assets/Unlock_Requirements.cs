using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlock_Requirements : MonoBehaviour
{

    public float panelsRequired;
    public float panelsActivated;

    public Material someActivePanelMat;
    public Material noneActivePanelMat;
    public Material allActivePanelMat;

    public Transform[] linkedObjects;

    // Start is called before the first frame update
    void Start()
    {
        panelsActivated = 0;

        SetMaterial(noneActivePanelMat);
    }

    public void PanelActivated()
    {
        panelsActivated++;        

        SetMaterial(someActivePanelMat);

        if(panelsActivated == panelsRequired)
        {
            SetMaterial(allActivePanelMat);

            for(int i = 0; i < linkedObjects.Length; i++)
            {
                linkedObjects[i].gameObject.GetComponent<Activate>().Activation();
            }
        }
    }

    public void SetMaterial(Material mat)
    {
        Debug.Log("Here");
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().material = mat;
    }
}
