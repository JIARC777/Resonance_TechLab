using System;
using UnityEngine.SceneManagement;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

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
            #if UNITY_EDITOR
            foreach (string sceneName in sceneNames)
            {
                EditorSceneManager.OpenScene(sceneName, OpenSceneMode.Additive);
            }
            #endif
        }
    }
    
}