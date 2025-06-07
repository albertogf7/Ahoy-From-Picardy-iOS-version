using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HapticTrackMixer : PlayableBehaviour
{
    // This will hold the reference to the HapticFeedback component bound to the track.
    private HapticFeedback _boundHapticFeedback;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // Get the HapticFeedback component from playerData.
        // This 'playerData' is what's assigned in the Timeline track binding.
        _boundHapticFeedback = playerData as HapticFeedback;

        if (_boundHapticFeedback == null)
        {
            Debug.LogError("HapticTrack is not bound to a HapticFeedback component. Haptics will not play.");
            return;
        }

        // Iterate through all active clips on this track.
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            // Get the input weight (how much this clip contributes)
            float inputWeight = playable.GetInputWeight(i);
            
            // Check if the clip is active (has a weight greater than 0)
            if (inputWeight > 0f)
            {
                // Get the playable behaviour for this input clip
                ScriptPlayable<HapticPlayableBehaviour> inputPlayable = (ScriptPlayable<HapticPlayableBehaviour>)playable.GetInput(i);
                HapticPlayableBehaviour behaviour = inputPlayable.GetBehaviour();

                // Pass the bound HapticFeedback component to the individual clip behaviour.
                // This ensures the behaviour always has a valid reference.
                behaviour.hapticFeedbackComponent = _boundHapticFeedback;

                // For a mixer, this is where you'd combine results if needed.
                // For our haptics, the individual clip's ProcessFrame (in HapticPlayableBehaviour)
                // will call PlayHaptic, and the HapticFeedback component's cooldowns will handle the logic.
                // We just need to ensure the reference is passed.
            }
        }
    }
}