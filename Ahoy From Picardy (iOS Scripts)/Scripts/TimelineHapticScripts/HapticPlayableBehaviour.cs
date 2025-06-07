using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HapticPlayableBehaviour : PlayableBehaviour
{
    public HapticType hapticType;
    public AnimationCurve intensityRamp = AnimationCurve.EaseInOut(0, 0f, 1, 1f);

    [HideInInspector] public HapticFeedback hapticFeedbackComponent; // This will now be set by the mixer

    // OnGraphStart and OnBehaviourPlay will NOT try to find the component themselves anymore.
    // They will assume hapticFeedbackComponent is set by the mixer's ProcessFrame.
    public override void OnGraphStart(Playable playable)
    {
        // No direct assignment here. Will be set by mixer or in ProcessFrame.
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (Application.isPlaying)
        {
            if (hapticFeedbackComponent != null)
            {
                hapticFeedbackComponent.PlayHaptic(hapticType);
                Debug.Log($"[Haptics] Clip starting: {hapticType.ToString()} - Initial Haptic.");
            }
            else
            {
                Debug.LogWarning("HapticFeedback component not set in OnBehaviourPlay. Haptic will rely on ProcessFrame (if in Play Mode).");
            }
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // No explicit "stop haptic" needed for impulse-based haptics.
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

        if (Application.isPlaying)
        {
            if (hapticFeedbackComponent == null)
            {
                // This is a critical error if it occurs during actual play mode.
                // It means your Timeline binding is not working correctly.
                Debug.LogError("HapticFeedback component is NULL in ProcessFrame during PLAY MODE. Ensure Timeline binding is correct and HapticFeedback object is active.");
                return; // Prevent NullReferenceException for subsequent calls
            }

            float timeInClip = (float)(playable.GetTime() / playable.GetDuration());
            float curveValue = intensityRamp.Evaluate(timeInClip);

            HapticType currentHapticType;

            if (curveValue <= 0.33f)
            {
                currentHapticType = HapticType.LightImpact;
            }
            else if (curveValue <= 0.66f)
            {
                currentHapticType = HapticType.MediumImpact;
            }
            else // curveValue > 0.66f
            {
                currentHapticType = HapticType.HeavyImpact;
            }
            hapticFeedbackComponent.PlayHaptic(currentHapticType);
        }
    }
}
