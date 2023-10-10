using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Buffers;
using System.Runtime.InteropServices;

namespace HamidashiCreativeStatic
{
    /// <summary>
    /// Artemis文件表
    /// </summary>
    public class ArtemisEntry
    {
        /// <summary>
        /// 名称
        /// </summary>
        public byte[] Name { get; }
        /// <summary>
        /// 文件偏移
        /// </summary>
        public long Offset { get; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="description">描述数据</param>
        /// <param name="offset">文件偏移</param>
        /// <param name="size">文件大小</param>
        public ArtemisEntry(byte[] description, long offset, int size)
        {
            this.Name = description;
            this.Offset = offset;
            this.Size = size;
        }
    }

    /// <summary>
    /// Artemis封包
    /// </summary>
    public class ArtemisArchive
    {
        /// <summary>
        /// 文件表 校验值
        /// </summary>
        public byte[] EntrySHA1 { get; protected set; } = Array.Empty<byte>();

        /// <summary>
        /// 文件表
        /// </summary>
        public List<ArtemisEntry> Entries { get; } = new();

        private ArtemisArchive() 
        {
        }

        /// <summary>
        /// 创建Artemis封包对象
        /// </summary>
        /// <param name="stream">资源流</param>
        public static ArtemisArchive? CreateInstance(Stream stream)
        {
            using BinaryReader br = new(stream, Encoding.Default, true);

            Span<byte> sign = stackalloc byte[3];
            stream.Read(sign);

            //pf8头
            if (sign[0] == 0x70 && sign[1] == 0x66 && sign[2] == 0x38)
            {
                ArtemisArchive archive = new();
                {
                    int indexSize = br.ReadInt32();
                    int count = br.ReadInt32();

                    archive.Entries.Capacity = count;

                    //解析文件表
                    for (int i = 0; i < count; ++i)
                    {
                        int length = br.ReadInt32();

                        byte[] description = br.ReadBytes(length);

                        stream.Position += 4;

                        long offset = br.ReadInt32();
                        int size = br.ReadInt32();

                        archive.Entries.Add(new(description, offset, size));
                    }

                    //SHA1
                    {
                        byte[] buf = ArrayPool<byte>.Shared.Rent(indexSize);

                        Span<byte> indexData = buf.AsSpan().Slice(0, indexSize);
                        stream.Position = 0;
                        stream.Read(indexData);

                        archive.EntrySHA1 = SHA1.HashData(indexData);

                        ArrayPool<byte>.Shared.Return(buf);
                    }
                }
                return archive;
            }
            return null;
        }
    }

    /// <summary>
    /// SyawaseWork文件表
    /// </summary>
    public class SWEntryV1
    {
        /// <summary>
        /// 文件Hash
        /// </summary>
        public ulong Hash { get; }
        /// <summary>
        /// 文件偏移
        /// </summary>
        public long Offset { get; }
        /// <summary>
        /// 文件种子
        /// </summary>
        public uint Seed { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hash">文件Hash</param>
        /// <param name="offset">文件Key</param>
        /// <param name="seed">文件种子</param>
        public SWEntryV1(ulong hash, long offset, uint seed)
        {
            this.Hash = hash;
            this.Offset = offset;
            this.Seed = seed;
        }


        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="dest">输出 0x18个uint32</param>
        /// <param name="src">输入 0x18字节</param>
        public static bool Base64(Span<uint> dest, ReadOnlySpan<byte> src)
        {
            if (src.Length < 0x18 || dest.Length < 0x18)
            {
                return false;
            }

            dest.Clear();

            int srcPos = 0;
            int destPos = 0;

            while (srcPos < 0x18)
            {
                uint value = 0;

                int offset = 0;
                {
                    uint v0 = src[srcPos + 0];
                    if (v0 > 0x7F)
                    {
                        uint v1 = src[srcPos + 1];
                        if (v0 > 0xDF)
                        {
                            uint v2 = src[srcPos + 2];
                            if (v0 > 0xEF)
                            {
                                uint v3 = src[srcPos + 3];
                                if (v0 > 0xF7)
                                {
                                    break;
                                }
                                else
                                {
                                    offset = 4;
                                    value = ((((((v0 & 0x7) << 6) | (v1 & 0x3F)) << 6) | (v2 & 0x3F)) << 6) | (v3 & 0x3F);
                                }
                            }
                            else
                            {
                                offset = 3;
                                value = ((((v0 & 0xF) << 6) | (v1 & 0x3F)) << 6) | (v2 & 0x3F);
                            }
                        }
                        else
                        {
                            offset = 2;
                            value = ((v0 & 0x1F) << 6) | (v1 & 0x3F);
                        }
                    }
                    else
                    {
                        offset = 1;
                        value = v0 & 0x7F;
                    }
                }
                srcPos += offset;

                //保存结果
                {
                    dest[destPos] = value;
                    ++destPos;
                }
            }

            return true;
        }

        /// <summary>
        /// 生成key
        /// </summary>
        /// <param name="dest">目标数据 8字节</param>
        /// <param name="src">源数据 8个uint32元素</param>
        public static bool GenerateKey(Span<byte> dest, ReadOnlySpan<uint> src)
        {
            if (src.Length < 0x8 || dest.Length < 0x8)
            {
                return false;
            }

            for (int i = 0; i < 8; ++i)
            {
                dest[i] = (byte)((src[i] >> 1) ^ 0xA9);
            }

            return true;
        }
    }

