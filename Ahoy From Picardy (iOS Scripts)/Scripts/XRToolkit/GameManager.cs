using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void QuitGameOnSignal()
    {
        Debug.Log("App should quit now");
        Application.Quit(); 
    }
}
