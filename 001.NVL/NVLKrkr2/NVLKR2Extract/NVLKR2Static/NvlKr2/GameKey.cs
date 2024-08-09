using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NVLKR2Static
{
    public class DataManager
    {
        public static Dictionary<string, IKeyInformation> GameMaps { get; } = new(16)
        {
            { "茸雪 Steam", new TinySnowSteam() },
            { "余香 Steam", new LingeringFragranceSteam() },
            { "花落冬阳 Steam", new SnowDreamsSteam() },
            { "雪之本境S Steam", new ConspiracyFieldSnowTrapSteam() },
            { "橘子班短篇合集 Steam", new ClassTangerineShortStoriesSteam() },
            { "高考恋爱100天 Steam", new GaoKaoLoveSteam() },
            { "高考恋爱100天 Package", new GaoKaoLovePackage() },
            { "虹色旋律 Steam", new MelodyofIrisSteam() },
            { "祈風 Steam", new InoriKazeSteam() },
            { "雾之本境S Steam", new ConspiracyFieldFogShadowSteam() },
            { "真恋寄语枫秋 Steam", new TrueLoveSteam() },
            { "回忆忘却之匣 Steam", new MemoryOblivionBoxSteam() },
            { "回忆忘却之匣 Package", new MemoryOblivionBoxPackage() },
            { "丑小鸭的天鹅湖 Package", new ChouXiaoYaDeTianEHu() }
        };
    }



    /// <summary>
    /// 游戏key信息
    /// </summary>
    public interface IKeyInformation
    {
        /// <summary>
        /// 解密key
        /// </summary>
        public byte[] Key { get; }
    }

    /// <summary>
    /// 茸雪 Steam
    /// </summary>
    public class TinySnowSteam : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0xFB, 0xAE, 0x1A, 0xAD, 0xE4, 0x8B, 0x25, 0x46
        };
    }

    /// <summary>
    /// 余香 Steam
    /// </summary>
    public class LingeringFragranceSteam : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0x7B, 0x98, 0x63, 0x8B, 0x00, 0x70, 0x42, 0x00
        };
    }

    /// <summary>
    /// 花落冬阳 Steam
    /// </summary>
    public class SnowDreamsSteam : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0x78, 0x29, 0xA3, 0x14, 0xF3, 0xDF, 0x62, 0xCA
        };
    }

    /// <summary>
    /// 雪之本境S Steam
    /// </summary>
    public class ConspiracyFieldSnowTrapSteam : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0x2F, 0xBA, 0xA5, 0x68, 0xB3, 0xAB, 0x8B, 0x82
        };

    }

    /// <summary>
    /// 橘子班短篇合集 Steam
    /// </summary>
    public class ClassTangerineShortStoriesSteam : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0x1E, 0x60, 0x03, 0x1E, 0x70, 0xA7, 0x48, 0x00
        };
    }

    /// <summary>
    /// 高考恋爱100天 Steam
    /// </summary>
    public class GaoKaoLoveSteam : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0x1D, 0xEF, 0x5B, 0xA3, 0x00, 0xCA, 0x41, 0x00
        };
    }

    /// <summary>
    /// 高考恋爱100天 Package
    /// </summary>
    public class GaoKaoLovePackage : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0x0C, 0xF0, 0x04, 0x61, 0x00, 0x4A, 0x42, 0x00
        };
    }

    /// <summary>
    /// 虹色旋律 Steam
    /// </summary>
    public class MelodyofIrisSteam : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0x9A, 0xDE, 0x37, 0x3E, 0x00, 0x22, 0x48, 0x00
        };
    }

    /// <summary>
    /// 祈風 Steam
    /// </summary>
    public class InoriKazeSteam : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0xB4, 0xEF, 0xDA, 0xBA, 0x00, 0xD4, 0x41, 0x00
        };
    }

    /// <summary>
    /// 雾之本境S Steam
    /// </summary>
    public class ConspiracyFieldFogShadowSteam : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0x2F, 0xFD, 0x15, 0x3C, 0x34, 0x71, 0x7A, 0xB8
        };
    }

    /// <summary>
    /// 真恋寄语枫秋 Steam
    /// </summary>
    public class TrueLoveSteam : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0x96, 0xFD, 0x5C, 0x4A, 0x00, 0x34, 0x48, 0x00
        };
    }

    /// <summary>
    /// 回忆忘却之匣 Steam
    /// </summary>
    public class MemoryOblivionBoxSteam : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0xDE, 0x88, 0xF3, 0x2C, 0x00, 0xD4, 0x41, 0x00
        };
    }

    /// <summary>
    /// 回忆忘却之匣 Package
    /// </summary>
    public class MemoryOblivionBoxPackage : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0x6B, 0x3D, 0x20, 0x6D, 0x00, 0x74, 0x41, 0x00
        };
    }

    /// <summary>
    /// 丑小鸭与天鹅湖
    /// </summary>
    public class ChouXiaoYaDeTianEHu : IKeyInformation
    {
        public byte[] Key { get; } = new byte[]
        {
            0xE5, 0xE2, 0x45, 0x78, 0x00, 0x28, 0x12, 0x00
        };
    }
}
