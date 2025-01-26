
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
        private Vector3 m_previousPosition;
        private Quaternion m_previousRotation;
        private Color m_color;

        private string Content => m_inputField.text;
        private Color Color => m_color;
        private Vector3 Position => transform.position;
        private Quaternion Rotation => transform.rotation;

        internal void Setup(
            string stickerId,
            string content,
            Color color,
            Vector3 position,
            Quaternion rotation,
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
            transform.position = m_previousPosition = position;
            transform.rotation = m_previousRotation = rotation;
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
            SendCustomEventIfValid(m_eventListener, m_onCloseEventName);
        }

        private bool IsEdit()
        {
            if (Color != m_previousColor) return true;
            if (Content != m_previousContent) return true;
            if (Position != m_previousPosition) return true;
            if (Rotation != m_previousRotation) return true;
            return false;
        }

        #region Unity Event
        public void OnClickCancel()
        {
            if (!IsEdit())
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
            m_system.UpdateLocalSticker(m_stickerId, Content, Color, Position, Rotation);
            Close();
        }
        #endregion
    }

}