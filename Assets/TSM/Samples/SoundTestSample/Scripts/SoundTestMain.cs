using UnityEngine;
using TSM;
using TSMSampleUtil;
using System.Collections.Generic;

namespace TSMSample
{
    public class SoundTestMain : MonoBehaviour
    {
        [SerializeField]
        public Transform mainCameraTransform;

        [SerializeField]
        private SoundTestUI mainUI;

        private UIVolumeConfig uiVolumeConfig;

        private AudioMixerManager audoMixerManager;

        [SerializeField]
        private Rotater cubeRotater;

        [SerializeField]
        private SwitchFuncButton pauseButton;

        private void Start()
        {
            SoundManager.Instance.SetAudioListener(mainCameraTransform);

            audoMixerManager = SoundManager.Instance.GetAudioMixerManager();

            SceneLoader.Instance.LoadAdditiveWithCallback("UI_VolumeConfig", Initialize);
        }

        public void Initialize()
        {
            uiVolumeConfig = FindObjectOfType<UIVolumeConfig>();

            uiVolumeConfig.SetMasterSliderEvent(vol => audoMixerManager.MasterVolumeByLinear = vol);
            uiVolumeConfig.SetBgmSliderEvent(vol => audoMixerManager.BgmVolumeByLinear = vol);
            uiVolumeConfig.SetGameSeSliderEvent(vol => audoMixerManager.GameSeVolumeByLinear = vol);

            pauseButton.SetEvent("Pause", GamePause, "Resume", GameResume);
        }

        public void SetJinglePlayStatusText()
        {
            uiVolumeConfig.Show(
                audoMixerManager.MasterVolumeByLinear,
                audoMixerManager.BgmVolumeByLinear,
                audoMixerManager.GameSeVolumeByLinear);
        }

        public void PlayMenuSe()
        {
            SoundManager.Instance.PlayMenuSe("Laser");
        }

        public void PlayJingle()
        {
            mainUI.SetJinglePlayStatusText("JinglePlaying");
            SoundManager.Instance.PlayJingle("Notice", () => mainUI.SetJinglePlayStatusText("JingleStop"));
        }

        public void GamePause()
        {
            cubeRotater.enabled = false;
            SoundManager.Instance.Pause();
        }

        public void GameResume()
        {
            cubeRotater.enabled = true;
            SoundManager.Instance.Resume();
        }
    }
}