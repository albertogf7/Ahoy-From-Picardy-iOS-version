using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using UnityEngine.Events;

public class ARPlacementEventForwarder : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARPlacementInteractable component on the 'ARPlacement Ahoy' GameObject.")]
    private ARPlacementInteractable placementInteractable;

    public static event System.Action<GameObject> OnCustomObjectPlacedEvent;

    [SerializeField]
    [Tooltip("Name of the prefab root GameObject after placement (e.g., 'AHOYTimelinePrefab').")]
    private string placedPrefabRootName = "AHOYTimelinePrefab";

    private void OnEnable()
    {
        if (placementInteractable == null)
        {
            Debug.LogError("ARPlacementEventForwarder requires a reference to the ARPlacementInteractable component. Please assign the 'ARPlacement Ahoy' GameObject in the Inspector.");
            enabled = false;
            return;
        }

        placementInteractable.objectPlaced.AddListener(OnObjectPlacedHandler);
    }

    private void OnDisable()
    {
        if (placementInteractable != null)
        {
            placementInteractable.objectPlaced.RemoveListener(OnObjectPlacedHandler);
        }
    }

    private void OnObjectPlacedHandler(ARObjectPlacementEventArgs args)
    {
        if (OnCustomObjectPlacedEvent != null && args.placementObject != null)
        {
            OnCustomObjectPlacedEvent(args.placementObject);

            DisablePlacement disablePlacementScript = null;

            // Check if the placed object's name matches our prefab root name
            if (args.placementObject.name.StartsWith(placedPrefabRootName))
            {
                // Try to get DisablePlacement directly from the placed object
                disablePlacementScript = args.placementObject.GetComponent<DisablePlacement>();
                if (disablePlacementScript == null)
                {
                    disablePlacementScript = args.placementObject.GetComponentInChildren<DisablePlacement>(); // Search children as well
                }
            }
            else
            {
                // Find the specific prefab root within the placed object
                Transform prefabRoot = args.placementObject.transform.Find(placedPrefabRootName);

                if (prefabRoot != null)
                {
                    // Get the DisablePlacement script from the prefab root (or its children)
                    disablePlacementScript = prefabRoot.GetComponent<DisablePlacement>();
                    if (disablePlacementScript == null)
                    {
                        disablePlacementScript = prefabRoot.GetComponentInChildren<DisablePlacement>(); // Search children as well
                    }
                }
                else
                {
                    Debug.LogWarning($"GameObject '{placedPrefabRootName}' not found within the placed object.");
                }
            }

            if (disablePlacementScript != null)
            {
                disablePlacementScript.StartFadingPlaneLines();
            }
            else
            {
                Debug.LogWarning("DisablePlacement script not found on the placed object or within its specified prefab root.");
            }
        }
    }
}