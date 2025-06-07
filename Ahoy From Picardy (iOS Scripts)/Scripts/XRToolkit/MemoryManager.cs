using UnityEngine;
using UnityEngine.SceneManagement;

public class MemoryManager : MonoBehaviour
{
    // Singleton instance
    private static MemoryManager _instance;

    public static MemoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // If no instance exists, find the existing MemoryManager in the scene
                _instance = FindObjectOfType<MemoryManager>();

                // If it's still null, create a new GameObject and attach MemoryManager to it
                if (_instance == null)
                {
                    GameObject obj = new GameObject("MemoryManager");
                    _instance = obj.AddComponent<MemoryManager>();
                }

                // Make sure the instance isn't destroyed between scene loads
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Ensure there's only one instance of MemoryManager
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        // Perform memory cleanup after a scene is unloaded
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        Debug.Log("Memory cleaned up after unloading scene: " + scene.name);
    }

    // Method to clean up memory before switching scenes
    public void CleanupMemoryBeforeSceneChange()
    {
        // Perform any necessary cleanup or optimization before switching scenes
        Resources.UnloadUnusedAssets();
        System.GC.Collect(); // Force garbage collection to free up memory
        Debug.Log("Memory cleaned up before scene change.");
    }

    // Method to optimize memory after switching timelines
    public void OptimizeMemoryAfterTimelineChange(int previousIndex)
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect(); // Force garbage collection
        Debug.Log("Memory optimized after timeline change.");
    }
}