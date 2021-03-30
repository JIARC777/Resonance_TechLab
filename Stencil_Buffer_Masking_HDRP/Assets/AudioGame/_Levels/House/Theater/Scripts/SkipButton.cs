using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipButton : MonoBehaviour
{
    public IntroManager introManager;
    private void OnCollisionEnter(Collision collision)
    {
        introManager.EndMovie();
    }
}
