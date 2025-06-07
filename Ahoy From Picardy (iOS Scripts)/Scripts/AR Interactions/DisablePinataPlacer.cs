using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePinataPlacer : MonoBehaviour
{
    [SerializeField]
    private string arPlacementObjectName = "Pinata AR Placement Object";

    [Header("Piñata Height Adjustment")]
    [SerializeField] private Transform pinataAnchorPoint;
    [SerializeField] private float desiredAnchorHeight = 2.3f;
    [SerializeField] private float minAcceptableAnchorHeight = 1.0f;

    [Header("Rotation Adjustment")]
    [Tooltip("Rotation for objects placed on ceilings (not elevated).")]
    [SerializeField] private Vector3 ceilingRotation = new Vector3(0, 180, 0);
    [Tooltip("Rotation for objects placed on floors (elevated).")]
    [SerializeField] private Vector3 floorRotation = new Vector3(0, 0, 0);

    [Header("Delayed Activation & Rope")]
    [Tooltip("The Rigidbody component of the Piñata Horse.")]
    [SerializeField] private Rigidbody piñataHorseRigidbody; 
    [Tooltip("The ConfigurableJoint component that connects to the piñata.")]
    [SerializeField] private ConfigurableJoint pinataJoint; 
    [Tooltip("The RopeRenderer script on the rope object.")]
    [SerializeField] private RopeRenderer ropeRendererScript; 
    [Tooltip("The exact point on the Piñata Horse where the rope should visually end.")]
    [SerializeField] private Transform hookPositionOverride; // <--- NEW FIELD: Assign your "Hook Position" here
    [Tooltip("Delay in seconds before enabling physics and rendering.")]
    [SerializeField] private float activationDelay = 0.15f; 


    private void Start()
    {
        // Deactivate AR placement object
        DeactivateARPlacer();

        // Adjust piñata height and apply rotation
        if (pinataAnchorPoint != null)
        {
            float currentAnchorHeight = pinataAnchorPoint.position.y;
            bool wasElevated = false; 

            if (currentAnchorHeight < minAcceptableAnchorHeight)
            {
                float yOffsetNeeded = desiredAnchorHeight - currentAnchorHeight;
                transform.position += Vector3.up * yOffsetNeeded;
                wasElevated = true;
                Debug.Log($"Piñata anchor elevated by {yOffsetNeeded:F2}m.");
            }
            else
            {
                Debug.Log($"Piñata anchor placed at acceptable height.");
            }

            // Apply rotation based on placement type
            if (wasElevated)
            {
                transform.localRotation = Quaternion.Euler(floorRotation);
                Debug.Log($"Applied floor rotation {floorRotation}.");
            }
            else
            {
                transform.localRotation = Quaternion.Euler(ceilingRotation);
                Debug.Log($"Applied ceiling rotation {ceilingRotation}.");
            }
        }
        else
        {
            Debug.LogError("Pinata Anchor Point not assigned.");
            return; 
        }

        // Start delayed activation coroutine
        StartCoroutine(DelayedActivationCoroutine());
    }

    // Deactivates AR placement object
    private void DeactivateARPlacer()
    {
        GameObject arPlacementObject = GameObject.Find(arPlacementObjectName);
        if (arPlacementObject != null)
        {
            arPlacementObject.SetActive(false);
            Debug.Log($"Deactivated AR Placement Object: {arPlacementObjectName}");
        }
        else
        {
            Debug.LogError($"GameObject '{arPlacementObjectName}' not found.");
        }
    }

    // Coroutine for delayed activation of physics and renderer
    private IEnumerator DelayedActivationCoroutine()
    {
        // Initial setup for physics and renderer states
        if (piñataHorseRigidbody != null)
        {
            piñataHorseRigidbody.isKinematic = true; 
        }
        
        if (ropeRendererScript != null)
        {
            ropeRendererScript.enabled = false; 
        }
        else
        {
             Debug.LogError("RopeRenderer script not assigned. Cannot control rope rendering.");
             yield break; 
        }
        
        // Wait for a short duration
        yield return new WaitForSeconds(activationDelay);

        // Enable Rigidbody
        if (piñataHorseRigidbody != null)
        {
            piñataHorseRigidbody.isKinematic = false; 
        }
        else
        {
            Debug.LogError("Piñata Horse Rigidbody not assigned. Cannot enable physics.");
            yield break; 
        }

        // Tell RopeRenderer to start drawing
        if (ropeRendererScript != null)
        {
            // Use the provided hookPositionOverride if available, otherwise fallback to piñata's transform
            Transform ropeEndPointTransform = (hookPositionOverride != null) ? hookPositionOverride : piñataHorseRigidbody.transform;

            ropeRendererScript.SetRopePointsAndStartDrawing(pinataAnchorPoint, ropeEndPointTransform);
            ropeRendererScript.enabled = true; 
        }
        else
        {
            Debug.LogError("RopeRenderer script not assigned. Cannot enable drawing.");
            yield break;
        }
    }
}