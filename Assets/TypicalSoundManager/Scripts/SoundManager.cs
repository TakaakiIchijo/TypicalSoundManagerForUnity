using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace TSM
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private Transform thisTransform;
        public GameObject audioListenerObject;

        //From resources folder//
        public string seAudioClipPath = "AudioClips/SE";

        public string bgmAudioClipPath = "AudioClips/BGM";
        public string jingledAudioClipPath = "AudioClips/JINGLE";
        public string voiseAudioClipPath = "AudioClips/VOICE";
        public int seMaxNum = 5;//SEの同時再生上限数//

        private AudioSource jingleAudioSource = new AudioSource();
        private List<AudioSource> seAudioSourceList = new List<AudioSource>();
        private List<AudioSource> bgmAudioSourceList = new List<AudioSource>();

        public List<AudioSource> BgmAudioSourceList
        { get { return bgmAudioSourceList; } }

        public AudioSource JingleAudioSource
        { get { return jingleAudioSource; } }

        public AudioMixer mixer;
        public AudioMixerGroup mixerSE;
        public AudioMixerGroup mixerBGM;
        public AudioMixerGroup mixerJINGLE;
        public AudioMixerGroup mixerVOICE;

        //読み込むサウンドデータのリスト//
        private List<AudioClip> seAudioClips = new List<AudioClip>();

        private List<AudioClip> bgmAudioClips = new List<AudioClip>();
        private List<AudioClip> jingleAudioClips = new List<AudioClip>();

        public bool IsPaused { get; private set; }

        private const float DEFAULT_FADETIME = 2f;
        private Action jingleCallback;

        public string LastPlayedSEName { get; private set; }

        public Coroutine currentFadeInCoroutine;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            thisTransform = this.transform;
            DontDestroyOnLoad(this.gameObject);

            if (audioListenerObject == null)
            {
                Debug.LogError("not found audiolisterObject");
            }

            //リソース読み込み（同期読み）//
            seAudioClips = TSMUtil.LoadAudioClipsFromResourcesFolder(seAudioClipPath);
            bgmAudioClips = TSMUtil.LoadAudioClipsFromResourcesFolder(bgmAudioClipPath);
            jingleAudioClips = TSMUtil.LoadAudioClipsFromResourcesFolder(jingledAudioClipPath);

            //オーディオソースの生成//
            bgmAudioSourceList = TSMUtil.InstantiateAudioSourceList(2, true, this.gameObject, mixerBGM);
            seAudioSourceList = TSMUtil.InstantiateAudioSourceList(seMaxNum, false, this.gameObject, mixerSE);
            jingleAudioSource = TSMUtil.InstantiateAudioSource(false, this.gameObject, mixerJINGLE);
        }

        public void SetAudioListenerFollower(Transform cameraTransform)
        {
            audioListenerObject.transform.parent = cameraTransform;
            audioListenerObject.transform.localPosition = Vector3.zero;
            audioListenerObject.transform.localRotation = Quaternion.identity;
        }

        public void ClearAudioListenerPos()
        {
            SetAudioListenerFollower(thisTransform);
        }

        public void PlaySE(string clipName)
        {
            PlaySE(clipName, 1f);
        }

        public void PlaySE(string clipName, float volume)
        {
            //空いているaudioSourceはあるか？//
            AudioSource _audioSource = TSMUtil.GetStoppedAudioSoureFromList(seAudioSourceList);

            if (_audioSource == null) { Debug.LogWarning("No Audio Source"); return; }

            //clipNameのaudioClipはロードされているか？//
            _audioSource.clip = TSMUtil.GetAudioClipFromLoadedList(clipName, seAudioClips);

            _audioSource.volume = volume;
            _audioSource.Play();
            LastPlayedSEName = clipName;
        }

        public void OnPause()
        {
            IsPaused = true;
            seAudioSourceList.ForEach(ase => ase.Pause());

            OnBGMPause();
        }

        public void OnBGMPause()
        {
            jingleAudioSource.Pause();
            bgmAudioSourceList.ForEach(audioSource => audioSource.Pause());
            //StopCoroutine(currentFadeInCoroutine);
            //フェードアウトしてからポーズ//
        }

        public void OnResume()
        {
            IsPaused = false;
            seAudioSourceList.ForEach(ase => ase.UnPause());

            OnBGMResume();
        }

        public void OnBGMResume()
        {
            jingleAudioSource.UnPause();
            bgmAudioSourceList.ForEach(audioSource => audioSource.UnPause());
            //StartCoroutine(currentFadeInCoroutine);
        }

        public void PlayBGM(string clipName)
        {
            PlayBGMWithFade(clipName, 0.1f);
        }

        public void PlayBGMWithFade(string clipName, float fadeTime = DEFAULT_FADETIME, Action callback = null)
        {
            AudioClip clip = TSMUtil.GetAudioClipFromLoadedList(clipName, bgmAudioClips);

            if (bgmAudioSourceList.Find(source => source.clip == clip))
            {
                return;
            }

            AudioSource audioSouce = bgmAudioSourceList.FirstOrDefault(source => !source.isPlaying);
            if (audioSouce != null)

            {
                if (fadeTime == 0f) fadeTime = DEFAULT_FADETIME;

                StopBGMWithFade(fadeTime);
                audioSouce.clip = clip;

                callback += () => { currentFadeInCoroutine = null; };
                currentFadeInCoroutine = StartCoroutine(audioSouce.FadeIn(fadeTime, callback));
            }
        }

        public void StopBGM()
        {
            StopBGMWithFade(0.1f);
        }

        public void StopBGMWithFade(float fadeTime = DEFAULT_FADETIME, Action callback = null)
        {
            if (fadeTime == 0f) fadeTime = DEFAULT_FADETIME;

            if (currentFadeInCoroutine != null)
            {
                StopCoroutine(currentFadeInCoroutine);
                currentFadeInCoroutine = null;
            }

            foreach (AudioSource source in bgmAudioSourceList)
            {
                if (source.isPlaying)
                {
                    StartCoroutine(source.FadeOut(fadeTime, callback));
                }
            }
        }

        public AudioClip GetSEAudioClipFromLoadedList(string clipName)
        {
            return TSMUtil.GetAudioClipFromLoadedList(clipName, seAudioClips);
        }

        public void StopAllSE()
        {
            seAudioSourceList.ForEach(ase => ase.Stop());
        }

        public int AddIntVolumeToMaster(int amount)
        {
            int vol = GetIntVolumeFromMixerGroup("Master") + amount;
            vol = Mathf.Min(10, Mathf.Max(0, vol));

            SetIntVolumeToMixerGroup("Master", vol);
            return vol;
        }

        public int IntVolumeOfMaster
        {
            get
            {
                return GetIntVolumeFromMixerGroup("Master");
            }
            set
            {
                SetIntVolumeToMixerGroup("Master", value);
            }
        }

        private void SetIntVolumeToMixerGroup(string mixerGroupName, int volume)
        {
            float fvolume = (float)volume * 0.1f;
            float dvolume = Mathf.Lerp(-80, 0, fvolume);
            mixer.SetFloat(mixerGroupName, dvolume);
        }

        private int GetIntVolumeFromMixerGroup(string mixerGroupName)
        {
            float volume;
            mixer.GetFloat(mixerGroupName, out volume);
            volume = Mathf.InverseLerp(-80, 0, volume) * 10f;
            return (int)volume;
        }

        public void PlayJingle(string jingleName)
        {
            if (jingleAudioSource.isPlaying)
            {
                jingleAudioSource.Stop();
            }

            jingleAudioSource.clip = TSMUtil.GetAudioClipFromLoadedList(jingleName, jingleAudioClips);
            jingleAudioSource.Play();
        }

        public void StopJingle()
        {
            if (jingleAudioSource.isPlaying)
                return;

            jingleAudioSource.Stop();
            jingleAudioSource.clip = null;
        }
    }
}