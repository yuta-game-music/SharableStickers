
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers.StickerEditorComponent
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MoveController : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private Collider m_collider;
        [SerializeField] private Collider m_internalCollider;
        [SerializeField] private VRCPickup m_pickup;
        [SerializeField] private GameObject m_moveGuide;
        private bool m_isInControlMode = false;
        private Vector3 m_position;
        private Quaternion m_rotation;
        internal Vector3 Position => m_position;
        internal Quaternion Rotation => m_rotation;
        internal void Setup(Vector3 position, Quaternion rotation)
        {
            transform.position = m_position = position;
            transform.rotation = m_rotation = rotation;
        }

        internal void StartMoveMode()
        {
            SetActive(true);
        }

        private void SetActive(bool active)
        {
            m_collider.enabled = active;
            m_internalCollider.enabled = !active;
            m_moveGuide.SetActive(active);
            m_isInControlMode = true;
        }

        public void EnterPosition()
        {
            m_position = transform.position;
            m_rotation = transform.rotation;
        }
        public void RevertPosition()
        {
            transform.position = m_position;
            transform.rotation = m_rotation;
        }

        #region Unity Event
        public override void OnPickupUseDown()
        {
            if (!m_isInControlMode) return;
            EnterPosition();
            SetActive(false);
            m_pickup.Drop();
        }
        public override void OnDrop()
        {
            if (!m_isInControlMode) return;
            RevertPosition();
            SetActive(false);
        }
        #endregion
    }

}