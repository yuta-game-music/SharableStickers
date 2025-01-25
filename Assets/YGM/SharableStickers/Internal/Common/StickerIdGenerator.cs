
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    public class StickerIdGenerator : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private PlayerHashGenerator m_playerHashGenerator;
        private int m_previousIdCount = 0;
        private string m_previousId = "";
        public string Generate()
        {
            var id = $"{m_playerHashGenerator.GenerateHash(LocalPlayer)}_{DateTime.UtcNow.Ticks}";
            if (id == m_previousId)
            {
                m_previousIdCount++;
                return id + "_" + m_previousIdCount;
            }
            else
            {
                m_previousId = id;
                m_previousIdCount = 0;
                return id;
            }
        }
    }

}