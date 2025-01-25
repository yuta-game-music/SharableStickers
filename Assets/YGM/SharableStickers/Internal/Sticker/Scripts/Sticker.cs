
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Sticker : UdonSharpBehaviourWithUtils
    {
        private VRCPlayerApi m_owner;
        private bool m_hasOwner; // ダウンロードして生成したStickerはfalseになっている

        private string m_stickerId;
        private string m_content;
        private Color m_color;

        public string StickerId => m_stickerId;
        public string Content => m_content;
        public Color Color => m_color;

        internal void SetupAsLocal(VRCPlayerApi owner, string stickerId, string content, Color color)
        {
            m_hasOwner = true;
            m_owner = owner;
            m_stickerId = stickerId;
            m_content = content;
            m_color = color;
        }

        internal void SetData(string content, Color color)
        {
            m_content = content;
            m_color = color;
        }
    }

}