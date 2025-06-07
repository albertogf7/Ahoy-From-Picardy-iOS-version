using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class AlwaysSelectedInteractable : ARSelectionInteractable
{
    [SerializeField, Tooltip("The visualization GameObject that will become active when the object is 'selected'.")]
    GameObject m_AlwaysSelectionVisualization;

    /// <summary>
    /// The visualization <see cref="GameObject"/> that will become active.
    /// </summary>
    public GameObject alwaysSelectionVisualization
    {
        get => m_AlwaysSelectionVisualization;
        set => m_AlwaysSelectionVisualization = value;
    }

    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        // We always allow manipulation (selection) on tap.
        return true;
    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        base.OnEndManipulation(gesture);
        // We don't directly set m_GestureSelected here as it's not accessible.
        // Our IsSelectableBy override will handle the always-selected logic.
    }

    /// <inheritdoc />
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        // We are always selectable by an ARGestureInteractor.
        return interactor is ARGestureInteractor;
    }

    /// <inheritdoc />
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        if (m_AlwaysSelectionVisualization != null)
        {
            m_AlwaysSelectionVisualization.SetActive(true);
        }
    }

    /// <inheritdoc />
    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        // We override this to prevent the visualization from being disabled.
        // We don't call base.OnSelectExiting to skip the original logic.
        if (m_AlwaysSelectionVisualization != null)
        {
            m_AlwaysSelectionVisualization.SetActive(true); // Ensure it stays active
        }
    }
}