using UnityEngine;
using System.Collections;
using System;

public static class AudioSourceExtentions  {

    public static IEnumerator FadeIn(this AudioSource audioSource, float fadeTime, Action callback)
    {
        audioSource.volume = 0f;
        audioSource.Play();
        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
        if(callback != null) callback();
    }

    public static IEnumerator FadeOut(this AudioSource audioSource, float fadeTime, Action callback)
    {
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.Stop();
        audioSource.clip = null;
        if (callback != null) callback();
    }
}
