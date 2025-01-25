
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerDataDeserializer : UdonSharpBehaviourWithUtils
    {
        public bool Deserialize(string text, Transform stickerParent, Sticker stickerPrefab)
        {
            var succeeded = VRCJson.TryDeserializeFromJson(text, out var token);
            if (!succeeded)
            {
                Log("JSON Deserialization Failed!");
                return false;
            }

            var versionFetchSucceeded = token.DataDictionary.TryGetValue("version", TokenType.Int, out var versionToken);
            if (!versionFetchSucceeded)
            {
                Log("Connot fetch version!");
                return false;
            }

            switch (versionToken.Int)
            {
                case 1:
                    return DeserializeV1(token, stickerParent, stickerPrefab);
            }
            return false;
        }

        private bool DeserializeV1(DataToken data, Transform stickerParent, Sticker stickerPrefab)
        {
            // プレイヤー名検証
            var playerHashFetchSucceeded = data.DataDictionary.TryGetValue("user", TokenType.String, out var playerHashToken);
            if (!playerHashFetchSucceeded)
            {
                Log("Cannot fetch playerHash!");
                return false;
            }
            var playerHash = ObjectOwner.displayName;
            if (playerHashToken != playerHash)
            {
                Log("Player Hash Mismatch!");
                return false;
            }

            // 既存ステッカーのIDを収集(マッチングしたものをnullにし、あとで残ったものだけを消す)
            // arrayのindexはstickerParent内のchildIndexに対応させたままにする
            var existingStickerIDs = new string[stickerParent.childCount];
            for (var stickerObjectIndex = existingStickerIDs.Length - 1; stickerObjectIndex >= 0; stickerObjectIndex--)
            {
                var tf = stickerParent.GetChild(stickerObjectIndex);
                if (tf == null) continue;

                var stickerComponent = tf.GetComponent<Sticker>();
                if (stickerComponent == null) continue;

                var stickerId = stickerComponent.StickerId;
                existingStickerIDs[stickerObjectIndex] = stickerId;
            }

            // ステッカーを生成・更新する
            var stickersTokenFetchResult = data.DataDictionary.TryGetValue("stickers", TokenType.DataList, out var stickerTokenList);
            if (!stickersTokenFetchResult)
            {
                Log("Cannot fetch Sticker List!");
                return false;
            }
            for (var tokenIndex = 0; tokenIndex < stickerTokenList.DataList.Count; tokenIndex++)
            {
                var token = stickerTokenList.DataList[tokenIndex];
                #region Data Fetch
                var stickerIdFetchResult = token.DataDictionary.TryGetValue("id", TokenType.String, out var stickerIdToken);
                if (!stickerIdFetchResult)
                {
                    Log("Cannot fetch Sticker ID!");
                    return false;
                }
                var stickerId = stickerIdToken.String;

                var stickerContentFetchResult = token.DataDictionary.TryGetValue("content", TokenType.String, out var stickerContentToken);
                if (!stickerContentFetchResult)
                {
                    Log("Cannot fetch Sticker Content!");
                    return false;
                }
                var stickerContent = stickerContentToken.String;

                var stickerColorFetchResult = token.DataDictionary.TryGetValue("color", TokenType.String, out var stickerColorToken);
                if (!stickerColorFetchResult)
                {
                    Log("Cannot fetch Sticker Color!");
                    return false;
                }

                var stickerColorInternalParseResult = TryFetchColor(stickerColorToken.DataDictionary, out var stickerColor);
                if (!stickerColorInternalParseResult)
                {
                    Log("Error while parsing Sticker Color!");
                    return false;
                }

                #endregion
                // 既存のものを探す
                var index = -1;
                for (var i = 0; i < existingStickerIDs.Length; i++)
                {
                    if (existingStickerIDs[i] == stickerId)
                    {
                        existingStickerIDs[i] = null;
                        index = i;
                        break;
                    }
                }
                if (index >= 0)
                {
                    // 既存のものが見つかった場合
                    var sticker = stickerParent.GetChild(index).GetComponent<Sticker>();

                    // ID検証(ねんのため)
                    if (sticker.StickerId != stickerId)
                    {
                        Log("Sticker ID Mismatch for Sticker " + sticker + "!");
                        return false;
                    }

                    sticker.SetData(stickerContent, stickerColor);
                }
                else
                {
                    // 新規作成
                    var newStickerGameObject = Instantiate(stickerPrefab.gameObject, stickerParent, false);
                    newStickerGameObject.name = GetStickerGameObjectByStickerId(stickerId);
                    var stickerComponent = newStickerGameObject.GetComponent<Sticker>();
                    if (stickerComponent == null)
                    {
                        Log("Sticker generation failed!");
                        return false;
                    }
                    stickerComponent.SetupAsLocal(ObjectOwner, stickerId, stickerContent, stickerColor);
                }
            }

            // 更新されなかった既存ステッカーは削除する
            for (var stickerObjectIndex = existingStickerIDs.Length - 1; stickerObjectIndex >= 0; stickerObjectIndex--)
            {
                if (string.IsNullOrEmpty(existingStickerIDs[stickerObjectIndex]))
                {
                    // 更新されたorステッカーオブジェクトではない
                    continue;
                }
                var tf = stickerParent.GetChild(stickerObjectIndex);
                if (tf == null) continue;

                Destroy(tf.gameObject);
            }
            return true;
        }

        private bool TryFetchColor(DataDictionary stickerColor, out Color color)
        {
            color = Color.black;

            var stickerColorRedFetchResult = stickerColor.TryGetValue("r", TokenType.Float, out var stickerColorRedToken);
            if (!stickerColorRedFetchResult)
            {
                Log("Cannot fetch Sticker Color (Red)!");
                return false;
            }
            color.r = stickerColorRedToken.Float;

            var stickerColorGreenFetchResult = stickerColor.TryGetValue("g", TokenType.Float, out var stickerColorGreenToken);
            if (!stickerColorGreenFetchResult)
            {
                Log("Cannot fetch Sticker Color (Green)!");
                return false;
            }
            color.g = stickerColorGreenToken.Float;

            var stickerColorBlueFetchResult = stickerColor.TryGetValue("b", TokenType.Float, out var stickerColorBlueToken);
            if (!stickerColorBlueFetchResult)
            {
                Log("Cannot fetch Sticker Color (Blue)!");
                return false;
            }
            color.b = stickerColorBlueToken.Float;

            var stickerColorAlphaFetchResult = stickerColor.TryGetValue("a", TokenType.Float, out var stickerColorAlphaToken);
            if (!stickerColorAlphaFetchResult)
            {
                Log("Cannot fetch Sticker Color (Alpha)!");
                return false;
            }
            color.a = stickerColorAlphaToken.Float;

            return true;
        }

        private string GetStickerGameObjectByStickerId(string stickerId)
        {
            return "Sticker_" + stickerId;
        }

    }

}