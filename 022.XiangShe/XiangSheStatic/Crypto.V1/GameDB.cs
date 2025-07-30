using System;

namespace XiangSheStatic.Crypto.V1
{
    /// <summary>
    /// Key配置
    /// </summary>
    public interface IKeyContext
    {
        /// <summary>
        /// 资源AESkey
        /// </summary>
        public string AssetKey { get; }
        /// <summary>
        /// 资源AES盐值
        /// </summary>
        public byte[] AssetSalt { get; }
        /// <summary>
        /// 资源XORkey
        /// </summary>
        public string AssetXORKey { get; }
        /// <summary>
        /// 文件AESKey
        /// </summary>
        public string FileKey { get; }
    }

    /// <summary>
    /// 文件管理配置
    /// </summary>
    public interface IFileManagerContext
    {
        /// <summary>
        /// 文件相对路径
        /// </summary>
        public string FMRelativePath { get; }
        /// <summary>
        /// 文件资源文件夹名
        /// </summary>
        public string FMAssetDirectoryName { get; }
        /// <summary>
        /// 存档文件夹名
        /// </summary>
        public string FMSaveDataDirectoryName { get; }
        /// <summary>
        /// 全局变量配置文件名
        /// </summary>
        public string FMGlobalKeyDataFileName { get; }
    }

    /// <summary>
    /// Unity包配置
    /// </summary>
    public interface IAssetContext
    {
        /// <summary>
        /// 包相对路径
        /// </summary>
        public string ASRelativePath { get; }
        /// <summary>
        /// 文件列表名称
        /// </summary>
        public string ASFileListFileName { get; }
    }

    /// <summary>
    /// 游戏数据
    /// </summary>
    public abstract class GameDataBase : IKeyContext, IFileManagerContext, IAssetContext
    {
        public virtual string AssetKey => string.Empty;
        public virtual byte[] AssetSalt => Array.Empty<byte>();
        public virtual string AssetXORKey => string.Empty;
        public virtual string FileKey => string.Empty;

        public virtual string FMRelativePath => "FileManager";
        public virtual string FMAssetDirectoryName => "AB";
        public virtual string FMSaveDataDirectoryName => "ICS";
        public virtual string FMGlobalKeyDataFileName => "GlobalKeyData.json";

        public virtual string ASRelativePath => "StreamingAssets";
        public virtual string ASFileListFileName => "manifest.json";
    }

    /// <summary>
    /// 大科学家
    /// </summary>
    public class AGreatScientist : GameDataBase
    {
        public override string AssetKey => "z6f0np7o1bF31lJy";
        public override byte[] AssetSalt { get; } = new byte[]
        {
            0x63, 0x61, 0x74, 0x64, 0xAA, 0x01, 0x43, 0x3B, 0x98, 0x33, 0x1A, 0x5B, 0xC3, 0x44, 0xD0, 0x8A
        };
        public override string AssetXORKey => "zzwwssaadd9d2g0rWZ.#d5pJge2$abv7";
        public override string FileKey => "filecatmin.9";
    }
}
