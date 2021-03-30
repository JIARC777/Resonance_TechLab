using System;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private string[] sceneNames;

    private void Awake()
    {


        if (Application.isPlaying)
        {
            foreach (string sceneName in sceneNames)
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }
        else
        {
            foreach (string sceneName in sceneNames)
            {
                EditorSceneManager.OpenScene(sceneName, OpenSceneMode.Additive);
            }
        }
    }
    
}