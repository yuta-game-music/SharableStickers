using UnityEngine;
using UdonSharp;
using VRC.SDKBase;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class System : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private ControlPanel m_controlPanel;
        [SerializeField] private PlayerObject m_playerObjectTemplate;
        public const char Delimiter = '$';
        void Start()
        {

        }

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
            playerObject.AddSticker("test", content, color);
        }
    }
}