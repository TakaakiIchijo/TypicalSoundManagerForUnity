using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TSMSampleUtil
{
    public static class UGUIListenerExtention
    {
        public static void Show(this CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        public static void Hide(this CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        //UnityEventがジェネリックで拡張できないので//
        public static void SetValueChangedEvent(this Slider slider, UnityAction<float> sliderCallback)
        {
            if (slider.onValueChanged != null)
            {
                slider.onValueChanged.RemoveAllListeners();
            }

            slider.onValueChanged.AddListener(sliderCallback);
        }

        public static void SetListener(this UnityEvent unityEvent, UnityAction unityAction)
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(unityAction);
        }
    }
}