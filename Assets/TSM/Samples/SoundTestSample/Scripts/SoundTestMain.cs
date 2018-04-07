using UnityEngine;
using TSM;
using System.Collections.Generic;

namespace TSMSample
{
    public class SoundTestMain : MonoBehaviour
    {
        [SerializeField]
        public Transform mainCameraTransform;

        [SerializeField]
        private SoundTestUI mainUI;

        [SerializeField]
        private VolumeConfigUI volumeConfigUI;

        private AudioMixerManager audoMixerManager;

        [SerializeField]
        private Rotater cubeRotater;

        [SerializeField]
        private SwitchFuncButton pauseButton;

        private void Start()
        {
            SoundManager.Instance.SetAudioListener(mainCameraTransform);

            audoMixerManager = SoundManager.Instance.GetAudioMixerManager();

            volumeConfigUI.SetMasterSliderEvent(vol => audoMixerManager.MasterVolumeByLinear = vol);
            volumeConfigUI.SetBgmSliderEvent(vol => audoMixerManager.BgmVolumeByLinear = vol);
            volumeConfigUI.SetGameSeSliderEvent(vol => audoMixerManager.GameSeVolumeByLinear = vol);

            pauseButton.SetEvent("Pause", GamePause, "Resume", GameResume);
        }

        public void ShowVolumeConfigUI()
        {
            volumeConfigUI.Show(
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