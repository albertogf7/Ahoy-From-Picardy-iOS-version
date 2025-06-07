using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GyroParallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("Maximum tilt angle (in degrees) to reach max offset.")]
    public float maxTiltAngle = 30f;
    [Tooltip("Smoothing factor (0-1).")]
    [Range(0f, 1f)]
    public float smoothingFactor = 0.1f;

    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform targetObject;
        [Tooltip("Maximum horizontal offset for this layer.")]
        public float maxOffset = 0.5f;
        [HideInInspector]
        public Vector3 initialLocalPosition;
    }

    [Header("Midground Layer")]
    public ParallaxLayer midground;

    [Header("Foreground Layer")]
    public ParallaxLayer foreground;

    private bool _gyroEnabled = false;
    public bool gyroEnabled => _gyroEnabled; // Public getter
    private UnityEngine.InputSystem.Gyroscope _gyroscope;
    public UnityEngine.InputSystem.Gyroscope gyroscope => _gyroscope; // Public getter
    private float accumulatedTiltAngleY = 0f;
    private float smoothedTiltAngleY = 0f;

    void Start()
    {
        EnableGyro();
        if (midground.targetObject != null) midground.initialLocalPosition = midground.targetObject.localPosition;
        if (foreground.targetObject != null) foreground.initialLocalPosition = foreground.targetObject.localPosition;
    }

    void EnableGyro()
    {
        _gyroscope = UnityEngine.InputSystem.Gyroscope.current;
        if (_gyroscope != null)
        {
            InputSystem.EnableDevice(_gyroscope);
            _gyroEnabled = true;
            Debug.Log("Gyroscope (New Input System) enabled.");
        }
        else
        {
            Debug.LogWarning("Gyroscope not found or not supported by the New Input System on this device.");
            enabled = false;
        }
    }

    // Public method to disable the gyroscope
    public void DisableGyroscope()
    {
        if (_gyroEnabled && _gyroscope != null)
        {
            InputSystem.DisableDevice(_gyroscope);
            _gyroEnabled = false;
            Debug.Log("Gyroscope disabled via public method.");
        }
    }

    void Update()
    {
        if (!_gyroEnabled || _gyroscope == null) return;

        // Get the rotation rate around the Y-axis (degrees per second)
        float rotationRateY = _gyroscope.angularVelocity.ReadValue().y;

        // Integrate the rotation rate to get the accumulated tilt angle
        accumulatedTiltAngleY += rotationRateY * Time.deltaTime * Mathf.Rad2Deg; // Convert to degrees

        // Clamp the accumulated tilt angle to your desired range (optional, but can prevent runaway values)
        accumulatedTiltAngleY = Mathf.Clamp(accumulatedTiltAngleY, -maxTiltAngle, maxTiltAngle);

        // Apply Exponential Moving Average (EMA) for smoothing the tilt angle
        smoothedTiltAngleY = smoothingFactor * accumulatedTiltAngleY + (1f - smoothingFactor) * smoothedTiltAngleY;

        // Normalize the smoothed tilt angle to a range of -1 to 1
        float normalizedTilt = Mathf.Clamp(smoothedTiltAngleY / maxTiltAngle, -1f, 1f);

        // Apply parallax to each layer
        ApplyParallax(midground, normalizedTilt, midground.maxOffset);
        ApplyParallax(foreground, normalizedTilt, foreground.maxOffset);
    }

    void ApplyParallax(ParallaxLayer layer, float normalizedTilt, float layerMaxOffset)
    {
        if (layer.targetObject != null)
        {
            float xOffset = normalizedTilt * layerMaxOffset;
            Vector3 original = layer.initialLocalPosition;
            layer.targetObject.localPosition = new Vector3(original.x + xOffset, original.y, original.z);
        }
    }
}