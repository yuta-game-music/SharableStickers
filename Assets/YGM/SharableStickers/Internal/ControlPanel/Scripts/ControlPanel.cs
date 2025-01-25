
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ControlPanel : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private System m_system;
        #region Unity Event
        public void OnClickAddSticker()
        {
            var randomColor = Random.ColorHSV(0, 1, 0.7f, 0.7f, 0.8f, 0.8f, 0.7f, 0.7f);
            m_system.AddNewLocalSticker("", randomColor);
        }

        public void OnClickSaveLocalStickers()
        {
            m_system.SaveLocalStickers();
        }
        #endregion
    }

}