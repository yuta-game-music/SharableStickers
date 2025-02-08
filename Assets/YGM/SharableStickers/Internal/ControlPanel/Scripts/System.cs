
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers.ControlPanel
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class System : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private SharableStickers.System m_system;
        [SerializeField] private ViewModeController m_viewModeController;
        [SerializeField] private Transform m_newStickerPosition;
        public void Start()
        {
            m_viewModeController.Setup(m_system);
        }
        #region Unity Event
        public void OnClickAddSticker()
        {
            var randomColor = Random.ColorHSV(0, 1, 0.7f, 0.7f, 0.8f, 0.8f, 0.7f, 0.7f);
            m_system.AddNewLocalSticker("", randomColor, m_newStickerPosition.position, m_newStickerPosition.rotation, true);
        }
        #endregion
    }

}