using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Unlock_Requirements : MonoBehaviour
{

    public float panelsRequired;
    public float panelsActivated;

    public Material someActivePanelMat;
    public Material noneActivePanelMat;
    public Material allActivePanelMat;

    public Transform[] linkedObjects;

    public TMP_Text remainingText;

    // Start is called before the first frame update
    void Start()
    {
        panelsActivated = 0;

        remainingText.text = "Panels Required\n\n" + panelsRequired;

        SetMaterial(noneActivePanelMat);
    }

    public void PanelActivated()
    {
        panelsActivated++;        

        SetMaterial(someActivePanelMat);

        remainingText.text = "Panels Required\n\n" + (panelsRequired - panelsActivated);

        if (panelsActivated == panelsRequired)
        {
            SetMaterial(allActivePanelMat);

            remainingText.text = "Panels Required\n\n" + (panelsRequired - panelsActivated);

            for (int i = 0; i < linkedObjects.Length; i++)
            {
                linkedObjects[i].gameObject.GetComponent<Activate>().Activation(true);
            }
        }
    }

    public void SetMaterial(Material mat)
    {
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().material = mat;
    }
}
