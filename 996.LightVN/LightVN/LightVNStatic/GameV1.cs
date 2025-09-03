using System;

namespace LightVNStatic
{
    /// <summary>
    /// 默认参数
    /// </summary>
    public abstract class DefaultGameV1 : CryptoFilterV1
    {
        public override byte[] Key { get; } = new byte[]
        {
            0x64, 0x36, 0x63, 0x35, 0x66, 0x4B, 0x49, 0x33, 0x47, 0x67, 0x42, 0x57, 0x70, 0x5A, 0x46, 0x33,
            0x54, 0x7A, 0x36, 0x69, 0x61, 0x33, 0x6B, 0x46, 0x30
        };
    }

    /// <summary>
    /// U-ena 空焰火少女
    /// </summary>
    public class UenaFarFireworks : DefaultGameV1
    {
        public override string ToString()
        {
            return "U-ena 空焰火少女";
        }
    }

    /// <summary>
    /// 姫の楽園
    /// </summary>
    public class PrincessParadise : DefaultGameV1
    {
        public override string ToString()
        {
            return "姫の楽園";
        }
    }
}
