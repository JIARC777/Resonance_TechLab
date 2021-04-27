using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaveManager : MonoBehaviour
{
    public List<GameObject> daves = new List<GameObject>();
    public float enableDelay = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("EnableDave", enableDelay);
    }

    void EnableDave()
    {
        foreach (GameObject dave in daves)
        {
            dave.SetActive(true);
        }
    }
}