    /// <summary>
    /// 封包信息
    /// </summary>
    public class SWArtemisArchive : IDisposable
    {
        /// <summary>
        /// 封包路径
        /// </summary>
        public string ArchiveFullPath { get; protected set; } = string.Empty;

        /// <summary>
        /// 文件表
        /// </summary>
        public List<SWEntryV1> Entries { get; } = new();

        /// <summary>
        /// 提取文件
        /// </summary>
        public bool Extract()
        {
            string path = this.ArchiveFullPath;
            if(!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                string outputDirectory = string.Empty;
                string folderName = string.Empty;
                {
                    if(Path.GetDirectoryName(path) is string dir)
                    {
                        outputDirectory = Path.Combine(dir, "Static_Extract");
                    }

                    string pckName = Path.GetFileName(path);
                    int start = pckName.IndexOf('.') + 1;
                    int end = pckName.IndexOf('.', start);
                    if (end != -1)
                    {
                        folderName = pckName[start..end];
                    }
                }

                using FileStream inFs = File.OpenRead(path);
                using BinaryReader inBr = new(inFs, Encoding.Default, true);


                int bufSize = 4 * 1024 * 1024;
                byte[] buf = ArrayPool<byte>.Shared.Rent(bufSize);

                foreach(SWEntryV1 entry in this.Entries)
                {
                    inFs.Position = entry.Offset;

                    int length = inBr.ReadInt32() ^ 0x18056468;
                    uint key = inBr.ReadUInt32() ^ 0x18056468;

                    if (length > bufSize)
                    {
                        bufSize = length;
                        ArrayPool<byte>.Shared.Return(buf);
                        buf = ArrayPool<byte>.Shared.Rent(bufSize);
                    }

                    //读取并解密
                    inFs.Read(buf, 0, length);
                    SWFilterV1 filter = new(key);
                    filter.Decrypt(buf, 0, length);

                    //导出文件
                    {
                        string fileName = entry.Hash.ToString("X16");
                        string relativePath = Path.Combine(folderName, fileName);
                        string outFullPath = Path.Combine(outputDirectory, relativePath);

                        if(Path.GetDirectoryName(outFullPath) is string dir)
                        {
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                        }

                        using FileStream outFs = new(outFullPath, FileMode.Create, FileAccess.ReadWrite);
                        outFs.Write(buf, 0, length);
                        outFs.Flush();

                        Console.WriteLine("Extract Success: {0}", relativePath);
                    }
                }

                ArrayPool<byte>.Shared.Return(buf);

                return true;
            }
            return false;
        }

        public void Dispose()
        {
        }

        private SWArtemisArchive()
        {
        }

        /// <summary>
        /// 创建封包对象
        /// </summary>
        /// <param name="fullpath">文件全路径</param>
        /// <returns>封包对象</returns>
        public static SWArtemisArchive? CreateInstance(string fullpath)
        {
            if (File.Exists(fullpath))
            {
                using FileStream fs = File.OpenRead(fullpath);
                using BinaryReader br = new(fs, Encoding.Default, true);

                if(ArtemisArchive.CreateInstance(fs) is null)       //pf8 Fake Parser
                {
                    //pfz Parser
                }
                else
                {
                    //pf8 Truth Parser
                    SWArtemisArchive archive = new();

                    fs.Position = 0x14;

                    int indexSize;
                    {
                        int v0 = br.ReadInt32();
                        int v1 = br.ReadInt32();
                        indexSize = ((v1 - 0xC) << 0x10) + (ushort)v0;
                    }

                    //解析文件表
                    {
                        Span<byte> description = stackalloc byte[0x30];
                        Span<uint> base64Output = stackalloc uint[0x18];
                        Span<byte> decResult = stackalloc byte[8];

                        int count = indexSize / 0x40;

                        archive.Entries.Capacity = count;

                        for (int i = 0; i < count; ++i)
                        {
                            fs.Position += 4;
                            fs.Read(description);

                            ulong hash = 0;
                            long offset = 0;
                            uint seed = 0;

                            SWEntryV1.Base64(base64Output, description.Slice(0, 0x18));
                            SWEntryV1.GenerateKey(decResult, base64Output);

                            hash = MemoryMarshal.Read<ulong>(decResult);

                            SWEntryV1.Base64(base64Output, description.Slice(0x18, 0x18));
                            SWEntryV1.GenerateKey(decResult, base64Output);

                            offset = MemoryMarshal.Read<long>(decResult);

                            fs.Position += 4;

                            {
                                long v0 = br.ReadUInt32();
                                long v1 = br.ReadUInt32();

                                ulong v2 = (ulong)(v0 + v1 - offset - 8) & 0xFFFFFFFFFFFFFFC0;

                                seed = (uint)v2;
                            }

                            archive.Entries.Add(new(hash, offset, seed));
                        }
                    }
                    archive.ArchiveFullPath = fullpath;

                    return archive;
                }
            }
            return null;
        }
    }
}