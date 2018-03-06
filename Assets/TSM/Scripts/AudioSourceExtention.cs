using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TSM
{
    public static class AudioSourceExtention
    {
        public static void Play(this AudioSource audioSource, AudioClip audioClip, float volume = 1f)
        {
            if (audioClip == null) return;

            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
        }

        public static bool PlayOneShot(this AudioSource audioSource, string clipName, AudioClip[] clipArray, float volume = 1f)
        {
            AudioClip audioClip = GetClipByNameFromArray(clipName, clipArray);
            audioSource.PlayOneShot(audioClip, volume);

            return audioClip != null;
        }

        public static bool Play(this AudioSource audioSource, string clipName, AudioClip[] clipArray, float volume = 1f)
        {
            AudioClip audioClip = GetClipByNameFromArray(clipName, clipArray);
            audioSource.Play(audioClip, volume);

            return audioClip != null;
        }

        public static IEnumerator PlayWithCompCallback(this AudioSource audioSource, string clipName, AudioClip[] clipArray, float volume = 1f, UnityAction compCallback = null)
        {
            AudioClip audioClip = GetClipByNameFromArray(clipName, clipArray);

            if (audioClip == null)
            {
                yield break;
            }

            audioSource.Play(clipName, clipArray, volume);

            float timer = 0f;

            //WaitForSecondsを使うとCoroutineを一時停止・再開できなくなるのでwhileで対応//
            while (timer < audioClip.length)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (compCallback != null)
            {
                compCallback();
            }
        }

        public static IEnumerator PlayWithFadeIn(this AudioSource audioSource, string clipName, AudioClip[] clipArray, float targetVolume = 1f, float fadeTime = 0.1f)
        {
            //目標ボリュームが0以下の場合は再生しない//
            if (targetVolume <= 0f) yield break;

            //配列からAudioClipを取得//
            AudioClip audioClip = GetClipByNameFromArray(clipName, clipArray);

            //clipがなかったら処理を中止//
            if (audioClip == null)
            {
                yield break;
            }

            audioSource.Play(audioClip, 0f);

            //フェードタイムが0かそれより小さればフェード処理を行わない//
            if (fadeTime <= 0f)
            {
                audioSource.volume = targetVolume;
                yield break;
            }

            //目標ボリュームに到達するまで毎フレームボリュームを上げる//
            while (audioSource.volume < targetVolume)
            {
                float tempVolume = audioSource.volume + Time.deltaTime / fadeTime;

                //目標ボリュームより計算結果が大きいか判定（いきなり大ボリュームにならないようにする）//
                audioSource.volume = tempVolume > targetVolume ? targetVolume : tempVolume;

                yield return null;
            }
        }

        public static IEnumerator StopWithFadeOut(this AudioSource audioSource, float fadeTime)
        {
            if (audioSource.isPlaying == false) yield break;

            //フェードタイムが0かそれより小さればフェード処理を行わない//
            if (fadeTime <= 0f)
            {
                audioSource.volume = 0f;
                audioSource.Stop();
                yield break;
            }

            while (audioSource.volume > 0f)
            {
                audioSource.volume -= Time.deltaTime / fadeTime;
                yield return null;
            }

            audioSource.Stop();
        }

        private static AudioClip GetClipByNameFromArray(string clipName, AudioClip[] clipArray)
        {
            for (int i = 0; i < clipArray.Length; i++)
            {
                if (clipArray[i].name.Equals(clipName))
                {
                    return clipArray[i];
                }
            }

            Debug.LogWarning(clipName + "という名前のAudioClipは見つかりません");
            return null;
        }
    }
}