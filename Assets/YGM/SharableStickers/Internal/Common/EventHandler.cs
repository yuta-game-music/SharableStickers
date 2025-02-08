
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class EventHandler : UdonSharpBehaviour
    {
        [SerializeField] private string m_name;
        private const int BulkInsertCount = 10;
        private UdonSharpBehaviour[] m_eventHandlers = new UdonSharpBehaviour[BulkInsertCount];
        private string[] m_eventNames = new string[BulkInsertCount];
        private int m_eventCount = 0;

        public void RegisterEventHandler(UdonSharpBehaviour eventHandler, string eventName)
        {
            if (m_eventHandlers.Length >= m_eventCount)
            {
                // 枠がないので先に追加
                var prevHandlers = m_eventHandlers;
                var prevNames = m_eventNames;
                m_eventHandlers = new UdonSharpBehaviour[m_eventCount + BulkInsertCount];
                m_eventNames = new string[m_eventCount + BulkInsertCount];
                for (var i = 0; i < prevHandlers.Length; i++)
                {
                    m_eventHandlers[i] = prevHandlers[i];
                    m_eventNames[i] = prevNames[i];
                }
            }
            m_eventHandlers[m_eventCount] = eventHandler;
            m_eventNames[m_eventCount] = eventName;
            m_eventCount++;
        }

        public void Invoke()
        {
            for (var i = 0; i < m_eventCount; i++)
            {
                if (m_eventHandlers[i] != null && !string.IsNullOrEmpty(m_eventNames[i]))
                {
                    m_eventHandlers[i].SendCustomEvent(m_eventNames[i]);
                }
            }
        }

        public void ClearEvents()
        {
            m_eventCount = 0;
        }
    }

}