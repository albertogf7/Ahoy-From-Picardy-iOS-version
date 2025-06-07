using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    #region Handlers
    [SerializeField] private PlayableDirector _timeline; // Single top-level timeline
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _ccButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _prevButton;
    [SerializeField] private TMP_Text _subtitleText;
    [SerializeField] private Sprite _ccIconOn;
    [SerializeField] private Sprite _ccIconOff;
    [SerializeField] private TimelineChapterNavigation _chapterNavigation;

    private GameManager _gameManager;
    private bool _isPlaying = true;
    private bool _ccEnabledState = false;
    private Image _ccButtonImage;

    private bool _isChapterSkipping = false;
    private float _skipDebounceTime = 0.4f;

    private bool _isToggling = false;
    #endregion

    #region Start Method
    void Start()
    {
        Debug.Log("UIController Start called");
        _playButton.onClick.AddListener(PlayTimeline);
        _pauseButton.onClick.AddListener(PauseTimeline);
        _restartButton.onClick.AddListener(RestartScene);
        _ccButton.onClick.AddListener(ToggleClosedCaptions);
        _nextButton.onClick.AddListener(GoToNextChapter);
        _prevButton.onClick.AddListener(GoToPreviousChapter);

        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }

        UpdateButtonVisibility();
        _ccButtonImage = _ccButton.GetComponent<Image>();

        // Initialize subtitle text object to be disabled
        if (_subtitleText != null)
        {
            _subtitleText.gameObject.SetActive(false);
            _ccEnabledState = false; // Ensure the boolean reflects the initial state
            Debug.Log("Subtitles initialized as Disabled (GameObject)");
        }

        UpdateCCButtonVisual(_ccEnabledState); // Set initial button visual to "Off"
        Debug.Log($"Initial CC State: {( _ccEnabledState ? "Enabled" : "Disabled" )}");

        if (_chapterNavigation == null)
        {
            Debug.LogError("TimelineChapterNavigation script not assigned in UIController!");
        }
    }
    #endregion
    #region PlayPauseRestart
    public void PlayTimeline()
    {
        Debug.Log("PlayTimeline called");
        if (_timeline != null && !_isPlaying)
        {
            if (_timeline.state != PlayState.Playing)
            {
                _timeline.Play();
                Debug.Log("Timeline Playing!");
            }
            else if (_timeline.state == PlayState.Paused)
            {
                _timeline.Resume();
                Debug.Log("Timeline resumed!");
            }
            _isPlaying = true;
            UpdateButtonVisibility();
        }
        else if (_timeline == null)
        {
            Debug.Log("No Timeline Found!!!");
        }
    }

    public void PauseTimeline()
    {
        Debug.Log("PauseTimeline called");
        if (_timeline != null && _isPlaying)
        {
            _timeline.Pause();
            Debug.Log("Timeline Paused!");
            _isPlaying = false;
            UpdateButtonVisibility();
        }
    }

    public void RestartScene()
    {
        Debug.Log("RestartScene called");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Scene Restarted!");
    }
    #endregion
    #region ClosedCaptionButton
    public void ToggleClosedCaptions()
    {
        if (!_isToggling)
        {
            StartCoroutine(DelayedToggle());
        }
    }
    private IEnumerator DelayedToggle()
    {
        _isToggling = true;

        Debug.Log($"Before Toggle (GameObject Coroutine): Closed Captions {(_ccEnabledState ? "Enabled" : "Disabled")}");
        _ccEnabledState = !_ccEnabledState;
        SetSubtitleVisibility(_ccEnabledState);
        Debug.Log($"Subtitle Object Visibility set to (Coroutine): {(_ccEnabledState ? "True" : "False")}");

        yield return new WaitForSeconds(0.05f);
        UpdateCCButtonVisual(_ccEnabledState);
        Debug.Log($"CC Button Visual updated to (GameObject Coroutine): {(_ccEnabledState ? "On" : "Off")}");
        Debug.Log($"Closed Captions set to (GameObject Coroutine): {(_ccEnabledState ? "Enabled" : "Disabled")}");

        _isToggling = false;
    }

    private void SetSubtitleVisibility(bool enabled)
    {
        if (_subtitleText != null)
        {
            _subtitleText.gameObject.SetActive(enabled);
            Debug.Log($"Subtitle Object Visibility set to: {enabled}");
        }
        else
        {
            Debug.LogWarning("Subtitle Text GameObject is null, cannot set active state.");
        }
    }

    private void UpdateCCButtonVisual(bool enabled)
    {
        if (_ccButtonImage != null && _ccIconOn != null && _ccIconOff != null)
        {
            _ccButtonImage.sprite = enabled ? _ccIconOn : _ccIconOff;
            Debug.Log($"CC Button Visual updated to: {(enabled ? "On" : "Off")}");
        }
    }
    #endregion
    #region ChapterSkipping
    public void GoToNextChapter()
    {
        Debug.Log("GoToNextChapter called");
        if (_chapterNavigation != null && !_isChapterSkipping)
        {
            _isChapterSkipping = true;
            _chapterNavigation.NextChapter();
            StartCoroutine(ResetSkipDebounce());
        }
    }

    public void GoToPreviousChapter()
    {
        Debug.Log("GoToPreviousChapter called");
        if (_chapterNavigation != null && !_isChapterSkipping)
        {
            _isChapterSkipping = true;
            _chapterNavigation.PreviousChapter();
            StartCoroutine(ResetSkipDebounce());
        }
    }
    private IEnumerator ResetSkipDebounce()
    {
        yield return new WaitForSeconds(_skipDebounceTime);
        _isChapterSkipping = false;
    }
    #endregion
    private void UpdateButtonVisibility()
    {
        _playButton.gameObject.SetActive(!_isPlaying);
        _pauseButton.gameObject.SetActive(_isPlaying);
    }
}