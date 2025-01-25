using System.Text;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerDataSerializer : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private PlayerHashGenerator m_playerHashGenerator;
        public const int DataVersion = 1;
        internal string Serialize(VRCPlayerApi owner, Sticker[] stickers)
        {
            var userHash = m_playerHashGenerator.GenerateHash(owner);
            var dic = new DataDictionary();
            dic.Add("version", new DataToken(DataVersion));
            dic.Add("user", new DataToken(userHash));

            var stickerList = new DataList();
            foreach (var sticker in stickers)
            {
                if (sticker == null) continue;
                stickerList.Add(GetStickerToken(sticker));
            }
            dic.Add("stickers", new DataToken(stickerList));
            VRCJson.TrySerializeToJson(dic, JsonExportType.Minify, out var data);
            return data.String;
        }
        private DataToken GetStickerToken(Sticker sticker)
        {
            var dic = new DataDictionary();
            dic.Add("id", new DataToken(sticker.StickerId));
            dic.Add("content", new DataToken(sticker.Content));
            dic.Add("color", GetColorToken(sticker.Color));
            return dic;
        }
        private DataToken GetColorToken(Color color)
        {
            var dic = new DataDictionary();
            dic.Add("r", new DataToken(color.r));
            dic.Add("g", new DataToken(color.g));
            dic.Add("b", new DataToken(color.b));
            dic.Add("a", new DataToken(color.a));
            return dic;
        }
    }

}