using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PinataInteraction : MonoBehaviour
{
    [Tooltip("The Rigidbody of the swinging piñata. Drag the GameObject here.")]
    [SerializeField] private Rigidbody pinataRb;

    [Header("Force Settings")]
    [Tooltip("Force applied when a simple tap is detected.")]
    [SerializeField] private float tapForce = 7f;
    [Tooltip("Force multiplier applied when a swipe is detected. Swipe distance will also influence force.")]
    [SerializeField] private float swipeForceMultiplier = 20f;
    [Tooltip("Additional torque applied during a swipe to induce spin.")]
    [SerializeField] private float swipeTorqueMultiplier = 5f;

    [Header("Input Detection Thresholds")]
    [Tooltip("Maximum pixel distance a touch/mouse can move to still be considered a tap.")]
    [SerializeField] private float minSwipeDistance = 50f;
    [Tooltip("Maximum duration a touch/mouse can be held down to be considered a tap.")]
    [SerializeField] private float maxTapDuration = 0.2f;

    [Header("Audio Feedback")]
[Tooltip("AudioSource component to play sounds.")]
[SerializeField] private AudioSource audioSource;
[Tooltip("Sound played when the piñata is hit (tap or swipe).")]
[SerializeField] private AudioClip missSound;
[Tooltip("Sound played for a tap hit (thud).")]
[SerializeField] private AudioClip tapThudSound;
[Tooltip("Sound played for a swipe hit (louder thud/smash).")]
[SerializeField] private AudioClip swipeThudSound;

    private Vector2 swipeStartScreenPos;
    private float touchStartTime;
    private bool isTouching = false;

    private Camera mainCamera;

    private void Start()
    {
        if (pinataRb == null)
        {
            pinataRb = GetComponent<Rigidbody>();
            if (pinataRb == null)
            {
                Debug.LogError("PinataInteraction: Rigidbody not assigned and not found on this GameObject. " +
                               "Please assign it in the Inspector or ensure it's on this GameObject or a reachable child.", this);
                enabled = false;
                return;
            }
        }

        // Attempt to find the main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("PinataInteraction: Main Camera not found! Please ensure your camera is tagged 'MainCamera'.", this);
            enabled = false; // Disable script if no camera to raycast from
            return;
        }
    }

    private void Update()
    {
        // Re-check for main camera in case it's added/changed at runtime
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return; // Still no camera, can't process input
        }

        // --- Handle Touch Input (for mobile) ---
        if (Touchscreen.current != null)
        {
            var primaryTouch = Touchscreen.current.primaryTouch;

            if (primaryTouch.press.wasPressedThisFrame)
            {
                isTouching = true;
                swipeStartScreenPos = primaryTouch.position.ReadValue();
                touchStartTime = Time.unscaledTime; // Use unscaledTime for consistent duration detection
            }
            else if (primaryTouch.press.wasReleasedThisFrame && isTouching)
            {
                isTouching = false; // Reset touch state
                Vector2 swipeEndScreenPos = primaryTouch.position.ReadValue();
                float touchDuration = Time.unscaledTime - touchStartTime;

                ProcessInputRelease(swipeStartScreenPos, swipeEndScreenPos, touchDuration);
            }
        }
        // --- Handle Mouse Input (for Editor testing) ---
        else if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                isTouching = true;
                swipeStartScreenPos = Mouse.current.position.ReadValue();
                touchStartTime = Time.unscaledTime;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame && isTouching)
            {
                isTouching = false; // Reset touch state
                Vector2 swipeEndScreenPos = Mouse.current.position.ReadValue();
                float touchDuration = Time.unscaledTime - touchStartTime;

                ProcessInputRelease(swipeStartScreenPos, swipeEndScreenPos, touchDuration);
            }
        }
    }

    /// <summary>
    /// Central logic for processing input release (tap or swipe).
    /// Determines if the piñata was hit and calls the appropriate force function.
    /// </summary>
    /// <param name="startPos">Screen position where the input started.</param>
    /// <param name="endPos">Screen position where the input ended.</param>
    /// <param name="duration">Duration of the input press.</param>
    private void ProcessInputRelease(Vector2 startPos, Vector2 endPos, float duration)
    {
        float distance = Vector2.Distance(startPos, endPos);

        Ray ray = mainCamera.ScreenPointToRay(startPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.rigidbody == pinataRb)
        {
            // It was a direct hit on the piñata
            if (duration < maxTapDuration && distance < minSwipeDistance)
            {
                ApplyTapForce(hit.point);
            }
            else // If it moved enough or was held long enough, consider it a swipe/stronger strike
            {
                ApplySwipeForce(hit.point, startPos, endPos);
            }
        }
        else // The input didn't start directly on the piñata
        {
            if (audioSource != null && missSound != null)
            {
                audioSource.PlayOneShot(missSound); 
            }
            Debug.Log("Piñata missed!");
        }
    }


    /// <summary>
    /// Applies a force to the piñata from a tap.
    /// </summary>
    /// <param name="hitPoint">The world position where the tap hit the piñata.</param>
    private void ApplyTapForce(Vector3 hitPoint)
    {
        Vector3 forceDirection = (hitPoint - mainCamera.transform.position).normalized;
        
        if (forceDirection.y < 0) forceDirection.y = 0.1f;
        forceDirection.Normalize();

        pinataRb.AddForceAtPosition(forceDirection * tapForce, hitPoint, ForceMode.Impulse);
        if (audioSource != null && tapThudSound != null)
        {
            audioSource.PlayOneShot(tapThudSound);
        }
        Debug.Log("Piñata tapped at: " + hitPoint + " with force: " + forceDirection * tapForce);
    }

    /// <summary>
    /// Applies a stronger force and optional torque to the piñata from a swipe.
    /// </summary>
    /// <param name="hitPoint">The world position where the swipe originated on the piñata.</param>
    /// <param name="swipeStartScreen">The screen position where the swipe started.</param>
    /// <param name="swipeEndScreen">The screen position where the swipe ended.</param>
    private void ApplySwipeForce(Vector3 hitPoint, Vector2 swipeStartScreen, Vector2 swipeEndScreen)
    {
        // Get the depth (Z coordinate) of the hit point on the piñata relative to the camera
        float hitPointZDepth = mainCamera.WorldToScreenPoint(hitPoint).z;

        // Convert the screen swipe start and end points to world points at that depth
        Vector3 worldSwipeStart = mainCamera.ScreenToWorldPoint(new Vector3(swipeStartScreen.x, swipeStartScreen.y, hitPointZDepth));
        Vector3 worldSwipeEnd = mainCamera.ScreenToWorldPoint(new Vector3(swipeEndScreen.x, swipeEndScreen.y, hitPointZDepth));

        // Calculate the force direction as the vector between these two world points
        Vector3 forceDirection = (worldSwipeEnd - worldSwipeStart).normalized;

        // Apply force scaled by swipe distance and multiplier
        float swipeMagnitude = Vector2.Distance(swipeStartScreen, swipeEndScreen);
        float actualForce = swipeForceMultiplier * (swipeMagnitude / minSwipeDistance); 
        pinataRb.AddForceAtPosition(forceDirection * actualForce, hitPoint, ForceMode.Impulse);

        if (audioSource != null && swipeThudSound != null)
        {
            audioSource.PlayOneShot(swipeThudSound);
        }

        // Calculate Torque (for spinning effect)
        // Torque axis perpendicular to the force direction and the piñata's "up"
        Vector3 torqueAxis = Vector3.Cross(forceDirection, pinataRb.transform.up).normalized;
        float actualTorque = swipeTorqueMultiplier * (swipeMagnitude / minSwipeDistance);
        pinataRb.AddTorque(torqueAxis * actualTorque, ForceMode.Impulse);

        Debug.Log($"Piñata swiped! Force: {forceDirection * actualForce}, Torque: {torqueAxis * actualTorque}");
    }
}