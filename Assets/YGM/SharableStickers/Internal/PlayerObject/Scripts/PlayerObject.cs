﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class PlayerObject : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private Sticker m_stickerPrefab;
        [SerializeField] private Transform m_stickerParent;
        [SerializeField] private PlayerDataSerializer m_serializer;

        [UdonSynced, FieldChangeCallback(nameof(StickerStatus))]
        private string m_stickerStatus;
        public string StickerStatus
        {
            get => m_stickerStatus;
            set
            {
                m_stickerStatus = value;
                UpdateStickers();
            }
        }

        internal Sticker AddSticker(string stickerId, string content, Color color)
        {
            var sticker = FindOrGenerateSticker(stickerId);
            sticker.SetData(content, color);
            UpdateStickerStatusText();
            return sticker;
        }

        /// <summary>
        /// 現状のステッカー状態をもとに状態テキストを作成します。
        /// </summary>
        private void UpdateStickerStatusText()
        {
            var stickers = new Sticker[m_stickerParent.childCount];
            for (var i = 0; i < stickers.Length; i++)
            {
                var tf = m_stickerParent.GetChild(i);
                if (tf == null) continue;
                stickers[i] = tf.GetComponent<Sticker>();
            }
            m_stickerStatus = m_serializer.Serialize(Networking.GetOwner(gameObject), stickers);
            Log("Updated Sticker Status Text (Send   ): " + m_stickerStatus);
        }

        /// <summary>
        /// 現在の状態テキストをもとにステッカーを更新します。
        /// </summary>
        private void UpdateStickers()
        {
            Log("Updated Sticker Status Text (Receive): " + m_stickerStatus);
        }

        private Sticker FindOrGenerateSticker(string stickerId)
        {
            var name = GetStickerGameObjectByStickerId(stickerId);
            var tf = m_stickerParent.Find(name);
            if (tf != null)
            {
                return tf.GetComponent<Sticker>();
            }
            // 生成
            var stickerGameObject = Instantiate(m_stickerPrefab.gameObject, m_stickerParent, false);
            stickerGameObject.name = name;
            var sticker = stickerGameObject.GetComponent<Sticker>();
            sticker.SetupAsLocal(ObjectOwner, stickerId, "", Color.white);
            return sticker;
        }

        private string GetStickerGameObjectByStickerId(string stickerId)
        {
            return "Sticker_" + stickerId;
        }
    }

}