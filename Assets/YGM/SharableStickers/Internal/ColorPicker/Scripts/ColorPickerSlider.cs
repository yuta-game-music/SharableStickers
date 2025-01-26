
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ColorPickerSlider : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private Slider m_slider;
        [SerializeField] private Text m_currentValueViewer;

        private UdonSharpBehaviour m_eventListener;
        private string m_onValueChangeEventName;

        public float Value => m_slider.value;
        public void Setup(float value, UdonSharpBehaviour eventListener, string onValueChangeEventName)
        {
            m_slider.value = value;
            m_eventListener = eventListener;
            m_onValueChangeEventName = onValueChangeEventName;
            UpdateView();
        }

        private void UpdateView()
        {
            m_currentValueViewer.text = $"{Value:F2}";
        }

        #region Unity Event
        public void OnSliderValueChanged()
        {
            UpdateView();
            if (m_eventListener != null && !string.IsNullOrEmpty(m_onValueChangeEventName))
            {
                m_eventListener.SendCustomEvent(m_onValueChangeEventName);
            }
        }
        #endregion
    }

}