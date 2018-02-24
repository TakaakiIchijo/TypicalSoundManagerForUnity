using UnityEngine;
using UnityEngine.Audio;

namespace TSM
{
    public class AudioMixerManager : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer audioMixer;

        [SerializeField]
        private AudioMixerGroup master, bgm, environment, gameSe, menuSe, voice, jingle;

        public float MasterVolumeByLinear
        {
            get
            {
                return master.GetVolumeByLinear();
            }

            set
            {
                master.SetVolumeByLinear(value);
            }
        }

        public float BgmVolumeByLinear
        {
            get
            {
                return bgm.GetVolumeByLinear();
            }

            set
            {
                bgm.SetVolumeByLinear(value);
            }
        }

        public float GameSeVolumeByLinear
        {
            get
            {
                return gameSe.GetVolumeByLinear();
            }

            set
            {
                gameSe.SetVolumeByLinear(value);
            }
        }
    }
}