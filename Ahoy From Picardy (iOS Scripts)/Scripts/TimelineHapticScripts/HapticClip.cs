using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class HapticClip : PlayableAsset
{
    public HapticType hapticType = HapticType.LightImpact; // Default for single-shot or base type

    // Add this for sustained haptics
    [Tooltip("Defines the intensity ramp over the clip duration for sustained haptics.")]
    public AnimationCurve intensityRamp = AnimationCurve.EaseInOut(0, 0f, 1, 1f); // Default to a smooth ramp

    public override double duration
    {
        // For sustained haptics, duration is very important. Let's make it editable in Timeline.
        // Returning a large number will make clips default to that length.
        // Or you can expose a public 'double clipDuration' field.
        // For now, let's keep it simple: if you set a clip to 10s, it'll use 10s.
        get { return base.duration; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<HapticPlayableBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();

        // Pass the selected haptic type and the intensity ramp to the behaviour
        behaviour.hapticType = hapticType;
        behaviour.intensityRamp = intensityRamp; // Pass the curve here

        return playable;
    }
}