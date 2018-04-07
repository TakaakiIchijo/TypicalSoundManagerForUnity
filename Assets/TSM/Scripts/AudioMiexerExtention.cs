using UnityEngine;
using UnityEngine.Audio;

namespace TSM
{
    public static class AudioMixerGroupExtention
    {
        //パラメーター名は"Group名+Volume"の命名規則で設定する//

        public static float GetVolumeByLinear(this AudioMixerGroup audioMixerGroup)
        {
            float decibel;

            audioMixerGroup.audioMixer.GetFloat(audioMixerGroup.name + "Volume", out decibel);

            return Mathf.Pow(10f, decibel / 20f);
        }

        public static void SetVolumeByLinear(this AudioMixerGroup audioMixerGroup, float linearVolume)
        {
            float decibel = 20.0f * Mathf.Log10(linearVolume);

            if (float.IsNegativeInfinity(decibel))
            {
                decibel = -96f;
            }

            audioMixerGroup.audioMixer.SetFloat(audioMixerGroup.name + "Volume", decibel);
        }
    }
}