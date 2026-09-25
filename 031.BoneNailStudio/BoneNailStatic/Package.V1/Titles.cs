
namespace BoneNailStatic.Package.V1
{
    /// <summary>
    /// 加密参数
    /// </summary>
    public interface ICryptoArguments
    {
        /// <summary>
        /// 加密key
        /// </summary>
        public byte[] Key { get; }
    }

    /// <summary>
    /// 游戏信息
    /// </summary>
    public abstract class TitleBase : ICryptoArguments
    {
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; } = string.Empty;
        public abstract byte[] Key { get; }
    }

    /// <summary>
    /// 黎明门前的吹笛人
    /// </summary>
    public class ThePiperOfDawn : TitleBase
    {
        public override string Name => "黎明门前的吹笛人";
        public override byte[] Key { get; } = new byte[]
        {
            0x17, 0x24, 0x4D, 0x9D, 0x46, 0x31, 0xD1, 0x53, 0xD4, 0xFB, 0xE8, 0xC0, 0xBF, 0xAC, 0x71, 0x0E,
            0xE4, 0xE0, 0x9F, 0xFF, 0x62, 0x90, 0x71, 0x22, 0xF2, 0xB7, 0xDB, 0x29, 0x47, 0x7F, 0x47, 0x77,
        };
    }
}
