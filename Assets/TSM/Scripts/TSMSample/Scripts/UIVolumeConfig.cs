using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIVolumeConfig : MonoBehaviour
{
    [SerializeField]
    private Camera testModeCamera;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Slider masterSlider, bgmSlider, seSlider;

    [SerializeField]
    private GameObject eventSystem;

    private void Start()
    {
        //追加ロードだった場合はテスト用のカメラを削除して初期化//
        if (SceneManager.sceneCount != 1)
        {
            Destroy(testModeCamera.gameObject);
            Destroy(eventSystem);
            Hide();
        }
    }

    public void Show(float masterVolume, float bgmVolume, float seVolume)
    {
        masterSlider.value = masterVolume;
        bgmSlider.value = bgmVolume;
        seSlider.value = seVolume;
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
        seSlider.SetValueChangedEvent(sliderCallback);
    }
}