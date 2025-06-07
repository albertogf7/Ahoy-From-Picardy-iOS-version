using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class CustomARRotationInteractable : ARRotationInteractable
{
    private HapticFeedback hapticFeedback;
    [SerializeField]
    private CustomARInteractionManager _interactionManager;

    new void Awake()
    {
        base.Awake();
        hapticFeedback = GetComponent<HapticFeedback>();
    }

    protected override void OnStartManipulation(DragGesture gesture)
    {
        base.OnStartManipulation(gesture);
        hapticFeedback?.PlayStrongHaptic();
        _interactionManager?.OnRotateStart();
    }

    protected override void OnStartManipulation(TwistGesture gesture)
    {
        base.OnStartManipulation(gesture);
        hapticFeedback?.PlayStrongHaptic();
        _interactionManager?.OnRotateStart();
    }

    protected override void OnContinueManipulation(DragGesture gesture)
    {
        base.OnContinueManipulation(gesture);
        hapticFeedback?.PlayMediumHaptic();
    }

    protected override void OnContinueManipulation(TwistGesture gesture)
    {
        base.OnContinueManipulation(gesture);
        hapticFeedback?.PlayMediumHaptic();
    }

    protected override void OnEndManipulation(DragGesture gesture)
    {
        base.OnEndManipulation(gesture);
        hapticFeedback?.PlayStrongHaptic();
        _interactionManager?.OnRotateEnd();
    }

    protected override void OnEndManipulation(TwistGesture gesture)
    {
        base.OnEndManipulation(gesture);
        hapticFeedback?.PlayStrongHaptic();
        _interactionManager?.OnRotateEnd();
    }
}
