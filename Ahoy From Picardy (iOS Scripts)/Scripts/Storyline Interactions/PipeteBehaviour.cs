using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PipeteBehaviour : MonoBehaviour
{
    private bool _pipeteTapped = false;
    private bool _colliderEnabled = false;
    [SerializeField] private PlayableDirector _pipeteDirector;
    [SerializeField] private float _loopAtTime;
    [SerializeField] private BoxCollider _pipeteCollider;
    [SerializeField] private AudioSource _confirmationSource; // Reference to the AudioSource component
    [SerializeField] private AudioClip _confirmationClip;    // Reference to the AudioClip to play
    private bool _hasPlayed = false;

    private void Start()
    {
        _pipeteCollider.enabled = false;
    }
    public void CheckLoop()
    {
        if (!_pipeteTapped)
        {
            _pipeteDirector.time = _loopAtTime;
        }
        else
        {
            return;
        }
    }
    public void Drop()
    {
        if (!_pipeteTapped)
        {
            _pipeteTapped = true;
            _pipeteCollider.enabled = false;
            _colliderEnabled = false;
            if (!_hasPlayed)
            {
                PlayConfirmationSound();
                _hasPlayed = true;
            }
        }
    }

    public void EnablePipeteCollider()
    {
        if (!_colliderEnabled)
        {
            _pipeteCollider.enabled = true;
            _colliderEnabled = true;
            _pipeteTapped = false;
            _hasPlayed = false;
        }
    }

    private void PlayConfirmationSound()
    {
        if (_confirmationSource != null && _confirmationClip != null)
        {
            _confirmationSource.clip = _confirmationClip; // Set the AudioClip to the AudioSource
            _confirmationSource.Play();  // Play the sound
        }
        else
        {
            Debug.LogWarning("Confirmation Sound or Clip is missing!");
        }
    }
}