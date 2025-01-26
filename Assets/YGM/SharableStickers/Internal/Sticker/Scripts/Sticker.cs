
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Sticker : UdonSharpBehaviourWithUtils
    {
        [SerializeField] protected StickerView m_stickerView;
        private VRCPlayerApi m_owner;
        private bool m_hasOwner; // ダウンロードして生成したStickerはfalseになっている

        private string m_stickerId;
        private string m_content;
        private Color m_color;

        public string StickerId => m_stickerId;
        public string Content => m_content;
        public Color Color => m_color;
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        internal void SetupAsLocal(VRCPlayerApi owner, string stickerId, string content, Color color, Vector3 position, Quaternion rotation)
        {
            m_hasOwner = true;
            m_owner = owner;
            m_stickerId = stickerId;
            m_content = content;
            m_color = color;
            transform.position = position;
            transform.rotation = rotation;
            UpdateStickerView();
        }

        internal void SetData(string content, Color color, Vector3 position, Quaternion rotation)
        {
            m_content = content;
            m_color = color;
            transform.position = position;
            transform.rotation = rotation;
            UpdateStickerView();
        }

        private void UpdateStickerView()
        {
            m_stickerView.SetData(m_content, m_color);
        }
    }

}