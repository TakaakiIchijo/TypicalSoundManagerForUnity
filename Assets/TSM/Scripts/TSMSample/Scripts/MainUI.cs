using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TSMSample
{
    public class MainUI : MonoBehaviour
    {
        [SerializeField]
        private Text jinglePlayStatusText;

        public void SetJinglePlayStatusText(string text)
        {
            jinglePlayStatusText.text = text;
        }
    }
}