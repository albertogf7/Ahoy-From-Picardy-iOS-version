using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class HapticFeedback : MonoBehaviour
{
    // Cooldown settings for impact types
    [Header("Impact Haptics Cooldowns")]
    [Tooltip("Minimum time (in seconds) between Light Impact haptic feedback calls.")]
    [SerializeField]
    private float lightImpactCooldown = 0.3f;

    [Tooltip("Minimum time (in seconds) between Medium Impact haptic feedback calls.")]
    [SerializeField]
    private float mediumImpactCooldown = 0.2f;

    [Tooltip("Minimum time (in seconds) between Heavy (Strong) Impact haptic feedback calls.")]
    [SerializeField]
    private float heavyImpactCooldown = 0.1f;

    // Global cooldown for notification types
    [Header("Notification Haptics Cooldown")]
    [Tooltip("Minimum time (in seconds) between any Notification (Success, Warning, Error) haptic feedback calls.")]
    [SerializeField]
    private float notificationGlobalCooldown = 0.6f; // A slightly longer cooldown for distinct notifications

    // A dictionary to store the last time each HapticType was played
    private Dictionary<HapticType, float> _lastHapticPlayTime = new Dictionary<HapticType, float>();

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern bool _IsHapticsSupported();

    [DllImport("__Internal")]
    private static extern void _PlayHaptic(int hapticType);
#endif

    void Awake()
    {
        // Initialize dictionary with negative values to ensure first play is always allowed
        foreach (HapticType type in System.Enum.GetValues(typeof(HapticType)))
        {
            _lastHapticPlayTime[type] = -1f;
        }
    }

    public bool IsHapticsSupported()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _IsHapticsSupported();
#else
        return false;
#endif
    }

    /// <summary>
    /// Play haptic feedback of the specified type, respecting cooldowns.
    /// This function will only execute on iOS devices that support haptics.
    /// </summary>
    /// <param name="type">The type of haptic feedback to play (e.g., LightImpact, Success).</param>
    public void PlayHaptic(HapticType type)
    {
        float requiredCooldown = GetCooldownForType(type);

        // Implement the cooldown check here
        if (Time.unscaledTime - _lastHapticPlayTime[type] < requiredCooldown)
        {
            // Debug.Log($"[Haptics] Haptic of type {type.ToString()} throttled by cooldown ({requiredCooldown}s).");
            return; // Don't play haptic if still on cooldown
        }

#if UNITY_IOS && !UNITY_EDITOR
        if (IsHapticsSupported())
        {
            _PlayHaptic((int)type);
            _lastHapticPlayTime[type] = Time.unscaledTime; // Update last haptic time ONLY if played
        }
#else
        Debug.Log($"[Haptics] PlayHaptic called: {type.ToString()} (iOS Haptics not supported in Editor)");
        _lastHapticPlayTime[type] = Time.unscaledTime; // Simulate cooldown in Editor as well
#endif
    }

    /// <summary>
    /// Determines the correct cooldown duration based on the HapticType.
    /// </summary>
    private float GetCooldownForType(HapticType type)
    {
        switch (type)
        {
            case HapticType.LightImpact:
                return lightImpactCooldown;
            case HapticType.MediumImpact:
                return mediumImpactCooldown;
            case HapticType.HeavyImpact:
                return heavyImpactCooldown;
            case HapticType.Success:
            case HapticType.Warning:
            case HapticType.Error:
                return notificationGlobalCooldown;
            default:
                return 0.0f; // No cooldown by default for unknown types, or throw an error.
        }
    }

    // --- Backward Compatibility / Convenience Methods (call PlayHaptic) ---
    public void PlayLightHaptic()
    {
        PlayHaptic(HapticType.LightImpact);
    }

    public void PlayMediumHaptic()
    {
        PlayHaptic(HapticType.MediumImpact);
    }

    public void PlayStrongHaptic()
    {
        PlayHaptic(HapticType.HeavyImpact);
    }

    // --- New Haptic Types Convenience Methods (call PlayHaptic) ---
    public void PlaySuccessHaptic()
    {
        PlayHaptic(HapticType.Success);
    }

    public void PlayWarningHaptic()
    {
        PlayHaptic(HapticType.Warning);
    }

    public void PlayErrorHaptic()
    {
        PlayHaptic(HapticType.Error);
    }
}
