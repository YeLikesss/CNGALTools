using System;

namespace LightVNStatic
{
    /// <summary>
    /// U-ena 空焰火少女
    /// </summary>
    public class UenaFarFireworks : CryptoFilterV1
    {
        public sealed override byte[] Key { get; } = new byte[]
        {
            0x64, 0x36, 0x63, 0x35, 0x66, 0x4B, 0x49, 0x33, 0x47, 0x67, 0x42, 0x57, 0x70, 0x5A, 0x46, 0x33,
            0x54, 0x7A, 0x36, 0x69, 0x61, 0x33, 0x6B, 0x46, 0x30
        };

        public override string ToString()
        {
            return "U-ena 空焰火少女";
        }
    }
}
