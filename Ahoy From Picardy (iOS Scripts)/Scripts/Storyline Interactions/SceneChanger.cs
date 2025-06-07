using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private string _sceneName;

    public void ChangeScene()
    {
        if (string.IsNullOrEmpty(_sceneName))
        {
            Debug.LogError("Scene name is empty. Please specify a scene to load in the Inspector.");
            return;
        }

        SceneManager.LoadScene(_sceneName);
        Debug.Log("Scene loaded: " + _sceneName);
    }
}