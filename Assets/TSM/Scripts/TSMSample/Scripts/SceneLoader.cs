using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSMSample
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;

        private Coroutine currentLoadCoroutine;

        private void Awake()
        {
            Instance = this;
        }

        public void LoadAdditive(string sceneName, Action callback = null)
        {
            currentLoadCoroutine = StartCoroutine(LoadUIParts(sceneName, callback));
        }

        private IEnumerator LoadUIParts(string sceneName, Action callback)
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