using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SingleTapAnimation : MonoBehaviour
{
    private bool _isDone = true;
    [SerializeField]
    private PlayableDirector _singleTimeDirector;

    public void SingleTap()
    {
        if (_isDone)
        {
            _singleTimeDirector.Play();
            _isDone = false;
        }
        else if (!_isDone)
        {
            return;
        }
    }
    public void ResetAnimationClickablity()
    {
        _isDone = true;
    }
}