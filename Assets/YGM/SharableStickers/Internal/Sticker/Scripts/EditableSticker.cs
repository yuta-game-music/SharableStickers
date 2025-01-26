
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
        /// 別途、<see cref="Sticker.SetupAsLocal(VRCPlayerApi, string, string, Color, Vector3, Quaternion)"/>を呼んで下さい。 
        /// </summary>
        /// <param name="system"></param>
        public void SetupAsEditable(System system)
        {
            m_system = system;
        }

        private void SetEditingMode(bool isEditing)
        {
            m_stickerView.SetVisibleByEditing(!isEditing);
        }

        public void OnPlacedByNewStickerButton()
        {
            m_system.ShowStickerEditorForLocal(StickerId, StickerEditorViewMode.Move, this, nameof(OnFinishEdit));
            SetEditingMode(true);
        }

        #region StickerEditor Callback
        public void OnFinishEdit()
        {
            SetEditingMode(false);
        }
        #endregion

        #region Unity Event
        public void OnClickEditButton()
        {
            m_system.ShowStickerEditorForLocal(StickerId, StickerEditorViewMode.Top, this, nameof(OnFinishEdit));
            SetEditingMode(true);
        }
        #endregion
    }

}