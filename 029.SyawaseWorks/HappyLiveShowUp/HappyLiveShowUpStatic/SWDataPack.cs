using System;
using System.Linq;
using System.IO;
using System.IO.Hashing;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace HappyLiveShowUpStatic
{
    /// <summary>
    /// Syawase封包
    /// </summary>
    public class SWDataPack : IDisposable
    {
        /// <summary>
        /// 文件头
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0xC)]
        private struct Header
        {
            /// <summary>
            /// 文件个数
            /// </summary>
            public uint FileCount;
            /// <summary>
            /// 文件表偏移
            /// </summary>
            public long EntryOffset;
        }

        private readonly string mDirectory;             //文件夹路径
        private readonly string mFileName;              //文件名
        private readonly FileStream mFileStream;        //文件流
        private readonly IGameParam mParam;             //游戏配置参数

        /// <summary>
        /// 封包名
        /// </summary>
        public string FileName => this.mFileName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="directory">文件夹路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="param">游戏信息</param>
        private SWDataPack(string directory, string fileName, FileStream fileStream, IGameParam param)
        {
            this.mDirectory = directory;
            this.mFileName = fileName;
            this.mFileStream = fileStream;
            this.mParam = param;
        }

        /// <summary>
        /// 解密封包
        /// </summary>
        public void Decrypt()
        {
            string outFilePath = Path.Combine(this.mDirectory, "dec_" + this.mFileName);

            //解密参数
            IGameParam param = this.mParam;
            DataPackInfo packInfo = param.DataPack[this.mFileName.ToLower()];
            
            //封包key
            byte key = SWDataPack.CalculateKey(packInfo.Order);

            //文件key表
            byte[] keyFileTable = new byte[0x40000];
            SWCrypto.Xor(keyFileTable, packInfo.KeyFile.AsSpan().Slice(0, 0x40000), key);

            //全局Key表
            byte[] globalKeyTable = new byte[0x100];
            SWCrypto.Transform256Bytes(globalKeyTable, param.GlobalKey);


            FileStream inFs = this.mFileStream;
            long fileLength = inFs.Length;

            using MemoryMappedFile inMapFile = MemoryMappedFile.CreateFromFile(inFs, null, 0L, MemoryMappedFileAccess.Read, HandleInheritability.None, true);
            using MemoryMappedViewAccessor inFileAccessor = inMapFile.CreateViewAccessor(0L, fileLength, MemoryMappedFileAccess.Read);

            //获取文件头偏移
            long swHeaderOffset = SWDataPack.DecryptHeaderOffset(inFileAccessor.ReadInt64(fileLength - 0x8L));

            //获取文件个数与文件表偏移
            inFileAccessor.Read(swHeaderOffset, out SWDataPack.Header header);
            header.FileCount = SWDataPack.DecryptFileCount(header.FileCount, key);
            header.EntryOffset = SWDataPack.DecryptEntryOffset(header.EntryOffset, key);


            using MemoryMappedFile outMapFile = MemoryMappedFile.CreateFromFile(outFilePath, FileMode.Create, null, fileLength, MemoryMappedFileAccess.ReadWrite);
            using MemoryMappedViewAccessor outFileAccessor = outMapFile.CreateViewAccessor(0L, fileLength, MemoryMappedFileAccess.ReadWrite);

            long qlieHeaderOffset = fileLength - 0x1CL;
            //解密
            {
                byte[] data = new byte[0x40000];
                long position = 0L;
                while (position < qlieHeaderOffset)
                {
                    int readLen = inFileAccessor.ReadArray(position, data, 0, data.Length);

                    SWCrypto.Transform8BytesBlockLine(data);
                    SWCrypto.Transform16BytesBlockLine(data);
                    SWCrypto.Transform4BytesBlockLine(data);
                    SWCrypto.TransformBlockLineBy256BytesTable(data, globalKeyTable);
                    SWCrypto.Xor(data, keyFileTable, 0L);

                    outFileAccessor.WriteArray(position, data, 0, readLen);

                    position += readLen;
                }
            }

            //写文件头
            {
                byte[] sign = SWDataPack.Signature;
                outFileAccessor.WriteArray(qlieHeaderOffset, sign, 0, sign.Length);
                outFileAccessor.Write<SWDataPack.Header>(qlieHeaderOffset + sign.Length, ref header);
            }

            outFileAccessor.Flush();
        }

        public void Dispose()
        {
            this.mFileStream.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 标记
        /// </summary>
        private static readonly byte[] Signature = new byte[]
        {
            0x46, 0x69, 0x6C, 0x65, 0x50, 0x61, 0x63, 0x6B, 0x56, 0x65, 0x72, 0x33, 0x2E, 0x31, 0x00, 0x00
        };

        /// <summary>
        /// 尝试打开文件
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <param name="version">游戏版本</param>
        /// <param name="message">信息</param>
        /// <returns></returns>
        public static SWDataPack? TryOpen(string filePath, GameVersion version)
        {
            //检查版本
            IGameParam? param = SWGameData.CreateFactory(version);
            if(param is null)
            {
                Console.WriteLine($"[{version}]未知的版本");
                return null;
            }

            FileInfo fileInfo = new(filePath);
            if (!fileInfo.Exists)
            {
                Console.WriteLine($"[{fileInfo.Name}]文件不存在");
                return null;
            }
            if (!param.DataPack.TryGetValue(fileInfo.Name.ToLower(), out DataPackInfo? packInfo))
            {
                Console.WriteLine($"[{fileInfo.Name}]封包不存在");
                return null;
            }
            if(fileInfo.Length < 0x1CL || (fileInfo.Length - 0x1CL) % 0x40000L != 0L)
            {
                Console.WriteLine($"[{fileInfo.Name}]封包大小错误");
                return null;
            }

            using FileStream inFs = fileInfo.OpenRead();

            //检查文件头
            {
                inFs.Position = fileInfo.Length - 0x1CL;
                Span<byte> sign = stackalloc byte[0x10];
                inFs.Read(sign);
                if (!sign.SequenceEqual(SWDataPack.Signature))
                {
                    Console.WriteLine($"[{fileInfo.Name}]非法封包");
                    return null;
                }
            }

            //检查CRC
            {
                inFs.Position = 0L;
                Crc32 crc = new();
                crc.Append(inFs);
                if (crc.GetCurrentHashAsUInt32() != packInfo.CRC32Hash)
                {
                    Console.WriteLine($"[{fileInfo.Name}]封包校验失败");
                    return null;
                }
            }

            return new SWDataPack(fileInfo.DirectoryName!, fileInfo.Name, fileInfo.OpenRead(), param);
        }


        /// <summary>
        /// 解密封包头偏移
        /// <para>HappyLiveShowup.dll(v102) Themida3.x VM</para>
        /// </summary>
        /// <param name="encOffset">加密的封包头偏移</param>
        /// <returns>封包头偏移</returns>
        public static long DecryptHeaderOffset(long encOffset)
        {
            return encOffset - 0x605L + 0x80000L;
        }

        /// <summary>
        /// 计算封包Key
        /// <para>HappyLiveShowup.dll(v102) Themida3.x VM</para>
        /// </summary>
        /// <param name="order">封包序号</param>
        /// <returns>封包key</returns>
        public static byte CalculateKey(uint order)
        {
            return (byte)(order * 0x000000D0u + 0x000000CDu);
        }

        /// <summary>
        /// 解密文件个数
        /// <para>HappyLiveShowup.dll(v102) Themida3.x VM</para>
        /// </summary>
        /// <param name="encCount">加密的文件个数值</param>
        /// <param name="key">封包key</param>
        /// <returns>文件个数</returns>
        public static uint DecryptFileCount(uint encCount, byte key)
        {
            return encCount ^ (key * 0xD3B496C8u);
        }

        /// <summary>
        /// 解密文件表偏移
        /// <para>HappyLiveShowup.dll(v102) Themida3.x VM</para>
        /// </summary>
        /// <param name="encOffset">加密的文件表偏移</param>
        /// <param name="key">封包key</param>
        /// <returns>文件表偏移</returns>
        public static long DecryptEntryOffset(long encOffset, byte key)
        {
            return encOffset ^ (long)(key * 0x7B3A91171D840419ul);
        }
    }
}
