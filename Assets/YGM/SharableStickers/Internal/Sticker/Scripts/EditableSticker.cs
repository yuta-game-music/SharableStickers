
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    public class EditableSticker : Sticker
    {
        private System m_system;

        /// <summary>
        /// 編集可能なステッカーオブジェクトを設定します。
        /// 別途、<see cref="Sticker.SetupAsLocal(VRCPlayerApi, string, string, Color)"/>を呼んで下さい。 
        /// </summary>
        /// <param name="system"></param>
        public void SetupAsEditable(System system)
        {
            m_system = system;
        }
        #region Unity Event
        public void OnClickEditButton()
        {
            m_system.ShowStickerEditorForLocal(StickerId);
        }
        #endregion
    }

}