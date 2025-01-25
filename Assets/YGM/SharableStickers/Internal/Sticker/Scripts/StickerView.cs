
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class StickerView : UdonSharpBehaviour
    {
        [SerializeField] private RectTransform m_canvasTransform;
        [SerializeField] private Image m_background;
        [SerializeField] private Text m_content;
        [SerializeField] private RectTransform m_contentTransform;
        [SerializeField] private Vector2 m_contentMargin;
        [SerializeField] private Vector2 m_canvasMinSize;

        private bool m_waitSizeChangeFlag = false;
        internal void SetData(string text, Color color)
        {
            m_content.text = text;
            m_background.color = color;
            m_waitSizeChangeFlag = true;
        }

        public void Update()
        {
            if (m_waitSizeChangeFlag)
            {
                m_waitSizeChangeFlag = false;

                var preferredSize = m_contentTransform.rect.size + m_contentMargin;
                preferredSize.x = Mathf.Max(preferredSize.x, m_canvasMinSize.x);
                preferredSize.y = Mathf.Max(preferredSize.y, m_canvasMinSize.y);
                m_canvasTransform.sizeDelta = preferredSize;
            }
        }
    }

}