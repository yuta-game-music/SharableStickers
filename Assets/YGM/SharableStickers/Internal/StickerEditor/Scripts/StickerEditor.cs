
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class StickerEditor : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private InputField m_inputField;
        [SerializeField] private ColorPicker m_colorPicker;
        [SerializeField] private NoEditConfirmDialog m_noEditConfirmDialog;
        private string m_stickerId;
        private System m_system;
        private UdonSharpBehaviour m_eventListener;
        private string m_onCloseEventName;

        private string m_previousContent;
        private Color m_previousColor;
        private Color m_color;

        private string Content => m_inputField.text;
        private Color Color => m_color;

        internal void Setup(
            string stickerId,
            string content,
            Color color,
            System system,
            UdonSharpBehaviour eventListener,
            string onCloseEventName)
        {
            m_stickerId = stickerId;
            m_system = system;
            m_eventListener = eventListener;
            m_onCloseEventName = onCloseEventName;
            m_inputField.text = m_previousContent = content;
            m_color = m_previousColor = color;
            gameObject.SetActive(true);
        }

        #region Color Picker Callback
        private void OnEnterColor()
        {
            m_color = m_colorPicker.Color;
        }

        private void OnCancelColorPicker()
        {

        }
        #endregion

        #region NoEditConfirmDialog Callback
        private void OnSelectCloseNoEditConfirm()
        {
            Close();
        }
        private void OnSelectDontCloseNoEditConfirm()
        {

        }
        #endregion

        internal void Close()
        {
            gameObject.SetActive(false);
            if (m_eventListener != null && !string.IsNullOrEmpty(m_onCloseEventName))
            {
                m_eventListener.SendCustomEvent(m_onCloseEventName);
            }
        }

        #region Unity Event
        public void OnClickCancel()
        {
            if (Color == m_previousColor && Content == m_previousContent)
            {
                Close();
            }
            m_noEditConfirmDialog.ShowDialog(this, nameof(OnSelectCloseNoEditConfirm), nameof(OnSelectDontCloseNoEditConfirm));
        }

        public void OnClickColorPicker()
        {
            m_colorPicker.Setup(m_color, this, nameof(OnEnterColor), nameof(OnCancelColorPicker));
            m_colorPicker.gameObject.SetActive(true);
        }

        public void OnClickEnter()
        {
            m_system.UpdateLocalSticker(m_stickerId, Content, Color);
            Close();
        }
        #endregion
    }

}