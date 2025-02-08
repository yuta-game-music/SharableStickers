
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class StickerView : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private System m_system;
        [SerializeField] private RectTransform m_canvasTransform;
        [SerializeField] private Image m_background;
        [SerializeField] private Text m_content;
        [SerializeField] private RectTransform m_contentTransform;
        [SerializeField] private Vector2 m_contentMargin;
        [SerializeField] private Vector2 m_canvasMinSize;
        [SerializeField] private CanvasGroup m_mainCanvasGroup;
        [SerializeField] private GameObject[] m_gameObjectsForViewMode;
        [SerializeField] private GameObject[] m_gameObjectsForEditMode;

        public void Start()
        {
            m_system.RegisterViewModeChangeEventHandler(this, nameof(OnViewModeChanged));
            UpdateViewModeSpecificObjects();
        }

        private bool m_waitSizeChangeFlag = false;
        internal void SetData(string text, Color color)
        {
            m_content.text = text;
            m_content.color = GetTextColor(color);
            m_background.color = color;
            m_waitSizeChangeFlag = true;
        }

        private Color GetTextColor(Color backgroundColor)
        {
            var sum = backgroundColor.r + backgroundColor.g + backgroundColor.b;
            if (sum < 3f / 2)
            {
                return Color.white;
            }
            else
            {
                return Color.black;
            }
        }

        public void LateUpdate()
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

        /// <summary>
        /// 編集中かどうかに基づいて表示・非表示を切り替える。
        /// 編集し始めるときにtrueで、終わったらfalseで呼ぶ。
        /// </summary>
        /// <param name="visible"></param>
        internal void SetVisibleByEditing(bool visible)
        {
            m_mainCanvasGroup.alpha = visible ? 1 : 0;
            m_mainCanvasGroup.interactable = visible;
            m_mainCanvasGroup.blocksRaycasts = visible;
        }

        private void UpdateViewModeSpecificObjects()
        {
            var mode = m_system.CurrentViewMode;
            foreach (var viewModeObject in m_gameObjectsForViewMode)
            {
                if (viewModeObject == null) continue;
                viewModeObject.SetActive(mode == ViewMode.ReadOnly);
            }
            foreach (var editModeObject in m_gameObjectsForEditMode)
            {
                if (editModeObject == null) continue;
                editModeObject.SetActive(mode == ViewMode.Edit);
            }
        }

        #region Event Listener
        public void OnViewModeChanged()
        {
            UpdateViewModeSpecificObjects();
        }
        #endregion
    }

}