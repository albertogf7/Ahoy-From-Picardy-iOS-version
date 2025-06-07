using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class CustomARScaleInteractable : ARScaleInteractable
{
    private HapticFeedback hapticFeedback;
    private CustomARInteractionManager _interactionManager;

    new void Awake()
    {
        base.Awake();
        hapticFeedback = GetComponent<HapticFeedback>();
    }

    protected override void OnStartManipulation(PinchGesture gesture)
    {
        base.OnStartManipulation(gesture);
        hapticFeedback?.PlayStrongHaptic();
        _interactionManager?.OnScaleStart();
    }

    protected override void OnContinueManipulation(PinchGesture gesture)
    {
        base.OnContinueManipulation(gesture);
        hapticFeedback?.PlayMediumHaptic();
    }

    protected override void OnEndManipulation(PinchGesture gesture)
    {
        base.OnEndManipulation(gesture);
        hapticFeedback?.PlayStrongHaptic();
        _interactionManager?.OnScaleEnd();
    }
}
