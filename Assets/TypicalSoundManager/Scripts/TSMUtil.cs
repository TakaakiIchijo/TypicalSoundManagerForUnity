using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace TSM
{
    public static class TSMUtil
    {
        public static List<AudioSource> InstantiateAudioSourceList(int num, bool isLoop, GameObject parent, AudioMixerGroup mixerGroup)
        {
            List<AudioSource> _audioSourceList = new List<AudioSource>();

            for (int i = 0; i < num; i++)
            {
                _audioSourceList.Add(InstantiateAudioSource(isLoop, parent, mixerGroup));
            }

            return _audioSourceList;
        }

        public static AudioSource InstantiateAudioSource(bool isLoop, GameObject parent, AudioMixerGroup mixerGroup)
        {
            AudioSource _audioSource = parent.AddComponent<AudioSource>() as AudioSource;
            _audioSource.playOnAwake = false;
            _audioSource.loop = isLoop;
            _audioSource.outputAudioMixerGroup = mixerGroup;
            return _audioSource;
        }

        public static AudioSource GetStoppedAudioSoureFromList(List<AudioSource> audioSourceList)
        {
            AudioSource _audioSource = audioSourceList.FirstOrDefault(ase => ase.isPlaying == false);
            if (_audioSource == null)
            {
                Debug.LogWarning("audioSourceSEの再生上限です。");
            }
            return _audioSource;
        }

        public static AudioClip GetAudioClipFromLoadedList(string clipName, List<AudioClip> clipList)
        {
            AudioClip _clip = clipList.Find(i => i.name == clipName);

            if (_clip == null)
            {
                Debug.LogWarning("オーディオクリップ「" + clipName + "」は読み込まれていません。");
            }

            return _clip;
        }

        public static List<AudioClip> LoadAudioClipsFromResourcesFolder(string path)
        {
            return Resources.LoadAll(path, typeof(AudioClip)).Cast<AudioClip>().ToList();
        }
    }


}