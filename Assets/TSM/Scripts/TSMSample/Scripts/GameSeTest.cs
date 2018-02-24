using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSM;

/// <summary>
/// 3Dオーディオ用にGameObjectコンポーネントに貼り付けたSEに再生処理を行うテストコード
/// </summary>

namespace TSMSample
{
    [RequireComponent(typeof(AudioSource))]
    public class GameSeTest : MonoBehaviour, IAudioPausable
    {
        [SerializeField]
        private AudioSource audioSource;

        public AudioClip[] audioClipArray;

        private void Start()
        {
            SoundManager.Instance.SetPausableList(this);
        }

        public void PlaySe(string seName)
        {
            audioSource.PlayOneShotFromArray(seName, audioClipArray);
        }

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.spatialBlend = 1f;//3Dオーディオ全振り//
        }

        public void Pause()
        {
            IsPaused = true;
            audioSource.Pause();
        }

        public void Resume()
        {
            IsPaused = false;
            audioSource.UnPause();
        }

        public bool IsPaused { get; private set; }
    }
}