using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    public int indexOfSceneToLoad;
    public float delay;

    public void StartSceneTransition()
    {
        Invoke("LoadNextScene", delay);
    }

    void LoadNextScene()
    {
        Destroy(GameObject.Find("Player_Intro"));
        SceneManager.LoadScene(indexOfSceneToLoad);
    }
}
