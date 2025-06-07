using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AhoyARInput : MonoBehaviour
{
    [SerializeField]
    private Camera _arCamera; // Reference to the AR Camera
    [SerializeField]
    private string uiMenuTag = "UIMenu"; // Tag assigned to the UI Basic Layout (UI Menu) prefab instance

    [Tooltip("Maximum duration (in seconds) for a tap to be considered a 'single tap' and toggle the menu.")]
    [SerializeField]
    private float tapToggleMaxDuration = 0.3f; // Adjust this value (e.g., 0.2s to 0.5s)

    private GameObject uiMenu; // Reference to the instantiated UI Menu

    private bool _isTapping = false; // Tracks if a touch is currently pressed down
    private bool _isOnLoop = false; // Controls menu visibility during certain loops

    private EventSystem eventSystem;

    // New: Variables to track tap duration on the AhoyARPrefab
    private float _ahoyPrefabTapStartTime = -1f;
    private GameObject _currentlyTouchedAhoyPrefab = null;


    void Start()
    {
        eventSystem = EventSystem.current;
        ARPlacementEventForwarder.OnCustomObjectPlacedEvent += HandleObjectPlaced;

        // No need to subscribe to CustomARInteractionManager.OnAnyManipulationStarted with this approach
        // CustomARInteractionManager.OnAnyManipulationStarted += HandleManipulationStarted;


        GameObject initialUIMenu = GameObject.FindGameObjectWithTag(uiMenuTag);
        if (initialUIMenu != null && initialUIMenu.activeSelf)
        {
            initialUIMenu.SetActive(false);
            uiMenu = initialUIMenu;
        }
    }

    void OnDestroy()
    {
        ARPlacementEventForwarder.OnCustomObjectPlacedEvent -= HandleObjectPlaced;
        // No need to unsubscribe if not subscribed
        // CustomARInteractionManager.OnAnyManipulationStarted -= HandleManipulationStarted;
    }

    void Update()
    {
        if (Touchscreen.current != null)
        {
            var primaryTouch = Touchscreen.current.primaryTouch;

            if (primaryTouch.press.isPressed && !_isTapping) // Touch just started
            {
                _isTapping = true;
                Vector2 touchPosition = primaryTouch.position.ReadValue();
                HandleTouch(touchPosition); // Process initial touch down
            }
            else if (!primaryTouch.press.isPressed && _isTapping) // Touch just released
            {
                _isTapping = false; // Reset the flag when the touch ends

                // NEW: Process tap release for AhoyARPrefab
                if (_currentlyTouchedAhoyPrefab != null)
                {
                    float tapDuration = Time.unscaledTime - _ahoyPrefabTapStartTime;
                    if (tapDuration < tapToggleMaxDuration)
                    {
                        // It was a short tap on AhoyARPrefab, so toggle the menu
                        if (uiMenu != null && !_isOnLoop)
                        {
                            UIMenuHandler();
                            Debug.Log($"AhoyARInput: AhoyARPrefab short tap ({tapDuration:F2}s). UI Menu Toggled.");
                        }
                    }
                    else
                    {
                        Debug.Log($"AhoyARInput: AhoyARPrefab long tap/gesture ({tapDuration:F2}s). Menu not toggled.");
                    }
                    // Reset tap tracking variables
                    _ahoyPrefabTapStartTime = -1f;
                    _currentlyTouchedAhoyPrefab = null;
                }
            }
        }
        // No Mouse input handling in ARInput as it's typically touch-only.
        // If you need mouse for editor testing, re-add the Mouse.current logic.
    }

    private void HandleObjectPlaced(GameObject placedObject)
    {
        uiMenu = FindUIMenuInHierarchy(placedObject);

        if (uiMenu != null)
        {
            Debug.Log("UI Menu found via OnCustomObjectPlaced event!");
            if (uiMenu.activeSelf)
            {
                uiMenu.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("UI Menu with tag '" + uiMenuTag + "' not found in the placed object's hierarchy.");
        }
    }

    private GameObject FindUIMenuInHierarchy(GameObject parent)
    {
        if (parent.CompareTag(uiMenuTag))
        {
            return parent;
        }

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject found = FindUIMenuInHierarchy(parent.transform.GetChild(i).gameObject);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private void HandleTouch(Vector2 touchPosition)
    {
        // NEW: Explicitly perform a UI raycast using EventSystem.RaycastAll
        if (eventSystem != null)
        {
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = touchPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, results);

            if (results.Count > 0)
            {
                Debug.Log("Touch ignored: UI was clicked. Hit: " + results[0].gameObject.name);
                // Reset AhoyPrefab tap tracking if UI was hit
                _ahoyPrefabTapStartTime = -1f;
                _currentlyTouchedAhoyPrefab = null;
                return;
            }
        }
        else
        {
            Debug.LogWarning("EventSystem is null in HandleTouch. UI clicks may not be ignored.");
        }
        Ray ray = _arCamera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HandleRaycastHit(hit); // Go to detailed hit handling
        }
        else // Hit nothing (empty space)
        {
            // If nothing is hit, toggle menu immediately (unless _isOnLoop prevents it)
            if (uiMenu != null && !_isOnLoop)
            {
                UIMenuHandler();
                Debug.Log("Hit No Object. UI Menu Toggled.");
            }
            // Reset AhoyPrefab tap tracking if empty space was hit
            _ahoyPrefabTapStartTime = -1f;
            _currentlyTouchedAhoyPrefab = null;
        }
    }

    private void HandleRaycastHit(RaycastHit hit)
    {
        GameObject hitObject = hit.collider.gameObject;

        // Reset AhoyPrefab tap tracking by default, unless this specific hit is the AhoyPrefab
        _ahoyPrefabTapStartTime = -1f;
        _currentlyTouchedAhoyPrefab = null;

        // Check for specific interactive story elements (Pipete, SingleTap, Pillar)
        // If these are hit, their specific behavior should execute, and menu should NOT toggle.
        var pipete = hitObject.GetComponent<PipeteBehaviour>();
        if (pipete != null)
        {
            pipete.Drop();
            Debug.Log("Drop true");
            return; // Exit: Specific interaction handled, no menu toggle.
        }

        var singleTap = hitObject.GetComponent<SingleTapAnimation>();
        if (singleTap != null)
        {
            singleTap.SingleTap();
            Debug.Log("SingleTapObject");
            return; // Exit: Specific interaction handled, no menu toggle.
        }

        var pillar = hitObject.GetComponent<PillarButton>();
        if (pillar != null)
        {
            pillar.CallTracks();
            Debug.Log("Pillar touched");
            return; // Exit: Specific interaction handled, no menu toggle.
        }

        // Special handling for the AhoyARPrefab (the object with HapticFeedback)
        var globe = hitObject.GetComponent<HapticFeedback>();
        if (globe != null)
        {
            Debug.Log("AhoyARPrefab touched on press. Will check duration on release.");
            // NEW: Record start time and reference for AhoyARPrefab
            _ahoyPrefabTapStartTime = Time.unscaledTime;
            _currentlyTouchedAhoyPrefab = hitObject;
            return; // Exit: Menu toggle for AhoyARPrefab will happen on release based on duration.
        }

        // Default behavior for any other 3D object hit (not UI, not specific interactable, not AhoyARPrefab).
        // If you hit a random 3D object that isn't one of the above, you want the menu to toggle immediately.
        if (uiMenu != null && !_isOnLoop)
        {
            UIMenuHandler();
            Debug.Log("Hit an unrecognized 3D object. UI Menu Toggled.");
        }
    }

    private void UIMenuHandler()
    {
        if (uiMenu != null)
        {
            uiMenu.SetActive(!uiMenu.activeSelf);
        }
        else
        {
            Debug.LogWarning("UI Menu GameObject not found. Ensure it has the correct tag: " + uiMenuTag);
        }
    }

    public void disableUIOnLoop()
    {
        _isOnLoop = true;
        if (uiMenu != null)
            uiMenu.gameObject.SetActive(false);
    }

    public void enableUIOnLoop()
    {
        _isOnLoop = false;
    }
}