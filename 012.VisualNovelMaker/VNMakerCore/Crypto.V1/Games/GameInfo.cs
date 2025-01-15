using System;
using System.Collections.Generic;
using System.Text;
using VNMakerCore.General;

namespace VNMakerCore.Crypto.V1.Games
{
    /// <summary>
    /// 爱与命的彼端
    /// </summary>
    public class AiYvMingDeBiDuan : XorFilter
    {
        public override byte[] Key { get; } = new byte[] { 0x0A, 0x2B, 0x36, 0x6F, 0x0B };

        public override string ToString()
        {
            return "爱与命的彼端";
        }
    }

    /// <summary>
    /// 执谕者:坠月之兆
    /// </summary>
    public class ArchenemyLunafall : XorFilter
    {
        public override byte[] Key { get; } = new byte[] { 0x0A, 0x2B, 0x36, 0x6F, 0x0B };

        public override string ToString()
        {
            return "执谕者:坠月之兆";
        }
    }
}
