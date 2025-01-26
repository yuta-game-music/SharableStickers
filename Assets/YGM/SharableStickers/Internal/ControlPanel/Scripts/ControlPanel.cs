
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
            var playerTrackingData = LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            var playerFrontPosition = playerTrackingData.position + playerTrackingData.rotation * new Vector3(0, -0.5f, 1f);
            var playerFaceRotation = playerTrackingData.rotation;
            m_system.AddNewLocalSticker("", randomColor, playerFrontPosition, playerFaceRotation);
        }

        public void OnClickSaveLocalStickers()
        {
            m_system.SaveLocalStickers();
        }
        #endregion
    }

}