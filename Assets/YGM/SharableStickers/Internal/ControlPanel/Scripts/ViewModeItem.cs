
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers.ControlPanel
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ViewModeItem : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private ViewMode m_viewMode;
        [SerializeField] private Image m_buttonBackground;
        [SerializeField] private Color m_normalColor;
        [SerializeField] private Color m_selectedColor;

        private SharableStickers.System m_system;
        public void Setup(SharableStickers.System system)
        {
            m_system = system;
        }
        public void UpdateView()
        {
            if (m_system == null)
            {
                Log("System not set!");
                return;
            }
            var isSelected = m_system.CurrentViewMode == m_viewMode;
            if (m_buttonBackground != null)
            {
                m_buttonBackground.color = isSelected ? m_selectedColor : m_normalColor;
            }
        }

        #region Unity Event
        public void OnSelect()
        {
            if (m_system == null)
            {
                Log("System not set!");
                return;
            }
            m_system.SetViewMode(m_viewMode);
        }
        #endregion
    }

}