using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace TSM
{
    public static class AudioSourceExtention
    {
        public static void Play(this AudioSource audioSource, AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
        }

        public static bool PlayOneShotFromArray(this AudioSource audioSource, string clipName, AudioClip[] clipArray, float volume = 1f)
        {
            AudioClip audioClip = GetClipByNameFromArray(clipName, clipArray);
            if (audioClip == null)
            {
                Debug.LogWarning(clipName + "という名前のAudioClipは見つかりません");
                return false;
            }
            else
            {
                audioSource.PlayOneShot(audioClip, volume);
                return true;
            }
        }

        public static bool PlayFromArray(this AudioSource audioSource, string clipName, AudioClip[] clipArray, float volume = 1f)
        {
            AudioClip audioClip = GetClipByNameFromArray(clipName, clipArray);
            if (audioClip == null)
            {
                Debug.LogWarning(clipName + "という名前のAudioClipは見つかりません");
                return false;
            }
            else
            {
                audioSource.Play(audioClip, volume);
                return true;
            }
        }

        public static IEnumerator PlayWithCompCallback(this AudioSource audioSource, string clipName, AudioClip[] clipArray, float volume = 1f, UnityAction compCallback = null)
        {
            AudioClip audioClip = GetClipByNameFromArray(clipName, clipArray);
            if (audioClip == null)
            {
                Debug.LogWarning(clipName + "という名前のAudioClipは見つかりません");
                yield break;
            }
            else
            {
                yield return audioSource.PlayWithCompCallback(audioClip, volume, compCallback);
            }
        }

        public static IEnumerator PlayWithCompCallback(this AudioSource audioSource, AudioClip clip, float volume = 1f, UnityAction compCallback = null)
        {
            audioSource.Play(clip, volume);

            WaitForSeconds waitForSeconds = new WaitForSeconds(clip.length);

            yield return waitForSeconds;

            if (compCallback != null) compCallback();
        }

        public static IEnumerator PlayWithFadeIn(this AudioSource audioSource, string clipName, AudioClip[] clipArray, float volume = 1f, float fadeTime = 0.1f)
        {
            //ボリュームが0以下の場合は再生しない//
            if (volume <= 0f) yield break;

            //リストから再生//
            bool isFound = audioSource.PlayFromArray(clipName, clipArray, 0f);

            //ファイルが無かったら処理中止//
            if (isFound == false) yield break;

            //フェードタイムが0かそれより小さればフェード処理を行わない//
            if (fadeTime <= 0f)
            {
                audioSource.volume = volume;
                yield break;
            }

            while (audioSource.volume < volume)
            {
                float tempVolume = audioSource.volume + Time.deltaTime / fadeTime;

                audioSource.volume = tempVolume > volume ? volume : tempVolume;//目標値よりでかい数値になったら丸める//

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

        private static AudioClip GetClipByNameFromArray(string clipName, AudioClip[] array)
        {
            //どっかで38BのGCゴミが出てる//
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].IsSameName(clipName))
                {
                    return array[i];
                }
            }

            return null;
        }
    }

    public static class AudioClipExtention
    {
        public static bool IsSameName(this AudioClip audioClip, string clipName)
        {
            return EqualityComparer<string>.Default.Equals(audioClip.name, clipName);
        }
    }
}