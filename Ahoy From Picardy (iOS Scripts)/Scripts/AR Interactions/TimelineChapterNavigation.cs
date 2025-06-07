using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public struct ChapterData
{
    public string chapterName;
    public float startTime;
}

public class TimelineChapterNavigation : MonoBehaviour
{
    [SerializeField] private PlayableDirector _timeline;
    [SerializeField] private List<ChapterData> _chapters;
    [SerializeField] private float _pauseDurationBeforeSkip = 0.3f;

    private int _currentChapterIndex = 0;

    public void NextChapter()
    {
        if (_chapters == null || _chapters.Count == 0 || _currentChapterIndex >= _chapters.Count - 1) return;

        _currentChapterIndex++;
        StartCoroutine(SkipToChapter(_chapters[_currentChapterIndex].startTime));
    }

    public void PreviousChapter()
    {
        if (_chapters == null || _chapters.Count == 0 || _currentChapterIndex <= 0) return;

        _currentChapterIndex--;
        StartCoroutine(SkipToChapter(_chapters[_currentChapterIndex].startTime));
    }

    private IEnumerator SkipToChapter(float time)
    {
        if (_timeline != null)
        {
            // Pause the timeline briefly
            PlayState previousState = _timeline.state;
            _timeline.Pause();
            yield return new WaitForSeconds(_pauseDurationBeforeSkip);

            // Set the new time
            _timeline.time = time;
            _timeline.Evaluate(); // Ensure the timeline updates

            // Resume the previous state if it was playing
            if (previousState == PlayState.Playing)
            {
                _timeline.Play();
            }

            Debug.Log($"Skipped to time {time}");
        }
        else
        {
            Debug.LogWarning("Timeline not set.");
        }
    }
/*
    public int GetCurrentChapterIndex()
    {
        return _currentChapterIndex;
    }

    public string GetCurrentChapterName()
    {
        if (_chapters != null && _chapters.Count > _currentChapterIndex && _currentChapterIndex >= 0)
        {
            return _chapters[_currentChapterIndex].chapterName;
        }
        return "";
    }*/
}