using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using UnityEngine.XR.ARFoundation;

public class DisablePlacement : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The target alpha value (0 for fully transparent).")]
    [Range(0f, 1f)]
    private float targetAlpha = 0f;

    [SerializeField]
    [Tooltip("The speed at which the plane detection visuals should fade out.")]
    private float fadeOutSpeed = 1f;

    [SerializeField]
    [Tooltip("Name of the XROrigin GameObject in the scene.")]
    private string xrOriginName = "XR Origin (XR Rig)";

    [SerializeField]
    [Tooltip("Name of the ARPlacement Ahoy GameObject in the scene.")]
    private string arPlacementObjectName = "ARPlacement Ahoy";

    [SerializeField]
    [Tooltip("Optional: Drag the AR Default Plane Prefab here to modify its initial appearance.")]
    private GameObject arDefaultPlanePrefab;

    [SerializeField]
    [Tooltip("Delay in seconds before fading existing planes after modifying the prefab.")]
    private float fadeDelay = 1f;

    private ARPlaneManager planeManager;
    private bool hasPlacedObject = false;

    private void Start()
    {
        // Find the GameObject that contains the ARPlacementInteractable
        GameObject arPlacementObject = GameObject.Find(arPlacementObjectName);

        if (arPlacementObject != null)
        {
            arPlacementObject.SetActive(false);
            FindPlaneManager();
        }
        else
        {
            Debug.LogError($"GameObject '{arPlacementObjectName}' not found in the scene.");
        }
    }

    private void FindPlaneManager()
    {
        // Find the XROrigin
        GameObject xrOrigin = GameObject.Find(xrOriginName);
        if (xrOrigin == null)
        {
            Debug.LogError($"XROrigin GameObject with name '{xrOriginName}' not found in the scene.");
            return;
        }

        // Get the ARPlaneManager from the XROrigin
        planeManager = xrOrigin.GetComponent<ARPlaneManager>();
        if (planeManager == null)
        {
            Debug.LogError("ARPlaneManager component not found on the XROrigin.");
        }
    }

    public void StartFadingPlaneLines()
    {
        if (hasPlacedObject) return; // Only fade once

        StartCoroutine(DelayedFadingSequence());

        hasPlacedObject = true;
    }

    private IEnumerator DelayedFadingSequence()
    {
        ModifyDefaultPlanePrefab();

        // Wait for the specified delay
        yield return new WaitForSeconds(fadeDelay);

        FadeOutExistingPlanes();
    }

    private void ModifyDefaultPlanePrefab()
    {
        if (arDefaultPlanePrefab != null)
        {
            SetLineRendererAlphaOnPrefab(arDefaultPlanePrefab, targetAlpha);
        }
        else
        {
            Debug.LogWarning("AR Default Plane Prefab not assigned, new planes might appear with default outlines.");
        }
    }

    private void FadeOutExistingPlanes()
    {
        if (planeManager != null && planeManager.trackables != null)
        {
            var currentPlanes = planeManager.trackables;
            if (currentPlanes != null)
            {
                foreach (var plane in currentPlanes)
                {
                    LineRenderer lineRenderer = plane.GetComponentInChildren<LineRenderer>();
                    if (lineRenderer != null)
                    {
                        StartCoroutine(FadeOutLineRenderer(lineRenderer));
                    }
                }
            }
            else
            {
                Debug.LogWarning("No planes found in ARPlaneManager's trackables at the time of placement.");
            }
        }
        else
        {
            Debug.LogWarning("Plane Manager not found when trying to fade existing planes.");
        }
    }

    private void SetLineRendererAlphaOnPrefab(GameObject prefab, float alphaValue)
    {
        LineRenderer lineRenderer = prefab.GetComponentInChildren<LineRenderer>();
        if (lineRenderer != null && lineRenderer.material != null)
        {
            Gradient gradient = lineRenderer.colorGradient;
            GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i].alpha = alphaValue;
            }
            gradient.SetKeys(gradient.colorKeys, alphaKeys);
            lineRenderer.colorGradient = gradient;
            Debug.Log("Modified AR Default Plane Prefab LineRenderer alpha.");
        }
        else
        {
            Debug.LogWarning("LineRenderer or its material not found on the AR Default Plane Prefab.");
        }
    }

    private IEnumerator FadeOutLineRenderer(LineRenderer lineRenderer)
    {
        if (lineRenderer == null)
        {
            yield break;
        }

        Gradient gradient = lineRenderer.colorGradient;
        GradientColorKey[] colorKeys = gradient.colorKeys;
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

        bool fading = true;
        while (fading)
        {
            fading = false;
            for (int i = 0; i < alphaKeys.Length; i++)
            {
                if (alphaKeys[i].alpha > targetAlpha)
                {
                    alphaKeys[i].alpha -= Time.deltaTime * fadeOutSpeed;
                    fading = true;
                }
            }

            gradient.SetKeys(gradient.colorKeys, alphaKeys);
            lineRenderer.colorGradient = gradient;

            if (!fading)
            {
                break;
            }
            yield return null;
        }

        Debug.Log($"LineRenderer on plane '{lineRenderer.transform.parent.name}' alpha faded out.");
    }
}