using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TSM
{
    public static class AudioSourceExtention
    {
        //Play関数の拡張
        //１行でaudioClip, ボリューム、再生位置のランダマイズまで指定できるようにする//
        public static void Play(this AudioSource audioSource, AudioClip audioClip, float volume = 1f, bool isRandomStartTime = false)
        {
            if (audioClip == null) return;

            audioSource.clip = audioClip;
            audioSource.volume = volume;

            if (isRandomStartTime)
            {
                audioSource.time = UnityEngine.Random.Range(0f, audioClip.length - 0.01f);
                //結果がlengthと同値になるとシークエラーを起こすため -0.01秒する//
            }

            audioSource.Play();
        }

        public static IEnumerator PlayWithCompCallback(this AudioSource audioSource, AudioClip audioClip, float volume = 1f, UnityAction compCallback = null)
        {
            audioSource.Play(audioClip, volume);

            float timer = 0f;

            //WaitForSecondsを使うとCoroutineを一時停止・再開できなくなるのでwhileで対応//
            while (timer < audioClip.length)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            //再生完了コールバックを実行//
            if (compCallback != null)
            {
                compCallback();
            }
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="targetVolume">フェードしたときの最終到達ボリューム</param>
        /// <param name="fadeTime">フェード時間</param>
        /// <param name="isRandomStartTime">再生開始位置をランダマイズ</param>
        /// <returns></returns>
        public static IEnumerator PlayWithFadeIn(this AudioSource audioSource, AudioClip audioClip, float targetVolume = 1f, float fadeTime = 0.1f, bool isRandomStartTime = false)
        {
            //目標ボリュームが0以下の場合は再生キャンセル//
            if (targetVolume <= 0f) yield break;

            //再生開始//
            audioSource.Play(audioClip, 0f, isRandomStartTime);

            //フェードタイムが0かそれより小さればフェード処理をキャンセルして//
            if (fadeTime <= 0f)
            {
                audioSource.volume = targetVolume;
                yield break;
            }

            //目標ボリュームに到達するまで毎フレームボリュームを上げる//
            while (audioSource.volume < targetVolume)
            {
                float tempVolume = audioSource.volume + (Time.deltaTime / fadeTime * targetVolume);

                //目標ボリュームより計算結果が大きいか判定（急に大ボリュームにならないようにする）//
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

    }
}