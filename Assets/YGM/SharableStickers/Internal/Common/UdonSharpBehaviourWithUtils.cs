
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace YGM.SharableStickers
{
    public abstract class UdonSharpBehaviourWithUtils : UdonSharpBehaviour
    {
        protected VRCPlayerApi LocalPlayer => Networking.LocalPlayer;
        protected VRCPlayerApi ObjectOwner => Networking.GetOwner(gameObject);
        protected bool IsLocalObject => LocalPlayer != null && GetPlayerId(LocalPlayer) == GetPlayerId(ObjectOwner);
        protected void SendCustomEventIfValid(UdonSharpBehaviour eventListener, string eventName)
        {
            if (eventListener == null) return;
            if (string.IsNullOrEmpty(eventName)) return;
            eventListener.SendCustomEvent(eventName);
        }
        protected void Log(string text)
        {
            var localPlayerId = GetPlayerId(Networking.LocalPlayer);
            var instanceMasterId = GetPlayerId(Networking.Master);
            var instanceOwnerId = GetPlayerId(Networking.InstanceOwner);
            var objectOwnerId = GetPlayerId(Networking.GetOwner(gameObject));
            Debug.Log($"[SharableStickers][LP={localPlayerId} IM={instanceMasterId} IO={instanceOwnerId} OO={objectOwnerId}] " + text);
        }

        private int GetPlayerId(VRCPlayerApi player)
        {
            if (player == null) return -1;
            return player.playerId;
        }
    }
}