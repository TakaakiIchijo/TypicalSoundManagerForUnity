using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSM;
using System.Linq;

/// <summary>
/// 3Dオーディオ用にGameObjectコンポーネントに貼り付けたSEに再生処理を行うテストコード
/// </summary>

namespace TSMSample
{
    public class EnvironmentPlayer : InGameAudioPlayerBase
    {
        [SerializeField]
        private float volume = 0.3f;

        [SerializeField]
        private bool isRandomStart = true;

        public override void Start()
        {
            base.Start();

            if (audioSource.clip == null)
            {
                AudioClip audioClip = audioClipList.FirstOrDefault();

                if (audioClip != null)
                {
                    StartCoroutine(audioSource.PlayWithFadeIn(audioClip, volume, 0.5f, isRandomStart));
                }
            }
            else
            {
                StartCoroutine(audioSource.PlayWithFadeIn(audioSource.clip, volume, 0.5f, isRandomStart));
            }
        }

        public override void Reset()
        {
            base.Reset();
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;//2Dオーディオ設定//
        }
    }
}