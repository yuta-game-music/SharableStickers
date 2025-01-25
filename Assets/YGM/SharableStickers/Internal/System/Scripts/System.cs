using UnityEngine;
using UdonSharp;
using VRC.SDKBase;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class System : UdonSharpBehaviour
    {
        [SerializeField] private ControlPanel m_controlPanel;
        [SerializeField] private PlayerObject m_playerObjectTemplate;
        void Start()
        {

        }

        public PlayerObject GetPlayerObject(VRCPlayerApi player)
        {
            var component = Networking.FindComponentInPlayerObjects(player, m_playerObjectTemplate);
            if (component == null) return null;
            return (PlayerObject)component;
        }
    }
}