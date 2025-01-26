using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using VRC.SDK3.Persistence;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class System : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private ControlPanel m_controlPanel;
        [SerializeField] private PlayerObject m_playerObjectTemplate;
        [SerializeField] private StickerIdGenerator m_stickerIdGenerator;
        [SerializeField] private StickerEditor m_stickerEditor;

        public const string PersistenceSaveKey = "SharableStickers_LocalStickers";

        public PlayerObject GetPlayerObject(VRCPlayerApi player)
        {
            var component = Networking.FindComponentInPlayerObjects(player, m_playerObjectTemplate);
            if (component == null) return null;
            return (PlayerObject)component;
        }

        public void AddNewLocalSticker(string content, Color color)
        {
            var playerObject = GetPlayerObject(LocalPlayer);
            if (playerObject == null)
            {
                Log("Cannot find PlayerObject!");
                return;
            }
            playerObject.SetSticker(m_stickerIdGenerator.Generate(), content, color);
        }

        public StickerEditor ShowStickerEditorForLocal(string stickerId)
        {
            var playerObject = GetPlayerObject(LocalPlayer);
            if (playerObject == null)
            {
                Log("Cannot find PlayerObject!");
                return null;
            }
            var sticker = playerObject.FindOrGenerateSticker(stickerId);

            var stickerEditorGameObject = Instantiate(m_stickerEditor.gameObject, transform, false);
            var stickerEditor = stickerEditorGameObject.GetComponent<StickerEditor>();
            stickerEditor.Setup(
                stickerId,
                sticker.Content,
                sticker.Color,
                this
            );
            return stickerEditor;
        }

        public void UpdateLocalSticker(string stickerId, string content, Color color)
        {
            var playerObject = GetPlayerObject(LocalPlayer);
            if (playerObject == null)
            {
                Log("Cannot find PlayerObject!");
                return;
            }
            playerObject.SetSticker(stickerId, content, color);
        }

        public void SaveLocalStickers()
        {
            var playerObject = GetPlayerObject(LocalPlayer);
            if (playerObject == null)
            {
                Log("Cannot find PlayerObject!");
                return;
            }
            var latestStickerStatusText = playerObject.GetStickerStatusText();
            PlayerData.SetString(PersistenceSaveKey, latestStickerStatusText);
        }

        #region VRChat Events
        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (player.isLocal)
            {
                var playerObject = GetPlayerObject(LocalPlayer);
                if (playerObject == null)
                {
                    Log("Cannot find PlayerObject!");
                    return;
                }
                var status = PlayerData.GetString(LocalPlayer, PersistenceSaveKey);
                Log("Restored status " + status);
                playerObject.SetStickerStatus(status);
            }
        }
        #endregion
    }
}