
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    public class ColorPicker : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private ColorPickerSlider m_redSlider;
        [SerializeField] private ColorPickerSlider m_greenSlider;
        [SerializeField] private ColorPickerSlider m_blueSlider;
        [SerializeField] private ColorPickerSlider m_alphaSlider;
        [SerializeField] private Image m_previewer;

        private Color m_color;
        private UdonSharpBehaviour m_eventListener;
        private string m_onEnterEventName;
        private string m_onCancelEventName;

        public Color Color => m_color;

        public void Setup(Color color, UdonSharpBehaviour eventListener, string onEnterEventName, string onCancelEventName)
        {
            m_color = color;
            m_eventListener = eventListener;
            m_onEnterEventName = onEnterEventName;
            m_onCancelEventName = onCancelEventName;

            m_redSlider.Setup(color.r, this, nameof(OnValueUpdated));
            m_greenSlider.Setup(color.g, this, nameof(OnValueUpdated));
            m_blueSlider.Setup(color.b, this, nameof(OnValueUpdated));
            m_alphaSlider.Setup(color.a, this, nameof(OnValueUpdated));
            m_previewer.color = m_color;
        }

        private void OnValueUpdated()
        {
            m_color = new Color(m_redSlider.Value, m_greenSlider.Value, m_blueSlider.Value, m_alphaSlider.Value);
            m_previewer.color = m_color;
        }

        #region Unity Event
        public void OnEnter()
        {
            SendCustomEventIfValid(m_eventListener, m_onEnterEventName);
            gameObject.SetActive(false);
        }
        public void OnCancel()
        {
            SendCustomEventIfValid(m_eventListener, m_onCancelEventName);
            gameObject.SetActive(false);
        }
        #endregion
    }

}