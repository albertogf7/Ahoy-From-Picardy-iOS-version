using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuToggler : MonoBehaviour
{
    private AhoyARInput _arInput;

    void Awake()
    {
        // Find AhoyARInput in the scene
        _arInput = FindObjectOfType<AhoyARInput>();
        if (_arInput == null)
        {
            Debug.LogError("AhoyARInput not found in the scene! Make sure it exists.");
        }
    }

    public void CallDisableMenu()
    {
        _arInput.disableUIOnLoop();
    }

    public void CallEnableMenu()
    {
        _arInput.enableUIOnLoop();
    }
}
