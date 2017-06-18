using System;
using System.Collections;
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
        public string se3DAudioClipPath = "AudioClips/SE3D";
        public string bgmAudioClipPath = "AudioClips/BGM";
        public string jingledAudioClipPath = "AudioClips/JINGLE";
        public string voiseAudioClipPath = "AudioClips/VOICE";
        public int seMaxNum = 5;//SEの同時再生上限数//

        private AudioSource jingleAudioSource = new AudioSource();
        private List<AudioSource> seAudioSourceList = new List<AudioSource>();
        private List<AudioSourceExtentions> bgmAudioSourceExList = new List<AudioSourceExtentions>();

        public List<AudioSourceExtentions> BgmAudioSourceExList
        { get { return bgmAudioSourceExList; } }

        public AudioSource JingleAudioSource
        { get { return jingleAudioSource; } }

        public AudioMixer mixer;
        public AudioMixerGroup mixerSE;
        public AudioMixerGroup mixerBGM;
        public AudioMixerGroup mixerJINGLE;
        public AudioMixerGroup mixerVOICE;

        //読み込むサウンドデータのリスト//
        private List<AudioClip> seAudioClips = new List<AudioClip>();
        private List<AudioClip> se3DAudioClips = new List<AudioClip>();
        private List<AudioClip> bgmAudioClips = new List<AudioClip>();
        private List<AudioClip> jingleAudioClips = new List<AudioClip>();

        public bool IsPaused { get; private set; }

        private const float DEFAULT_FADETIME = 2f;
        private Action jingleCallback;

        public string LastPlayedSEName { get; private set; }

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
            se3DAudioClips = TSMUtil.LoadAudioClipsFromResourcesFolder(se3DAudioClipPath);
            bgmAudioClips = TSMUtil.LoadAudioClipsFromResourcesFolder(bgmAudioClipPath);
            jingleAudioClips = TSMUtil.LoadAudioClipsFromResourcesFolder(jingledAudioClipPath);

            //オーディオソースの生成/

            for (int i = 0; i < 2; i++)
            {
                AudioSource _audioSource = TSMUtil.InstantiateAudioSource(true, gameObject, mixerBGM);
                AudioSourceExtentions ase = gameObject.AddComponent<AudioSourceExtentions>() as AudioSourceExtentions;
                ase.audioSource = _audioSource;
                bgmAudioSourceExList.Add(ase);
            }

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

            if (_audioSource == null) { Debug.LogWarning("No Audio Source for "+ clipName); return; }

            //clipNameのaudioClipはロードされているか？//
            _audioSource.clip = TSMUtil.GetAudioClipFromLoadedList(clipName, seAudioClips);

            _audioSource.volume = volume;
            _audioSource.Play();
            LastPlayedSEName = clipName;
        }

        public void PlaySE3D(AudioSource audioSource, string clipName, float volume)
        {
            audioSource.clip = TSMUtil.GetAudioClipFromLoadedList(clipName, se3DAudioClips);

            audioSource.volume = volume;
            audioSource.Play();
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
            bgmAudioSourceExList.ForEach(ase => ase.Pause());
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
            bgmAudioSourceExList.ForEach(ase => ase.UnPause());
        }

        public void PlayBGM(string clipName)
        {
            PlayBGMWithFade(clipName, 0.1f);
        }

        public void PlayBGMWithFade(string clipName, float fadeTime = DEFAULT_FADETIME, Action callback = null)
        {
            AudioClip clip = TSMUtil.GetAudioClipFromLoadedList(clipName, bgmAudioClips);

            if (bgmAudioSourceExList.Find(ase => ase.audioSource.clip == clip))
            {
                return;
            }

            AudioSourceExtentions audioSourceEx = bgmAudioSourceExList.FirstOrDefault(ase => ase.audioSource.isPlaying == false);
            if (audioSourceEx != null)
            {
                if (fadeTime == 0f) fadeTime = DEFAULT_FADETIME;

                StopBGMWithFade(fadeTime);
                audioSourceEx.PlayWithFadeIn(clip, fadeTime, callback);
            }
        }

        public void StopBGM()
        {
            bgmAudioSourceExList.ForEach(ase => ase.Stop());
        }

        public void StopBGMWithFade(float fadeTime = DEFAULT_FADETIME, Action callback = null)
        {
            if (fadeTime == 0f) fadeTime = DEFAULT_FADETIME;
            bgmAudioSourceExList.ForEach(ase => ase.StopWithFadeOut(fadeTime, callback));
        }
        
        public void StopAllSE()
        {
            seAudioSourceList.ForEach(ase => ase.Stop());
        }

        public int MasterVolumeInt
        {
            get
            {
                return (int)(GetLinearVolumeFromMixerGroup("MasterVolume") * 10f);
            }
            set
            {
                SetLinearVolumeToMixerGroup("MasterVolume", value * 0.1f);
            }
        }

        public void SetLinearVolumeToMixerGroup(string mixerGroupName, float linearVolume)
        {
            float decibel = 20.0f * Mathf.Log10(linearVolume);

            if (float.IsNegativeInfinity(decibel))
            {
                decibel = -96f;
            }

            mixer.SetFloat(mixerGroupName, decibel);
        }

        public float GetLinearVolumeFromMixerGroup(string mixerGroupName)
        {
            float decibel;

            mixer.GetFloat(mixerGroupName, out decibel);

            return Mathf.Pow(10f, decibel / 20f);
        }

        public void PlayJingle(string jingleName)
        {
            if (!jingleAudioSource.isPlaying)
            {
                jingleAudioSource.clip = TSMUtil.GetAudioClipFromLoadedList(jingleName, jingleAudioClips);
                jingleAudioSource.Play();
            }
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