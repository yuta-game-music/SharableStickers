
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerObject : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private EditableSticker m_localStickerPrefab;
        [SerializeField] private Sticker m_othersStickerPrefab;
        [SerializeField] private Transform m_stickerParent;
        [SerializeField] private PlayerDataSerializer m_serializer;
        [SerializeField] private PlayerDataDeserializer m_deserializer;
        [SerializeField] System m_system;

        private Sticker StickerPrefab => IsLocalObject ? m_localStickerPrefab : m_othersStickerPrefab;

        /// <summary>
        /// 付箋の状態を受け取ったときに、状態を更新します。
        /// </summary>
        /// <param name="stickerStatus"></param>
        internal void SetStickerStatus(string stickerStatus)
        {
            UpdateStickers(stickerStatus);
        }

        /// <summary>
        /// 付箋状態を更新します。ローカルプレイヤー用です。
        /// </summary>
        /// <param name="stickerId"></param>
        /// <param name="content"></param>
        /// <param name="color"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        internal Sticker SetSticker(string stickerId, string content, Color color, Vector3 position, Quaternion rotation)
        {
            if (!IsLocalObject)
            {
                Log("Tried to write sticker status on other's object!");
                return null;
            }
            var sticker = FindOrGenerateSticker(stickerId);
            sticker.SetData(content, color, position, rotation);
            UpdateStickerStatusText();
            return sticker;
        }

        /// <summary>
        /// 付箋状態を表す文字列を作成します。
        /// </summary>
        /// <returns></returns>
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
        /// 現状のステッカー状態をもとに状態テキストを作成し、保存します。
        /// </summary>
        private void UpdateStickerStatusText()
        {
            if (!IsLocalObject)
            {
                Log("Tried to write sticker status on other's persistence!");
                return;
            }
            var stickerStatus = GetStickerStatusText();
            Log("Updated Sticker Status Text (Save   ): " + stickerStatus);
            PlayerData.SetString(System.PersistenceSaveKey, stickerStatus);
        }

        /// <summary>
        /// 現在の状態テキストをもとにステッカーを更新します。
        /// </summary>
        private void UpdateStickers(string stickerStatus)
        {
            Log("Updated Sticker Status Text (Receive): " + stickerStatus);
            m_deserializer.Deserialize(stickerStatus, m_stickerParent, StickerPrefab, m_system);
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

        #region VRChat Event
        public override void OnPlayerDataUpdated(VRCPlayerApi player, PlayerData.Info[] infos)
        {
            foreach (var info in infos)
            {
                if (info.Key == System.PersistenceSaveKey)
                {
                    if (info.State != PlayerData.State.Unchanged && IsObjectOwnerMatch(player))
                    {
                        var status = PlayerData.GetString(player, System.PersistenceSaveKey);
                        SetStickerStatus(status);
                    }
                }
            }
        }
        #endregion


    }

}