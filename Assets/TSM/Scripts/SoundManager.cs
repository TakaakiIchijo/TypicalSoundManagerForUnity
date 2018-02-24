using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

namespace TSM
{
    [RequireComponent(typeof(AudioMixerManager))]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        private AudioMixerManager audioMixermanager;

        [SerializeField]
        private Transform audioListenerTransform;

        [SerializeField]
        private AudioSource audioSourceMenuSe;

        [SerializeField]
        private AudioSource audioSourceJingle;

        [SerializeField]
        private List<AudioSource> audioSourceBGMList = new List<AudioSource>(2);

        [SerializeField]
        private AudioClip[] menuSeAudioClipArray, bgmAudioSourceArray, jingleClipArray;

        public static SoundManager Instance;

        private IEnumerator[] fadeCoroutineArray = new IEnumerator[2];

        public bool IsPaused { get; private set; }

        private List<IAudioPausable> pausableList = new List<IAudioPausable>();

        private float currentJingleClipLength;
        private UnityAction jingleCompCallback;

        public void SetPausableList(IAudioPausable audioPausable)
        {
            pausableList.Add(audioPausable);
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

            audioSourceBGMList.ForEach(asb => asb.loop = true);
        }

        public AudioMixerManager GetAudioMixerManager()
        {
            return audioMixermanager;
        }

        public void PlayJingle(string clipName, UnityAction compCallback)
        {
            if (IsPaused) return;
            audioSourceJingle.PlayFromArray(clipName, jingleClipArray);

            currentJingleClipLength = audioSourceJingle.clip.length;
            jingleCompCallback = compCallback;
        }

        private void Update()
        {
            if (IsPaused) return;

            if (jingleCompCallback != null && currentJingleClipLength > 0f)
            {
                currentJingleClipLength -= Time.deltaTime;

                if (currentJingleClipLength <= 0f)
                {
                    jingleCompCallback();
                    jingleCompCallback = null;
                }
            }
        }

        public void PlayMenuSe(string clipName)
        {
            if (IsPaused) return;
            audioSourceMenuSe.PlayFromArray(clipName, menuSeAudioClipArray);
        }

        public void PlayMenuSeWithCallback(string clipName)
        {
            if (IsPaused) return;
            audioSourceMenuSe.PlayWithCompCallback(clipName, menuSeAudioClipArray, 1f, () => { Debug.Log("end!"); });
        }

        public void PlayBGM(string clipName)
        {
            PlayBGMWithFade(clipName, 0.1f);
        }

        public void PlayBGMWithFade(string clipName)
        {
            PlayBGMWithFade(clipName, 2f);
        }

        public void PlayBGMWithFade(string clipName, float fadeTime)
        {
            if (IsPaused) return;

            AudioSource audioSourceEmpty = audioSourceBGMList.FirstOrDefault(asb => asb.isPlaying == false);

            if (audioSourceEmpty == null)
            {
                Debug.LogWarning("フェード処理中は新たなBGMを再生開始できません");
                return;
            }
            else
            {
                StopFadeCoroutine();

                //どちらか片方が再生中ならフェードアウト処理//
                AudioSource audioSourcePlaying = audioSourceBGMList.FirstOrDefault(asb => asb.isPlaying == true);
                if (audioSourcePlaying != null)
                {
                    fadeCoroutineArray[0] = audioSourcePlaying.StopWithFadeOut(fadeTime);
                    StartCoroutine(fadeCoroutineArray[0]);
                }

                fadeCoroutineArray[1] = audioSourceEmpty.PlayWithFadeIn(clipName, bgmAudioSourceArray, fadeTime: fadeTime);
                StartCoroutine(fadeCoroutineArray[1]);
            }
        }

        public void StopBGM()
        {
            StopBGMWithFade(0.1f);
        }

        public void StopBGMWithFade(float fadeTime)
        {
            if (IsPaused) return;

            StopFadeCoroutine();

            //２本のフェードコルーチンを両方使ってフェードアウト処理をする//
            fadeCoroutineArray[0] = audioSourceBGMList[0].StopWithFadeOut(fadeTime);
            StartCoroutine(fadeCoroutineArray[0]);

            fadeCoroutineArray[1] = audioSourceBGMList[1].StopWithFadeOut(fadeTime);
            StartCoroutine(fadeCoroutineArray[1]);
        }

        private void ResumeFadeCoroutine()
        {
            if (fadeCoroutineArray[0] != null)
            {
                StartCoroutine(fadeCoroutineArray[0]);
            }

            if (fadeCoroutineArray[1] != null)
            {
                StartCoroutine(fadeCoroutineArray[1]);
            }
        }

        private void StopFadeCoroutine()
        {
            StopAllCoroutines();
            fadeCoroutineArray[0] = null;
            fadeCoroutineArray[1] = null;
        }

        public void SetAudioListener(Transform followTransform)
        {
            audioListenerTransform.SetParent(followTransform);
            audioListenerTransform.SetPositionAndRotation(followTransform.position, followTransform.rotation);
        }

        public void ClearAudioListenerPos()
        {
            audioListenerTransform.SetParent(this.transform);
            audioListenerTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public void Pause()
        {
            IsPaused = true;
            StopAllCoroutines();
            audioSourceBGMList.ForEach(asb => asb.Pause());

            audioSourceMenuSe.Pause();
            audioSourceJingle.Pause();

            pausableList.ForEach(p => p.Pause());
        }

        public void Resume()
        {
            IsPaused = false;
            ResumeFadeCoroutine();
            audioSourceBGMList.ForEach(asb => asb.UnPause());

            audioSourceMenuSe.UnPause();
            audioSourceJingle.UnPause();

            pausableList.ForEach(p => p.Resume());
        }
    }
}