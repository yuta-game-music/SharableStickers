
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
        [SerializeField] private PlayerHashGenerator m_playerHashGenerator;
        public bool Deserialize(string text, Transform stickerParent, Sticker stickerPrefab, System system)
        {
            var succeeded = VRCJson.TryDeserializeFromJson(text, out var token);
            if (!succeeded)
            {
                Log("JSON Deserialization Failed!");
                return false;
            }

            var versionFetchSucceeded = token.DataDictionary.TryGetValue("version", out var versionToken);
            if (!versionFetchSucceeded)
            {
                Log($"Cannot fetch version! ({versionToken.Error})");
                return false;
            }
            if (!versionToken.IsNumber)
            {
                Log($"Cannot fetch version! (Type mismatch: {versionToken.TokenType})");
                return false;
            }

            switch ((int)versionToken.Number)
            {
                case 1:
                    return DeserializeV1(token, stickerParent, stickerPrefab, system);
            }
            return false;
        }

        private bool DeserializeV1(DataToken data, Transform stickerParent, Sticker stickerPrefab, System system)
        {
            // プレイヤー名検証
            var playerHashFetchSucceeded = data.DataDictionary.TryGetValue("user", TokenType.String, out var playerHashToken);
            if (!playerHashFetchSucceeded)
            {
                Log("Cannot fetch playerHash!");
                return false;
            }
            var playerHash = m_playerHashGenerator.GenerateHash(ObjectOwner);
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

                string stickerContent;
                var stickerContentFetchResult = token.DataDictionary.TryGetValue("content", TokenType.String, out var stickerContentToken);
                if (stickerContentFetchResult)
                {
                    stickerContent = stickerContentToken.String;
                }
                else
                {
                    Log("Cannot fetch Sticker Content! Falling back to empty");
                    stickerContent = string.Empty;
                }

                Color stickerColor;
                var stickerColorFetchResult = token.DataDictionary.TryGetValue("color", TokenType.DataDictionary, out var stickerColorToken);
                if (stickerColorFetchResult)
                {
                    var stickerColorInternalParseResult = TryFetchColor(stickerColorToken.DataDictionary, out stickerColor);
                    if (!stickerColorInternalParseResult)
                    {
                        Log("Error while parsing Sticker Color! Falling back to white");
                        stickerColor = Color.white;
                    }
                }
                else
                {
                    Log($"Cannot fetch Sticker Color! Falling back to white");
                    stickerColor = Color.white;
                }

                Vector3 stickerPosition;
                var stickerPositionFetchResult = token.DataDictionary.TryGetValue("position", TokenType.DataDictionary, out var stickerPositionToken);
                if (stickerPositionFetchResult)
                {
                    var stickerPositionInternalParseResult = TryFetchVector3(stickerPositionToken.DataDictionary, out stickerPosition);
                    if (!stickerPositionInternalParseResult)
                    {
                        Log("Error while parsing Sticker Position! Falling back to zero");
                        stickerPosition = Vector3.zero;
                    }
                }
                else
                {
                    Log($"Cannot fetch Sticker Position! Falling back to zero");
                    stickerPosition = Vector3.zero;
                }

                Vector3 stickerRotation;
                var stickerRotationFetchResult = token.DataDictionary.TryGetValue("rotation", TokenType.DataDictionary, out var stickerRotationToken);
                if (stickerRotationFetchResult)
                {
                    var stickerRotationInternalParseResult = TryFetchVector3(stickerRotationToken.DataDictionary, out stickerRotation);
                    if (!stickerRotationInternalParseResult)
                    {
                        Log("Error while parsing Sticker Rotation! Falling back to zero");
                        stickerRotation = Vector3.zero;
                    }
                }
                else
                {
                    Log($"Cannot fetch Sticker Rotation! Falling back to zero");
                    stickerRotation = Vector3.zero;
                }
                var stickerRotationAsQuaternion = Quaternion.Euler(stickerRotation);
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

                    sticker.SetData(stickerContent, stickerColor, stickerPosition, stickerRotationAsQuaternion);
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
                    stickerComponent.SetupAsLocal(ObjectOwner, stickerId, stickerContent, stickerColor, stickerPosition, stickerRotationAsQuaternion);

                    var editableStickerComponent = newStickerGameObject.GetComponent<EditableSticker>();
                    if (editableStickerComponent != null)
                    {
                        // ローカルオブジェクトの場合、Editableとしてのセットアップも必要
                        editableStickerComponent.SetupAsEditable(system);
                    }
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

            var stickerColorRedFetchResult = stickerColor.TryGetValue("r", out var stickerColorRedToken);
            if (!stickerColorRedFetchResult)
            {
                Log("Cannot fetch Sticker Color (Red)!");
                return false;
            }
            color.r = (float)stickerColorRedToken.Number;

            var stickerColorGreenFetchResult = stickerColor.TryGetValue("g", out var stickerColorGreenToken);
            if (!stickerColorGreenFetchResult)
            {
                Log("Cannot fetch Sticker Color (Green)!");
                return false;
            }
            color.g = (float)stickerColorGreenToken.Number;

            var stickerColorBlueFetchResult = stickerColor.TryGetValue("b", out var stickerColorBlueToken);
            if (!stickerColorBlueFetchResult)
            {
                Log("Cannot fetch Sticker Color (Blue)!");
                return false;
            }
            color.b = (float)stickerColorBlueToken.Number;

            var stickerColorAlphaFetchResult = stickerColor.TryGetValue("a", out var stickerColorAlphaToken);
            if (!stickerColorAlphaFetchResult)
            {
                Log("Cannot fetch Sticker Color (Alpha)!");
                return false;
            }
            color.a = (float)stickerColorAlphaToken.Number;

            return true;
        }

        private bool TryFetchVector3(DataDictionary vectorDict, out Vector3 vector)
        {
            vector = Vector3.zero;

            var vectorDictXFetchResult = vectorDict.TryGetValue("x", out var vectorDictXToken);
            if (!vectorDictXFetchResult)
            {
                Log("Cannot fetch vector (x)!");
                return false;
            }
            vector.x = (float)vectorDictXToken.Number;

            var vectorDictYFetchResult = vectorDict.TryGetValue("y", out var vectorDictYToken);
            if (!vectorDictYFetchResult)
            {
                Log("Cannot fetch vector (y)!");
                return false;
            }
            vector.y = (float)vectorDictYToken.Number;

            var vectorDictZFetchResult = vectorDict.TryGetValue("z", out var vectorDictZToken);
            if (!vectorDictZFetchResult)
            {
                Log("Cannot fetch vector (z)!");
                return false;
            }
            vector.z = (float)vectorDictZToken.Number;

            return true;
        }
        private string GetStickerGameObjectByStickerId(string stickerId)
        {
            return "Sticker_" + stickerId;
        }

    }

}