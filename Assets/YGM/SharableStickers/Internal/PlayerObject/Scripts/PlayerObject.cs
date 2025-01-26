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
        [SerializeField] private EditableSticker m_localStickerPrefab;
        [SerializeField] private Sticker m_othersStickerPrefab;
        [SerializeField] private Transform m_stickerParent;
        [SerializeField] private PlayerDataSerializer m_serializer;
        [SerializeField] private PlayerDataDeserializer m_deserializer;
        [SerializeField] System m_system;

        [UdonSynced, FieldChangeCallback(nameof(StickerStatus))]
        private string m_stickerStatus;
        public string StickerStatus
        {
            get => m_stickerStatus;
            set
            {
                m_stickerStatus = value;
                SendCustomEventDelayedFrames(nameof(UpdateStickers), 1);
            }
        }

        private Sticker StickerPrefab => IsLocalObject ? m_localStickerPrefab : m_othersStickerPrefab;

        internal void SetStickerStatus(string stickerStatus)
        {
            m_stickerStatus = stickerStatus;
            UpdateStickers();
        }

        internal Sticker SetSticker(string stickerId, string content, Color color, Vector3 position, Quaternion rotation)
        {
            var sticker = FindOrGenerateSticker(stickerId);
            sticker.SetData(content, color, position, rotation);
            UpdateStickerStatusText();
            return sticker;
        }

        internal string GetStickerStatusText()
        {
            var stickers = new Sticker[m_stickerParent.childCount];
            for (var i = 0; i < stickers.Length; i++)
            {
                var tf = m_stickerParent.GetChild(i);
                if (tf == null) continue;
                stickers[i] = tf.GetComponent<Sticker>();
            }
            return m_serializer.Serialize(Networking.GetOwner(gameObject), stickers);
        }

        /// <summary>
        /// 現状のステッカー状態をもとに状態テキストを作成します。
        /// </summary>
        private void UpdateStickerStatusText()
        {
            m_stickerStatus = GetStickerStatusText();
            Log("Updated Sticker Status Text (Send   ): " + m_stickerStatus);
        }

        /// <summary>
        /// 現在の状態テキストをもとにステッカーを更新します。
        /// </summary>
        private void UpdateStickers()
        {
            Log("Updated Sticker Status Text (Receive): " + m_stickerStatus);
            m_deserializer.Deserialize(m_stickerStatus, m_stickerParent, StickerPrefab, m_system);
        }

        internal Sticker FindOrGenerateSticker(string stickerId)
        {
            var sticker = FindSticker(stickerId);
            if (sticker != null)
            {
                return sticker;
            }
            // 生成
            var stickerGameObject = Instantiate(StickerPrefab.gameObject, m_stickerParent, false);
            stickerGameObject.name = GetStickerGameObjectByStickerId(stickerId);
            sticker = stickerGameObject.GetComponent<Sticker>();
            sticker.SetupAsLocal(ObjectOwner, stickerId, "", Color.white, new Vector3(0, 1, 0), Quaternion.identity);
            if (IsLocalObject)
            {
                var editableSticker = stickerGameObject.GetComponent<EditableSticker>();
                if (editableSticker != null)
                {
                    editableSticker.SetupAsEditable(m_system);
                }
            }
            return sticker;
        }

        internal Sticker FindSticker(string stickerId)
        {
            var name = GetStickerGameObjectByStickerId(stickerId);
            var tf = m_stickerParent.Find(name);
            if (tf != null)
            {
                return tf.GetComponent<Sticker>();
            }
            return null;
        }

        private string GetStickerGameObjectByStickerId(string stickerId)
        {
            return "Sticker_" + stickerId;
        }
    }

}