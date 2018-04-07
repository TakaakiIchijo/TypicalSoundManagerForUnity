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
        private List<AudioClip> menuSeAudioClipList, bgmAudioClipList, jingleClipList;

        public static SoundManager Instance;

        private List<IEnumerator> fadeCoroutineList = new List<IEnumerator>();
        private IEnumerator jinglePlayCompCallbackCoroutine;

        public bool IsPaused { get; private set; }

        private List<IAudioPausable> pausableList = new List<IAudioPausable>();

        public void SetPausableList(IAudioPausable audioPausable)
        {
            pausableList.Add(audioPausable);
        }

        public void RemovePausableList(IAudioPausable audioPausable)
        {
            pausableList.Remove(audioPausable);
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

        public void PlayJingle(string clipName, UnityAction compCallback = null)
        {
            if (IsPaused) return;

            compCallback += () => { jinglePlayCompCallbackCoroutine = null; };

            AudioClip audioClip = jingleClipList.FirstOrDefault(clip => clip.name == clipName);

            //clipがなかったら処理を中止//
            if (audioClip == null)
            {
                Debug.Log("Can't find audioClip " + clipName);
                return;
            }

            jinglePlayCompCallbackCoroutine = audioSourceJingle.PlayWithCompCallback(audioClip: audioClip, compCallback: compCallback);

            StartCoroutine(jinglePlayCompCallbackCoroutine);
        }

        public void PlayMenuSe(string clipName)
        {
            if (IsPaused) return;

            AudioClip audioClip = menuSeAudioClipList.FirstOrDefault(clip => clip.name == clipName);

            if (audioClip == null)
            {
                Debug.Log("Can't find audioClip " + clipName);
                return;
            }

            audioSourceMenuSe.Play(audioClip);
        }

        public void PlayBGM(string clipName)
        {
            PlayBGMWithFade(clipName, 0.1f);
        }

        //uGUIから呼ぶ用//
        public void PlayBGMWithFade(string clipName)
        {
            PlayBGMWithFade(clipName, 2f);
        }

        public void PlayBGMWithFade(string clipName, float fadeTime)
        {
            if (IsPaused) return;

            //リストからAudioClipを取得//
            AudioClip audioClip = bgmAudioClipList.FirstOrDefault(clip => clip.name == clipName);

            //clipがなかったら処理を中止//
            if (audioClip == null)
            {
                Debug.Log("Can't find audioClip " + clipName);
                return;
            }

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
                    AddFadeCoroutineListAndStart(audioSourcePlaying.StopWithFadeOut(fadeTime));
                }

                AddFadeCoroutineListAndStart(audioSourceEmpty.PlayWithFadeIn(audioClip, fadeTime: fadeTime));
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

            //再生しているbgm audio sourceがあったら止める//
            foreach (AudioSource asb in audioSourceBGMList.Where(asb => asb.isPlaying == true))
            {
                AddFadeCoroutineListAndStart(asb.StopWithFadeOut(fadeTime));
            }
        }

        private void AddFadeCoroutineListAndStart(IEnumerator routine)
        {
            fadeCoroutineList.Add(routine);
            StartCoroutine(routine);
        }

        private void StopFadeCoroutine()
        {
            fadeCoroutineList.ForEach(routine => StopCoroutine(routine));
            fadeCoroutineList.Clear();
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

            fadeCoroutineList.ForEach(routine => StopCoroutine(routine));
            audioSourceBGMList.ForEach(asb => asb.Pause());

            PauseExeptBGM();
        }

        public void PauseExeptBGM()
        {
            IsPaused = true;

            audioSourceMenuSe.Pause();
            audioSourceJingle.Pause();

            pausableList.ForEach(p => p.Pause());

            if (jinglePlayCompCallbackCoroutine != null)
            {
                StopCoroutine(jinglePlayCompCallbackCoroutine);
            }
        }

        public void Resume()
        {
            IsPaused = false;

            fadeCoroutineList.ForEach(routine => StartCoroutine(routine));
            audioSourceBGMList.ForEach(asb => asb.UnPause());

            ResumeExeptBGM();
        }

        public void ResumeExeptBGM()
        {
            IsPaused = false;

            audioSourceMenuSe.UnPause();
            audioSourceJingle.UnPause();

            pausableList.ForEach(p => p.Resume());

            if (jinglePlayCompCallbackCoroutine != null)
            {
                StartCoroutine(jinglePlayCompCallbackCoroutine);
            }
        }
    }
}