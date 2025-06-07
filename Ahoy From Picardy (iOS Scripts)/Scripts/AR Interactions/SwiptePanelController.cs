using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SwiptePanelController : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] panels; // Assign the UI panels in the Inspector
    [SerializeField]
    private float transitionSpeed = 6f; // Smooth movement speed
    [SerializeField]
    private Button leftButton;
    [SerializeField]
    private Button rightButton;

    private int currentPanelIndex = 0;
    private float panelWidth;

    void Start()
    {
        if (panels.Length == 0)
        {
            Debug.LogError("No panels assigned in the Inspector!");
            enabled = false; // Disable the script if no panels are set
            return;
        }
        if (leftButton == null || rightButton == null)
        {
            Debug.LogError("Left or Right button not assigned in the Inspector!");
            enabled = false; // Disable if buttons are missing
            return;
        }

        panelWidth = panels[0].rect.width; // Get panel width for movement
        UpdatePanelPositions();
        UpdateButtonsVisibility(); // Initial button visibility setup
    }

    void Update()
    {
        SmoothMovePanels();
    }

    public void MoveToPreviousPanel()
    {
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            UpdateButtonsVisibility();
        }
    }

    public void MoveToNextPanel()
    {
        if (currentPanelIndex < panels.Length - 1)
        {
            currentPanelIndex++;
            UpdateButtonsVisibility();
        }
    }

    void SmoothMovePanels()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            float targetX = (i - currentPanelIndex) * panelWidth;
            panels[i].anchoredPosition = Vector2.Lerp(panels[i].anchoredPosition, new Vector2(targetX, 0), Time.deltaTime * transitionSpeed);
        }
    }

    void UpdatePanelPositions()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].anchoredPosition = new Vector2((i - currentPanelIndex) * panelWidth, 0);
        }
    }

    void UpdateButtonsVisibility()
    {
        if (leftButton != null)
        {
            leftButton.gameObject.SetActive(currentPanelIndex > 0);
        }

        if (rightButton != null)
        {
            rightButton.gameObject.SetActive(currentPanelIndex < panels.Length - 1);
        }
    }
}