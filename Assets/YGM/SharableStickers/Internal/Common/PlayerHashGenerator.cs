
using System.Text;
using UdonSharp;
using VRC.SDKBase;

namespace YGM.SharableStickers
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerHashGenerator : UdonSharpBehaviour
    {
        private const int RepeatCount = 3;
        private const int Salt = 598124031;
        private const int ValueLength = 31;
        public string GenerateHash(VRCPlayerApi player)
        {
            if (player == null) return string.Empty;
            var playerName = player.displayName;
            var playerNameChars = playerName.ToCharArray();
            long randomValue = Salt;
            for (var repeatIndex = 0; repeatIndex < RepeatCount; repeatIndex++)
            {
                for (var charIndex = 0; charIndex < playerNameChars.Length; charIndex++)
                {
                    var writeIndex = (Salt + playerNameChars[0] + charIndex) % ValueLength;
                    for (var charData = (int)playerNameChars[charIndex]; charData > 0; charData = charData >> 1)
                    {
                        if (charData % 2 == 0)
                        {
                            randomValue ^= 1 << writeIndex;
                        }
                        writeIndex = (writeIndex + 1) % ValueLength;
                    }
                }
            }

            var stringBuilder = new StringBuilder();
            if (randomValue <= 0)
            {
                randomValue -= int.MinValue;
            }

            return $"{randomValue:X10}".Substring(0, 10);
        }
    }

}