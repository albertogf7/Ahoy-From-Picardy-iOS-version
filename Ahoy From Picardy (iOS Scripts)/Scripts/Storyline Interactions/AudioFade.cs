using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FadeAudioOnCue : MonoBehaviour
{
    public float fadeDuration = 1.5f;
    public float targetVolume = 1f;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
        audioSource.playOnAwake = false;
    }

    public void FadeIn()
    {
        audioSource.Play();
        StartFade(targetVolume);
    }

    public void FadeOut()
    {
        StartFade(0f, stopAfterFade: true);
    }

    private void StartFade(float toVolume, bool stopAfterFade = false)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeAudio(toVolume, stopAfterFade));
    }

    private IEnumerator FadeAudio(float targetVol, bool stopAfter)
    {
        float startVol = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, targetVol, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVol;

        if (stopAfter)
            audioSource.Stop();
    }
}