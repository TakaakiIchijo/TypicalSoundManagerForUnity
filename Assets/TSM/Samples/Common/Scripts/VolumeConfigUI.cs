using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TSMSample
{
    public class VolumeConfigUI : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private Slider masterSlider, bgmSlider, seSlider;

        [SerializeField]
        private AudioSource seTestPlayAudioSource;

        private bool testSoundEnable = false;

        private void Start()
        {
            Hide();
        }

        public void Show(float masterVolume, float bgmVolume, float seVolume)
        {
            masterSlider.value = masterVolume;
            bgmSlider.value = bgmVolume;
            seSlider.value = seVolume;

            testSoundEnable = true;
            canvasGroup.Show();
        }

        public void Hide()
        {
            canvasGroup.Hide();
        }

        public void SetMasterSliderEvent(UnityAction<float> sliderCallback)
        {
            masterSlider.SetValueChangedEvent(sliderCallback);
        }

        public void SetBgmSliderEvent(UnityAction<float> sliderCallback)
        {
            bgmSlider.SetValueChangedEvent(sliderCallback);
        }

        public void SetGameSeSliderEvent(UnityAction<float> sliderCallback)
        {
            sliderCallback += (vol) => { PlayTestSe(); };
            seSlider.SetValueChangedEvent(sliderCallback);
        }

        private void PlayTestSe()
        {
            if (testSoundEnable == false || seTestPlayAudioSource.isPlaying) return;

            seTestPlayAudioSource.Play();
        }
    }
}