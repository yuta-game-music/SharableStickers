
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
            m_system.AddNewLocalSticker("test", Color.green);
        }
        #endregion
    }

}