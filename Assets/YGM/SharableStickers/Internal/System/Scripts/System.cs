﻿using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using VRC.SDK3.Persistence;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class System : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private ControlPanel.System m_controlPanel;
        [SerializeField] private PlayerObject m_playerObjectTemplate;
        [SerializeField] private StickerIdGenerator m_stickerIdGenerator;
        [SerializeField] private StickerEditorManager m_stickerEditorManager;
        [SerializeField] private EventHandler m_viewModeChangeEventHandler;

        public const string PersistenceSaveKey = "SharableStickers_LocalStickers";
        private ViewMode m_currentViewMode = ViewMode.ReadOnly;
        public ViewMode CurrentViewMode => m_currentViewMode;

        public void Start()
        {
            m_stickerEditorManager.Setup(this);
        }

        public PlayerObject GetPlayerObject(VRCPlayerApi player)
        {
            var component = Networking.FindComponentInPlayerObjects(player, m_playerObjectTemplate);
            if (component == null) return null;
            return (PlayerObject)component;
        }

        public void AddNewLocalSticker(string content, Color color, Vector3 position, Quaternion rotation, bool showEditorImmediately)
        {
            var playerObject = GetPlayerObject(LocalPlayer);
            if (playerObject == null)
            {
                Log("Cannot find PlayerObject!");
                return;
            }
            var stickerId = m_stickerIdGenerator.Generate();
            var sticker = playerObject.SetSticker(stickerId, content, color, position, rotation);
            if (showEditorImmediately)
            {
                var editableSticker = sticker.GetComponent<EditableSticker>();
                editableSticker.OnPlacedByNewStickerButton();
            }
        }

        public StickerEditor ShowStickerEditorForLocal(string stickerId, StickerEditorViewMode initialViewMode, UdonSharpBehaviour eventListener, string onCloseEventName)
        {
            var playerObject = GetPlayerObject(LocalPlayer);
            if (playerObject == null)
            {
                Log("Cannot find PlayerObject!");
                return null;
            }
            var sticker = playerObject.FindOrGenerateSticker(stickerId);

            return m_stickerEditorManager.FindOrCreateEditor(
                stickerId,
                sticker.Content,
                sticker.Color,
                sticker.Position,
                sticker.Rotation,
                initialViewMode,
                eventListener,
                onCloseEventName
            );
        }

        public void UpdateLocalSticker(string stickerId, string content, Color color, Vector3 position, Quaternion rotation)
        {
            var playerObject = GetPlayerObject(LocalPlayer);
            if (playerObject == null)
            {
                Log("Cannot find PlayerObject!");
                return;
            }
            playerObject.SetSticker(stickerId, content, color, position, rotation);
        }

        public void SetViewMode(ViewMode viewMode)
        {
            if (m_currentViewMode == viewMode)
            {
                return;
            }
            m_currentViewMode = viewMode;
            m_viewModeChangeEventHandler.Invoke();
        }
        public void RegisterViewModeChangeEventHandler(UdonSharpBehaviour eventHandler, string eventName)
        {
            m_viewModeChangeEventHandler.RegisterEventHandler(eventHandler, eventName);
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

    public enum ViewMode
    {
        ReadOnly,
        Edit,
    }
}