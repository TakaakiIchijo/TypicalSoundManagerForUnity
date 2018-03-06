using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TSMSampleUtil
{
    [RequireComponent(typeof(Button))]
    public class SwitchFuncButton : MonoBehaviour
    {
        private bool boolState = false;
        private UnityAction[] actionArray = new UnityAction[2];
        private string[] buttonNameArray = new string[2];

        [SerializeField]
        private Button thisButton;

        [SerializeField]
        private Text buttonName;

        public void SetEvent(string firstName, UnityAction firstAction, string secondName, UnityAction secondAction)
        {
            actionArray[0] = firstAction;
            actionArray[1] = secondAction;

            buttonNameArray[0] = firstName;
            buttonNameArray[1] = secondName;

            buttonName.text = buttonNameArray[boolState ? 1 : 0];
            thisButton.onClick.SetListener(OnClick);
        }

        private void OnClick()
        {
            actionArray[boolState ? 1 : 0]();

            boolState = !boolState;
            buttonName.text = buttonNameArray[boolState ? 1 : 0];
        }
    }
}