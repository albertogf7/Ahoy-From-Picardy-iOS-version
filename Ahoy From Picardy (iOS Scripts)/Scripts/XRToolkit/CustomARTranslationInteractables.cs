using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class CustomARTranslationInteractables : ARTranslationInteractable
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
        _interactionManager?.OnTranslateStart();
    }

    protected override void OnContinueManipulation(DragGesture gesture)
    {
        base.OnContinueManipulation(gesture);
        hapticFeedback?.PlayMediumHaptic();
    }

    protected override void OnEndManipulation(DragGesture gesture)
    {
        base.OnEndManipulation(gesture);
        hapticFeedback?.PlayStrongHaptic();
        _interactionManager?.OnTranslateEnd();
    }
}
