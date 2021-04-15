using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{

    Scene transitionScene;
    Scene tutorial;
    Scene virtualSlice;
    Scene levelThree;

    int nextScene = 1;

    // Start is called before the first frame update
    void Start()
    {

        DontDestroyOnLoad(transform);
    }

    IEnumerator LoadTransitionScene()
    {
        AsyncOperation transition = SceneManager.LoadSceneAsync(0);

        while (!transition.isDone)
        {
            Debug.Log("Transition Scene not loaded");
            yield return null;
        }

        Debug.Log("Transition Scene loaded");

        transitionScene = SceneManager.GetSceneByBuildIndex(0);
    }

    IEnumerator LoadNextScene()
    {

        StartCoroutine(LoadTransitionScene());

        yield return new WaitForSeconds(1);

        AsyncOperation load = SceneManager.LoadSceneAsync(nextScene);

        while (!load.isDone)
        {
            Debug.Log(SceneManager.GetSceneByBuildIndex(nextScene).name + " not loaded");
            
            yield return null;
        }

        Debug.Log("Is loaded");

        nextScene++;

        yield return new WaitForSeconds(1);
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadNextScene());
    }
}
