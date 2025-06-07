using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections; 

public class PrologueEndHandler : MonoBehaviour
{
    public PlayableDirector prologueDirector;
    public string nextSceneName = "Scene2Name";
    private bool sceneLoadInitiated = false; 

    public void LoadNextSceneFromSignal()
    {
        if (sceneLoadInitiated) return; // Prevent multiple loads
        sceneLoadInitiated = true;

        Debug.Log("LoadNextSceneFromSignal called");
        StartCoroutine(DelayedLoadNextScene());
    }

    IEnumerator DelayedLoadNextScene()
    {
        // 1. Disable Gyroscope
        GyroParallax parallaxScript = FindObjectOfType<GyroParallax>();
        if (parallaxScript != null)
        {
            parallaxScript.DisableGyroscope();
        }

        // 2. Wait for a short delay
        yield return new WaitForSeconds(0.5f);

        // 3. Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}