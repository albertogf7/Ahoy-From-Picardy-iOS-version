using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Collections;

public class TitleScreenController : MonoBehaviour
{
    [Header("Timeline Control")]
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private float _loopAtTime = 0f;

    [Header("UI")]
    [SerializeField] private Button _playButton;
    [SerializeField] private AudioSource _buttonClickSound;
    [SerializeField] private Canvas _titleCanvas;

    [Header("Scene Management")]
    [SerializeField] private PrologueEndHandler _prologueEndHandler;

    private bool _hasPlayButtonBeenPressed = false;
    private bool _isLooping = true;

    private void Start()
    {
        if (_playButton != null)
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
        }
        else
        {
            Debug.LogError("Play Button not assigned in TitleScreenController!");
        }

        if (_director == null)
        {
            Debug.LogError("Playable Director not assigned in TitleScreenController!");
        }
        else
        {
            _director.Play(); 
        }

        if (_prologueEndHandler == null)
        {
            Debug.LogError("PrologueEndHandler not assigned in TitleScreenController!");
        }

        if (_titleCanvas == null)
        {
            Debug.LogError("Title Canvas not assigned in TitleScreenController!");
        }
    }

    public void OnPlayButtonClicked()
    {
        if (_hasPlayButtonBeenPressed) return;

        _hasPlayButtonBeenPressed = true;

        if (_playButton != null)
        {
            _playButton.interactable = false;
        }

        if (_buttonClickSound != null)
        {
            _buttonClickSound.Play();
        }

        _isLooping = false;
        Debug.Log("Starting Prologue");
        if (_titleCanvas != null)
        {
            _titleCanvas.gameObject.SetActive(false);
        }
    }

    public void CheckLoopSignal()
    {
        if (_isLooping)
        {
            _director.time = _loopAtTime;
            Debug.Log("Looping Timeline to: " + _loopAtTime);
        }
    }
}