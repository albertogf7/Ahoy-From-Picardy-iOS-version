using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Handles input for non-AR 3D scenes (Prologue, Epilogue) to toggle a UI menu.
/// Toggles the menu if the screen is tapped and no UI element or 3D object is hit.
/// </summary>
public class MenuHandler : MonoBehaviour
{
    [Tooltip("Assign the root GameObject of your UI Menu here (e.g., the Canvas or a panel).")]
    [SerializeField] private GameObject _uiMenu;

    [Tooltip("Optional: Tag assigned to the UI Menu GameObject if not assigned directly.")]
    [SerializeField] private string uiMenuTag = "UIMenu";

    private EventSystem _eventSystem;
    private bool _isTapping = false;
    [SerializeField]
    private bool _onTitleScreen;

    void Awake()
    {
        _eventSystem = EventSystem.current;
        if (_eventSystem == null)
        {
            Debug.LogError("MenuHandler: EventSystem not found! Please ensure an EventSystem exists in your scene (GameObject -> UI -> Event System).");
        }

        if (_uiMenu == null && !string.IsNullOrEmpty(uiMenuTag))
        {
            _uiMenu = GameObject.FindGameObjectWithTag(uiMenuTag);
            if (_uiMenu == null)
            {
                Debug.LogWarning($"MenuHandler: UI Menu with tag '{uiMenuTag}' not found. Please assign it in the Inspector or ensure it has the correct tag.");
            }
        }

        if (_uiMenu != null && _uiMenu.activeSelf)
        {
            _uiMenu.SetActive(false);
            Debug.Log("MenuHandler: UI Menu initially hidden.");
        }
    }

    void Update()
    {
        if (Touchscreen.current != null)
        {
            var primaryTouch = Touchscreen.current.primaryTouch;

            if (primaryTouch.press.isPressed && !_isTapping)
            {
                _isTapping = true;
                Vector2 touchPosition = primaryTouch.position.ReadValue();
                ProcessTap(touchPosition);
            }
            else if (!primaryTouch.press.isPressed && _isTapping)
            {
                _isTapping = false;
            }
        }
        else if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                ProcessTap(mousePosition);
            }
        }
    }

    private void ProcessTap(Vector2 inputPosition)
    {
        if (_eventSystem != null && IsTouchOverUI(inputPosition))
        {
            Debug.Log("MenuHandler: Tap ignored. UI element was clicked.");
            return;
        }

        if (_uiMenu != null)
        {
            ToggleUIMenu();
            Debug.Log("MenuHandler: UI was not hit. UI Menu Toggled.");
        }
        else
        {
            Debug.LogWarning("MenuHandler: UI Menu GameObject is null, cannot toggle menu.");
        }
    }

    /// <summary>
    /// Manually raycasts UI elements to detect if a touch hit UI.
    /// Works reliably in Editor and mobile builds.
    /// </summary>
    private bool IsTouchOverUI(Vector2 screenPosition)
    {
        if (_eventSystem == null) return false;

        PointerEventData pointerData = new PointerEventData(_eventSystem)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        _eventSystem.RaycastAll(pointerData, results);
        return results.Count > 0;
    }

    private void ToggleUIMenu()
    {
        if (_uiMenu != null && !_onTitleScreen)
        {
            _uiMenu.SetActive(!_uiMenu.activeSelf);
            Debug.Log($"MenuHandler: UI Menu Visibility set to: {_uiMenu.activeSelf}");
        }
    }
    public void NotOnTitleScreen()
    {
        _onTitleScreen = false;
    }
}