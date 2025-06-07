using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;

#if AR_FOUNDATION_PRESENT

public class PinataARPlacement : ARPlacementInteractable
{
    [Header("Piñata Specific AR Placement")]
    [Tooltip("Manager to detect and filter AR planes.")]
    [SerializeField] private ARPlaneManager planeManager;
    [Tooltip("The height above the detected floor plane if no ceiling is found.")]
    [SerializeField] private float defaultFloorHeightOffset = 2.4f;
    [Tooltip("The local Y distance from prefab's pivot to the Pinata Anchor point.")]
    [SerializeField] private float pinataAnchorLocalYOffset = 0f; // Set this in Inspector!
    [Tooltip("Max distance upwards to raycast for a virtual ceiling (if no AR ceiling plane).")]
    [SerializeField] private float virtualCeilingDetectionMaxHeight = 7f;
    [Tooltip("LayerMask for virtual ceiling detection (non-AR planes).")]
    [SerializeField] private LayerMask virtualCeilingLayerMask;

    private static readonly List<ARRaycastHit> s_RaycastHits = new List<ARRaycastHit>(); // For AR raycasts

    // Lifecycle methods
    protected override void OnEnable()
    {
        base.OnEnable();
        if (planeManager != null)
        {
            planeManager.planesChanged += HandlePlanesChanged; // Subscribe to plane updates
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (planeManager != null)
        {
            planeManager.planesChanged -= HandlePlanesChanged; // Unsubscribe
        }
    }

    // Handles AR plane visibility based on classification
    private void HandlePlanesChanged(ARPlanesChangedEventArgs obj)
    {
        // Hide/Destroy non-ceiling planes
        foreach (var plane in obj.added)
        {
            if (plane.classification != PlaneClassification.Ceiling)
            {
                DestroyPlaneRenderer(plane);
            }
        }
        foreach (var plane in obj.updated)
        {
            if (plane.classification != PlaneClassification.Ceiling)
            {
                DestroyPlaneRenderer(plane);
            }
        }
        // Removed planes typically handled by ARFoundation itself
    }

    // Helper to destroy plane renderer components
    private void DestroyPlaneRenderer(ARPlane plane)
    {
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        if (planeRenderer != null) Destroy(planeRenderer);
        ARPlaneMeshVisualizer meshVisualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
        if (meshVisualizer != null) Destroy(meshVisualizer);
        Debug.Log($"Hidden non-ceiling plane: {plane.classification}");
    }

    // Determines placement pose based on tap
    protected override bool TryGetPlacementPose(TapGesture gesture, out Pose pose)
    {
        pose = default;
        Camera arCamera = xrOrigin != null ? xrOrigin.Camera : Camera.main;
        if (arCamera == null)
        {
            Debug.LogError("AR Camera not found for placement.");
            return false;
        }

        // Raycast for AR planes
        if (planeManager == null)
        {
            Debug.LogError("ARPlaneManager not assigned for plane detection.");
            return false;
        }
        
        // Use ARPlaneManager's raycast for AR planes
        if (planeManager.Raycast(gesture.startPosition, s_RaycastHits, TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit bestHit = default;
            bool foundCeiling = false;
            
            // Prioritize ceiling hits
            foreach (var hit in s_RaycastHits)
            {
                ARPlane hitPlane = planeManager.Get ">" (hit.trackableId); // Get ARPlane component from hit
                if (hitPlane != null && hitPlane.classification == PlaneClassification.Ceiling)
                {
                    bestHit = hit;
                    foundCeiling = true;
                    break; // Found a ceiling, use it
                }
            }

            // If ceiling found, place anchor at ceiling hit point
            if (foundCeiling)
            {
                pose = new Pose(bestHit.pose.position - Vector3.up * pinataAnchorLocalYOffset, bestHit.pose.rotation);
                Debug.Log($"Placed on AR Ceiling at: {pose.position.y}");
                return true;
            }
            else // No AR ceiling hit, fallback to first valid AR plane (likely floor)
            {
                bestHit = s_RaycastHits[0]; // Take the first hit (usually floor/wall)
                Debug.Log($"No AR ceiling. Placing on AR plane ({bestHit.pose.position.y}) with offset.");
                
                // Check for virtual ceiling if no AR ceiling was found
                Ray virtualCeilingRay = new Ray(bestHit.pose.position + Vector3.up * 0.1f, Vector3.up);
                RaycastHit virtualCeilingHit;

                if (Physics.Raycast(virtualCeilingRay, out virtualCeilingHit, virtualCeilingDetectionMaxHeight, virtualCeilingLayerMask))
                {
                    pose = new Pose(virtualCeilingHit.point - Vector3.up * pinataAnchorLocalYOffset, bestHit.pose.rotation);
                    Debug.Log($"Placed on Virtual Ceiling at: {pose.position.y}");
                    return true;
                }
                else // No AR ceiling, no virtual ceiling. Default to floor offset.
                {
                    pose = new Pose(bestHit.pose.position + Vector3.up * defaultFloorHeightOffset, bestHit.pose.rotation);
                    Debug.Log($"Placed on Floor with Default Offset at: {pose.position.y}");
                    return true;
                }
            }
        }
        
        Debug.Log("No AR plane hit for placement.");
        return false;
    }
}

#endif // AR_FOUNDATION_PRESENT
