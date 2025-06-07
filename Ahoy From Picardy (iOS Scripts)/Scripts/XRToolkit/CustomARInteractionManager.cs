using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class CustomARInteractionManager : MonoBehaviour
{
    [SerializeField]
    private ARScaleInteractable scaleInteractable;
    [SerializeField]
    private ARRotationInteractable rotationInteractable;
    [SerializeField]
    private ARTranslationInteractable translateInteractable;

    private bool isScaling = false;
    private bool isRotating = false;
    private bool isTranslating = false;

    private void Awake()
    {
        EnableAllInteractions();
    }

    public void OnScaleStart()
    {
        if (isRotating || isTranslating) return; // Prevent activation if other interaction is active
        isScaling = true;
        rotationInteractable.enabled = false;
        translateInteractable.enabled = false;
        Debug.Log("Scale Enabled/ others disabled");
    }

    public void OnScaleEnd()
    {
        isScaling = false;
        EnableAllInteractions();
    }

    public void OnRotateStart()
    {
        if (isScaling || isTranslating) return;
        isRotating = true;
        scaleInteractable.enabled = false;
        translateInteractable.enabled = false;
        Debug.Log("Rotation Enabled/ others disabled");
    }

    public void OnRotateEnd()
    {
        isRotating = false;
        EnableAllInteractions();
    }

    public void OnTranslateStart()
    {
        if (isScaling || isRotating) return;
        isTranslating = true;
        scaleInteractable.enabled = false;
        rotationInteractable.enabled = false;
        Debug.Log("Translate Enabled/ others disabled");
    }

    public void OnTranslateEnd()
    {
        isTranslating = false;
        EnableAllInteractions();
    }

    private void EnableAllInteractions()
    {
        if (!isScaling && !isRotating && !isTranslating)
        {
            scaleInteractable.enabled = true;
            rotationInteractable.enabled = true;
            translateInteractable.enabled = true;
            Debug.Log("All Enabled");
        }
    }
}
