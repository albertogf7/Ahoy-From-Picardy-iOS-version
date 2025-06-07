using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ShowOnlyThisClassification : MonoBehaviour
{
    public ARPlaneManager planeManager;

    [Tooltip("List of plane classifications to show. All others will be hidden.")]
    public List<PlaneClassification> allowedClassifications;

    [Tooltip("Material to apply to detected Floor planes.")]
    public Material floorMaterial;
    [Tooltip("Material to apply to detected Ceiling planes.")]
    public Material ceilingMaterial;
    [Tooltip("Material to apply to other allowed planes (e.g., Wall, Table).")]
    public Material defaultAllowedMaterial; // Optional, for other allowed types

    private void OnEnable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged += SetupPlane;
        }
        else
        {
            Debug.LogError("ShowOnlyThisClassification: ARPlaneManager is not assigned!", this);
        }
    }

    private void OnDisable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged -= SetupPlane;
        }
    }

    private void SetupPlane(ARPlanesChangedEventArgs obj)
    {
        // Handle newly added planes
        foreach (var plane in obj.added)
        {
            ApplyPlaneVisibilityAndMaterial(plane);
        }

        // Handle updated planes (classification might change)
        foreach (var plane in obj.updated)
        {
            ApplyPlaneVisibilityAndMaterial(plane);
        }
    }

    private void ApplyPlaneVisibilityAndMaterial(ARPlane plane)
    {
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        ARPlaneMeshVisualizer meshVisualizer = plane.GetComponent<ARPlaneMeshVisualizer>();

        if (planeRenderer == null || meshVisualizer == null) return; // Need both components

        if (allowedClassifications.Contains(plane.classification))
        {
            // Plane matches an allowed classification
            planeRenderer.enabled = true; // Ensure renderer is active

            // Assign specific material based on classification
            if (plane.classification == PlaneClassification.Floor && floorMaterial != null)
            {
                planeRenderer.material = floorMaterial;
            }
            else if (plane.classification == PlaneClassification.Ceiling && ceilingMaterial != null)
            {
                planeRenderer.material = ceilingMaterial;
            }
            else if (defaultAllowedMaterial != null) // For other allowed types (e.g., Wall)
            {
                planeRenderer.material = defaultAllowedMaterial;
            }
            // else: if no specific material, it keeps its default AR material
        }
        else
        {
            // Hide planes that do not match allowed classifications
            planeRenderer.enabled = false;
        }
    }
}
