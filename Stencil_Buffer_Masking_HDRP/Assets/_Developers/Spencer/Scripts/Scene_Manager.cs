using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class Scene_Manager : MonoBehaviour
{
    [Header("Start with vertical slice position")]
    public List<Vector3> levelPositions = new List<Vector3>();

    public GameObject player;
    
    Scene transitionScene;
    Scene tutorial;
    Scene virtualSlice;
    Scene levelThree;

    int nextScene = 3;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(transform);
    }

    IEnumerator LoadTransitionScene()
    {
        AsyncOperation transition = SceneManager.LoadSceneAsync(1);

        while (!transition.isDone)
        {
            Debug.Log("Transition Scene not loaded");
            yield return null;
        }

        Debug.Log("Transition Scene loaded");

        transitionScene = SceneManager.GetSceneByBuildIndex(1);
    }

    IEnumerator LoadNextScene()
    {
        player.GetComponent<ResonanceMovement>().enabled = false;
        ResonancePlayerHealthRespawn playerHP = player.GetComponent<ResonancePlayerHealthRespawn>();
        playerHP.currentPlayerHealth = playerHP.maxPlayerHealth;
        playerHP.healthNumberText.text = playerHP.currentPlayerHealth.ToString();
        playerHP.respawnLocation = levelPositions[nextScene - 1];

        //player.transform.position = Vector3.up;
        player.transform.position = levelPositions[nextScene];
        StartCoroutine(LoadTransitionScene());

        yield return new WaitForSeconds(10);

        AsyncOperation load = SceneManager.LoadSceneAsync(nextScene);

        while (!load.isDone)
        {
            Debug.Log(SceneManager.GetSceneByBuildIndex(nextScene).name + " not loaded");
            
            yield return null;
        }

        Debug.Log("Is loaded");
        
        player.GetComponent<ResonanceMovement>().enabled = true;
        nextScene++;

        yield return new WaitForSeconds(1);
    }

    public void LoadNextLevel()
    {
        DestroyTools();
        StartCoroutine(LoadNextScene());
    }

    void DestroyTools()
    {
        if (GameObject.Find("AMT"))
        {
            Destroy(GameObject.Find("AMT"));
        }
        if (GameObject.Find("3DR"))
        {
            Destroy(GameObject.Find("3DR"));
        }
    }
}
