
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    public class NoEditConfirmDialog : YesNoDialog
    {
        internal void ShowDialog(UdonSharpBehaviour eventListener, string onYesEventName, string onNoEventName)
        {
            Show(
                "編集を終了しますか？\n※変更内容は保存されません！",
                "終了",
                "続ける",
                eventListener,
                onYesEventName,
                onNoEventName
            );
        }
    }

}