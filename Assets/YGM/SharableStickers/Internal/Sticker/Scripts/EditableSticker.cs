
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    public class EditableSticker : Sticker
    {
        [SerializeField] private System m_system;
        #region Unity Event
        public void OnClickEditButton()
        {

        }
        #endregion
    }

}