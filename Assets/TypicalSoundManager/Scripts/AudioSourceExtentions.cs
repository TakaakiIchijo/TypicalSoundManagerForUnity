using UnityEngine;
using System.Collections;
using System;

namespace TSM
{
    public class AudioSourceExtentions : MonoBehaviour
    {
        public AudioSource audioSource;
        private IEnumerator currentFadeCoroutine;
        private bool isPaused = false;

        private float volumeBeforePause = 1f;

        public void Pause()
        {
            //audioSource.Pause();
            isPaused = true;
            volumeBeforePause = audioSource.volume;

            StartCoroutine("PauseWithFade");

            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }
        }

        public IEnumerator PauseWithFade()
        {
            while (audioSource.volume > 0f)
            {
                audioSource.volume -= Time.deltaTime / 0.1f;
                yield return null;
            }
            audioSource.Pause();
        }

        public void UnPause()
        {
            //audioSource.UnPause();
            isPaused = false;
            StartCoroutine("UnPauseWithFade");

            if (currentFadeCoroutine != null)
            {
                StartCoroutine(currentFadeCoroutine);
            }
        }

        public IEnumerator UnPauseWithFade()
        {
            audioSource.UnPause();
            while (audioSource.volume < volumeBeforePause)
            {
                audioSource.volume += Time.deltaTime / 0.1f;
                yield return null;
            }
        }


        public void Play(AudioClip clip)
        {
            audioSource.clip = clip;
            PlayWithFadeIn(clip, 0.1f);
        }

        public void PlayWithFadeIn(AudioClip clip, float fadeTime, Action callback = null)
        {
            if (isPaused == true) return;
            currentFadeCoroutine = FadeIn(clip, fadeTime, callback);
            StartCoroutine(currentFadeCoroutine);
        }

        public IEnumerator FadeIn(AudioClip clip, float fadeTime, Action callback)
        {
            audioSource.clip = clip;
            audioSource.volume = 0f;
            audioSource.Play();
            while (audioSource.volume < 1f)
            {
                audioSource.volume += Time.deltaTime / fadeTime;
                yield return null;
            }
            if (callback != null) callback();
            currentFadeCoroutine = null;
        }

        public void Stop()
        {
            StopWithFadeOut(0.1f);
        }

        public void StopWithFadeOut(float fadeTime, Action callback = null)
        {
            if (isPaused == true) return;
            if (audioSource.isPlaying == false) return;

            if(currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }

            currentFadeCoroutine = FadeOut(fadeTime, callback);
            StartCoroutine(currentFadeCoroutine);
        }

        public IEnumerator FadeOut(float fadeTime, Action callback)
        {
            while (audioSource.volume > 0f)
            {
                audioSource.volume -= Time.deltaTime / fadeTime;
                yield return null;
            }
            audioSource.Stop();
            audioSource.clip = null;
            if (callback != null) callback();
            currentFadeCoroutine = null;
        }


    }
}
