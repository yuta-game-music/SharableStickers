
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class YesNoDialog : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private Text m_contentText;
        [SerializeField] private Text m_yesText;
        [SerializeField] private Text m_noText;
        private UdonSharpBehaviour m_eventListener;
        private string m_onYesEventName;
        private string m_onNoEventName;
        internal void Show(
            string contentText,
            string yesText,
            string noText,
            UdonSharpBehaviour eventListener,
            string onYesEventName,
            string onNoEventName
        )
        {
            m_contentText.text = contentText;
            m_yesText.text = yesText;
            m_noText.text = noText;

            m_eventListener = eventListener;
            m_onYesEventName = onYesEventName;
            m_onNoEventName = onNoEventName;
            gameObject.SetActive(true);
        }
        #region Unity Event
        public void OnClickYes()
        {
            SendCustomEventIfValid(m_eventListener, m_onYesEventName);
            gameObject.SetActive(false);
        }
        public void OnClickNo()
        {
            SendCustomEventIfValid(m_eventListener, m_onNoEventName);
            gameObject.SetActive(false);
        }
        #endregion
    }

}