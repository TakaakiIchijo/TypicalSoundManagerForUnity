using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TSMSample
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

        public static IEnumerator FadeIn(this Graphic graphic, float fadeTime = 2f)
        {
            while (graphic.color.a < 1f)
            {
                float temp = graphic.color.a + Time.deltaTime / fadeTime;
                graphic.SetAlpha(temp);

                yield return null;
            }
        }

        public static IEnumerator FadeOut(this Graphic graphic, float fadeTime = 2f, UnityAction callback = null)
        {
            while (graphic.color.a > 0f)
            {
                float temp = graphic.color.a - Time.deltaTime / fadeTime;
                graphic.SetAlpha(temp);

                yield return null;
            }

            if (callback != null)
            {
                callback();
            }
        }

        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
        }
    }
}