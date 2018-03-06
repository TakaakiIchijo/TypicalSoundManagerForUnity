using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSMSampleUtil
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;

        private IEnumerator currentLoadCoroutine;

        private void Awake()
        {
            Instance = this;
        }

        public void LoadAdditiveWithCallback(string sceneName, Action callback = null)
        {
            currentLoadCoroutine = LoadAdditiveWithCallbackCoroutine(sceneName, callback);
            StartCoroutine(currentLoadCoroutine);
        }

        private IEnumerator LoadAdditiveWithCallbackCoroutine(string sceneName, Action callback)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (ao.progress < 1f)
            {
                yield return new WaitForEndOfFrame();
            }

            if (callback != null) callback();
        }
    }
}