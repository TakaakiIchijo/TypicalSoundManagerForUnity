using UnityEngine;
using System.Collections;
using TSM;
using System.Collections.Generic;
using System.Linq;

namespace TSMSample
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class InGameAudioPlayerBase : MonoBehaviour, IAudioPausable
    {
        [SerializeField]
        protected AudioSource audioSource;

        [SerializeField]
        protected List<AudioClip> audioClipList;

        [SerializeField]
        public bool audoPlay = false;

        public bool IsPaused { get; private set; }

        public virtual void Start()
        {
            SoundManager.Instance.SetPausableList(this);

            if (audoPlay)
            {
                PlayDefault();
            }
        }

        public virtual void PlayDefault()
        {
            if (audioSource.clip == null)
            {
                AudioClip audioClip = audioClipList.FirstOrDefault();

                if (audioClip != null)
                {
                    audioSource.Play(audioClip);
                }
            }
            else
            {
                audioSource.Play();
            }
        }

        //再生中だったら再生をキャンセルする//
        public virtual bool PlayIfPossible()
        {
            if (audioSource.isPlaying)
            {
                return false;
            }
            else
            {
                PlayDefault();
                return true;
            }
        }

        public virtual void Pause()
        {
            IsPaused = true;
            audioSource.Pause();
        }

        public virtual void Resume()
        {
            IsPaused = false;
            audioSource.UnPause();
        }

        public virtual void OnDestroy()
        {
            SoundManager.Instance.RemovePausableList(this);
        }

        public virtual void Reset()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }
}