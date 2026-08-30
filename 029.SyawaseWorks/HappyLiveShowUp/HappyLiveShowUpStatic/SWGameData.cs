using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HappyLiveShowUpStatic
{
    /// <summary>
    /// 游戏版本
    /// </summary>
    public enum GameVersion : uint
    {
        V102,
        Max,
    }

    /// <summary>
    /// 游戏参数
    /// </summary>
    public interface IGameParam
    {
        /// <summary>
        /// 0x200000 Key文件
        /// <para>位于Themida3.x XBundler内</para>
        /// </summary>
        public byte[] EncKeyFile { get; }

        /// <summary>
        /// 0x40000 Key文件
        /// <para>位于patch.pack 0x2000000处</para>
        /// </summary>
        public byte[] PatchKeyFile { get; }

        /// <summary>
        /// 0x100 全局Key
        /// <para>由Themida3.x 虚拟化代码内生成</para>
        /// </summary>
        public byte[] GlobalKey { get; }

        /// <summary>
        /// 封包目录
        /// </summary>
        public Dictionary<string, DataPackInfo> DataPack { get; }
    }

    public class SWGameData
    {
        /// <summary>
        /// 使用版本获取对应参数
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        public static IGameParam? CreateFactory(GameVersion ver)
        {
            IGameParam? res = ver switch
            {
                GameVersion.V102 => new HappyLiveShowUp_V102(),
                _ => null,
            };
            return res;
        }
    }

    /// <summary>
    /// 封包信息
    /// </summary>
    public class DataPackInfo
    {
        /// <summary>
        /// CRC32值
        /// </summary>
        public uint CRC32Hash { get; init; }
        /// <summary>
        /// 封包序号
        /// </summary>
        public uint Order { get; init; }

        /// <summary>
        /// 绑定的KeyFile
        /// </summary>
        public byte[] KeyFile { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="crc32">CRC32值</param>
        /// <param name="order">封包序号</param>
        /// <param name="keyfile">绑定keyfile</param>
        public DataPackInfo(uint crc32, uint order, byte[] keyfile)
        {
            this.CRC32Hash = crc32;
            this.Order = order;
            this.KeyFile = keyfile;
        }
    }

    /// <summary>
    /// V102版本
    /// </summary>
    public class HappyLiveShowUp_V102 : IGameParam
    {
        protected readonly byte[] mEncKeyFile;
        protected readonly byte[] mPatchKeyFile;
        protected readonly byte[] mGlobalKey;
        protected readonly Dictionary<string, DataPackInfo> mDataPack = new(16);

        public virtual byte[] EncKeyFile => this.mEncKeyFile;
        public virtual byte[] PatchKeyFile => this.mPatchKeyFile;
        public virtual byte[] GlobalKey => this.mGlobalKey;
        public virtual Dictionary<string, DataPackInfo> DataPack => this.mDataPack;

        public HappyLiveShowUp_V102()
        {
            //读取KeyFile
            Assembly assembly = Assembly.GetExecutingAssembly();

            {
                string encKey = assembly.GetName().Name + ".Enc_Key_V102";
                using Stream? stream = assembly.GetManifestResourceStream(encKey);
                if (stream is null)
                {
                    throw new IOException("KeyFile文件不存在");
                }
                if (stream.Length != 0x200000L)
                {
                    throw new IOException("KeyFile非法");
                }
                this.mEncKeyFile = new byte[0x200000];
                stream.Read(this.mEncKeyFile);
            }

            {
                string patchKey = assembly.GetName().Name + ".Patch_Key_V102";
                using Stream? stream = assembly.GetManifestResourceStream(patchKey);
                if (stream is null)
                {
                    throw new IOException("PatchKeyFile文件不存在");
                }
                if (stream.Length != 0x40000L)
                {
                    throw new IOException("PatchKeyFile非法");
                }
                this.mPatchKeyFile = new byte[0x40000];
                stream.Read(this.mPatchKeyFile);
            }

            //全局Key
            this.mGlobalKey = new byte[] 
            {
                0xF0, 0x21, 0x96, 0x38, 0x69, 0xB6, 0x1F, 0xF3, 0xD7, 0x8C, 0x67, 0xEE, 0x02, 0x07, 0xC8, 0x53,
                0x9C, 0xE2, 0x73, 0x3F, 0x04, 0x0B, 0x3C, 0xF2, 0xE0, 0x35, 0xE8, 0x8E, 0xB1, 0xA4, 0x44, 0x4E,
                0xC3, 0x84, 0x9F, 0x0E, 0x6E, 0x23, 0x11, 0xC2, 0x3B, 0x9B, 0x2A, 0x60, 0x08, 0x43, 0x8B, 0xCE,
                0xF5, 0xE5, 0xE9, 0x4D, 0x49, 0x17, 0xA0, 0xFB, 0x47, 0xA5, 0xBE, 0xD2, 0xAE, 0x8D, 0x87, 0x62,
                0xB2, 0x10, 0x5D, 0x32, 0x85, 0xF7, 0xD3, 0xAF, 0x86, 0x82, 0x25, 0xD4, 0xBD, 0x8F, 0xB3, 0x76,
                0x33, 0x0C, 0x01, 0x1A, 0xE7, 0xC0, 0x34, 0x29, 0x90, 0x2F, 0xBB, 0xC7, 0x6A, 0xB8, 0x22, 0x57,
                0x81, 0x50, 0x7C, 0xAA, 0x03, 0x12, 0xB5, 0xB7, 0x05, 0x65, 0x5F, 0x4C, 0x6B, 0x3E, 0xA2, 0xCD,
                0x4F, 0xBF, 0x28, 0x58, 0xCB, 0x52, 0x09, 0xCA, 0x74, 0xA7, 0x16, 0xFD, 0x37, 0xAB, 0x99, 0x93,
                0x8A, 0x88, 0x5C, 0x2C, 0x15, 0xE1, 0x4B, 0x2B, 0x79, 0x0D, 0x64, 0x95, 0x5E, 0x91, 0xE6, 0xD1,
                0x51, 0x41, 0x83, 0x2D, 0x75, 0x45, 0x1C, 0xF4, 0xE3, 0x80, 0x46, 0x71, 0x61, 0xCF, 0x7F, 0x3D,
                0x7D, 0x5A, 0x3A, 0xEB, 0x1B, 0xC6, 0xB9, 0xA9, 0x55, 0xEA, 0x2E, 0xA8, 0x1E, 0xF6, 0x7B, 0x18,
                0x19, 0x94, 0x00, 0xFA, 0xFC, 0x1D, 0xDE, 0xDB, 0x59, 0x24, 0xAC, 0xF1, 0xDD, 0xD9, 0x36, 0xB4,
                0x7E, 0xA1, 0x42, 0x6C, 0xEC, 0xC1, 0xA6, 0x9A, 0x66, 0x72, 0x98, 0x9D, 0xDA, 0xDC, 0x39, 0xFE,
                0x6D, 0xE4, 0xD6, 0x20, 0x13, 0x78, 0xED, 0xCC, 0x97, 0x0F, 0xC4, 0x92, 0x7A, 0x68, 0x70, 0x89,
                0x5B, 0x48, 0xDF, 0xA3, 0x56, 0x6F, 0x40, 0x30, 0xF8, 0xB0, 0x54, 0x9E, 0xAD, 0x0A, 0xD0, 0x27,
                0x14, 0x06, 0xD5, 0xBC, 0xC5, 0xF9, 0x31, 0x63, 0x4A, 0xEF, 0x26, 0xBA, 0x77, 0xD8, 0xC9, 0xFF,
            };

            //封包信息
            Dictionary<string, DataPackInfo> datapack = this.mDataPack;
            datapack.Add("data0.pack", new(0x3DAB7C00u, 0u, this.mEncKeyFile));
            datapack.Add("data1.pack", new(0x8F9D3998u, 1u, this.mEncKeyFile));
            datapack.Add("data2.pack", new(0xDBF011E9u, 2u, this.mEncKeyFile));
            datapack.Add("data3.pack", new(0x2FC40FFEu, 3u, this.mEncKeyFile));
            datapack.Add("data4.pack", new(0x57A42D00u, 4u, this.mEncKeyFile));
            datapack.Add("data5.pack", new(0x104A21CDu, 5u, this.mEncKeyFile));
            datapack.Add("data6.pack", new(0x97EDB0EBu, 6u, this.mEncKeyFile));
            datapack.Add("data7.pack", new(0x058E3F0Fu, 7u, this.mPatchKeyFile));
            datapack.Add("data8.pack", new(0xCBD2927Au, 8u, this.mPatchKeyFile));
            datapack.Add("data9.pack", new(0x3ACD5388u, 9u, this.mPatchKeyFile));
        }
    }
}
