using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NVLKR2Static
{
    /// <summary>
    /// XP3封包
    /// </summary>
    public class XP3Archive
    {
        /// <summary>
        /// 封包文件列表
        /// </summary>
        public struct XP3File
        {
            /// <summary>
            /// File标记
            /// </summary>
            public uint FileSign;
            /// <summary>
            /// 文件信息大小
            /// </summary>
            public long FileInfoSize;
            /// <summary>
            /// info标记 I
            /// </summary>
            public uint InfoSign;
            /// <summary>
            /// 基本信息大小
            /// </summary>
            public long BaseInfoSize;
            /// <summary>
            /// 加密标记
            /// </summary>
            public uint Protect;
            /// <summary>
            /// 文件原始大小(解压后)
            /// </summary>
            public long FileOriginalSize;
            /// <summary>
            /// 文件实际大小(解压前)
            /// </summary>
            public long FileActuallySize;

            /// <summary>
            /// HashKey
            /// </summary>
            public uint HashKey;
            /// <summary>
            /// 文件名Hash
            /// </summary>
            public uint NameHash;
            /// <summary>
            /// 后缀Hash
            /// </summary>
            public uint ExtensionHash;

            /// <summary>
            /// segm标记
            /// </summary>
            public uint SegmSign;
            /// <summary>
            /// 文件段大小
            /// </summary>
            public long FileSegmSize;

            /// <summary>
            /// 段结构
            /// </summary>
            public List<XP3FileSegment> Segments;

            /// <summary>
            /// adlr标记
            /// </summary>
            public uint AdlrSign;
            /// <summary>
            /// 文件附加数据大小
            /// </summary>
            public long FileAdlrSize;
            /// <summary>
            /// Adlr32
            /// </summary>
            public uint Adlr32;
        }

        /// <summary>
        /// 封包数据块
        /// </summary>
        public struct XP3FileSegment
        {
            /// <summary>
            /// 压缩标记
            /// </summary>
            public uint Compress;
            /// <summary>
            /// 文件在封包内偏移
            /// </summary>
            public long FileOffset;
            /// <summary>
            /// 文件原始大小(解压后)
            /// </summary>
            public long DecompressedSize;
            /// <summary>
            /// 文件实际大小(解压前)
            /// </summary>
            public long CompressedSize;
            /// <summary>
            /// 获取文件是否已压缩
            /// </summary>
            public bool IsCompressed => this.Compress == 0x00000001;
        }


        /// <summary>
        /// 文件信息表
        /// </summary>
        public struct XP3Info
        {
            /// <summary>
            /// 表压缩标记
            /// </summary>
            public byte Compress;
            /// <summary>
            /// 表在封包大小(解压前)
            /// </summary>
            public long CompressedSize;
            /// <summary>
            /// 表原始大小(解压后)
            /// </summary>
            public long DecompressedSize;

            /// <summary>
            /// 获取表是否已压缩
            /// </summary>
            public bool IsCompressed => this.Compress == 0x01;
        }


        /// <summary>
        /// 封包名
        /// </summary>
        public string PackageName { get; private set; }
        /// <summary>
        /// 获取是否已经释放
        /// </summary>
        public bool IsDispose => this.mStream is null;


        private Stream mStream;
        private List<XP3File> mFileEntries;

        /// <summary>
        /// 解析文件表
        /// </summary>
        public void ParseFileEntry()
        {
            if (this.IsDispose)
            {
                return;
            }


            Stream stream = this.mStream;
            //初始化文件读取器
            using BinaryReader fileReader = new(stream, Encoding.Default, true);

            //读取文件表信息偏移
            stream.Position = 0x20;
            long xp3InfoOffset = fileReader.ReadInt64();

            //读文件表信息
            stream.Position = xp3InfoOffset;
            XP3Archive.XP3Info xp3Info = new()
            {
                Compress = fileReader.ReadByte(),
                CompressedSize = fileReader.ReadInt64(),
                DecompressedSize = fileReader.ReadInt64()
            };

            byte[] indexBuffer = ArrayPool<byte>.Shared.Rent((int)xp3Info.CompressedSize);
            int indexReadLen = stream.Read(indexBuffer, 0, (int)xp3Info.CompressedSize);

            Stream memIndexStream = new MemoryStream(indexBuffer, 0, indexReadLen, false);
            //文件表压缩检测
            if (xp3Info.IsCompressed)
            {
                memIndexStream = Zlib.CreateDecompressStream(memIndexStream);
            }

            //初始化文件表读取器
            using BinaryReader indexDataReader = new(memIndexStream, Encoding.Default, true);
            memIndexStream.Position = 0;

            //读取分析文件表并读取文件
            List<XP3File> xp3Files = new(64);

            while (memIndexStream.Position < memIndexStream.Length)
            {
                XP3File mXP3File = new();
                //顺序读取各个字段

                //文件信息
                mXP3File.FileSign = indexDataReader.ReadUInt32();
                mXP3File.FileInfoSize = indexDataReader.ReadInt64();

                //保存文件信息起始位置
                long fileInfoPos = memIndexStream.Position;


                //文件基本信息
                mXP3File.InfoSign = indexDataReader.ReadUInt32();
                mXP3File.BaseInfoSize = indexDataReader.ReadInt64();

                //保存文件基本信息起始位置
                long baseInfoPos = memIndexStream.Position;

                mXP3File.Protect = indexDataReader.ReadUInt32();
                mXP3File.FileOriginalSize = indexDataReader.ReadInt64();
                mXP3File.FileActuallySize = indexDataReader.ReadInt64();

                mXP3File.HashKey = indexDataReader.ReadUInt32();
                mXP3File.NameHash = indexDataReader.ReadUInt32();
                mXP3File.ExtensionHash = indexDataReader.ReadUInt32();

                memIndexStream.Position = baseInfoPos + mXP3File.BaseInfoSize;    //设置下一块起始点

                //文件段信息
                mXP3File.SegmSign = indexDataReader.ReadUInt32();
                mXP3File.FileSegmSize = indexDataReader.ReadInt64();

                //保存文件段信息起始位置
                long segmInfoPos = memIndexStream.Position;

                mXP3File.Segments = new((int)mXP3File.FileSegmSize / 28);

                for (int i = 0; i < mXP3File.FileSegmSize / 28; ++i)
                {
                    XP3FileSegment segment = new()
                    {
                        Compress = indexDataReader.ReadUInt32(),
                        FileOffset = indexDataReader.ReadInt64(),
                        DecompressedSize = indexDataReader.ReadInt64(),
                        CompressedSize = indexDataReader.ReadInt64()
                    };

                    mXP3File.Segments.Add(segment);
                }

                memIndexStream.Position = segmInfoPos + mXP3File.FileSegmSize;        //设置下一块起始点

                //文件Hash信息
                mXP3File.AdlrSign = indexDataReader.ReadUInt32();
                mXP3File.FileAdlrSize = indexDataReader.ReadInt64();

                mXP3File.Adlr32 = indexDataReader.ReadUInt32();


                //设置下一个表起始点
                memIndexStream.Position = fileInfoPos + mXP3File.FileInfoSize;

                //添加到文件表数组
                xp3Files.Add(mXP3File);
            }
            
            this.mFileEntries = xp3Files;

            memIndexStream.Dispose();
            ArrayPool<byte>.Shared.Return(indexBuffer);
        }

        /// <summary>
        /// 提取
        /// </summary>
        /// <param name="outDirectoty">目标文件夹</param>
        /// <param name="keyInformation">游戏key信息</param>
        public void Extract(string outDirectoty, IKeyInformation keyInformation)
        {
            Stream stream = this.mStream;

            int bufferLen = 1024 * 1024 * 16;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferLen);

            foreach(XP3File xp3File in this.mFileEntries)
            {
                string outPath = string.Empty;
                {
                    string fileNameHash = (xp3File.NameHash ^ xp3File.HashKey).ToString("X8");
                    string extHash = (xp3File.ExtensionHash ^ xp3File.HashKey).ToString("X8");

                    outPath = Path.Combine(outDirectoty, this.PackageName, string.Format("{0}.{1}", fileNameHash, extHash));

                    string dir = Path.GetDirectoryName(outPath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                using FileStream outFs = new(outPath, FileMode.Create, FileAccess.ReadWrite);

                XP3Filter filter = new(xp3File, keyInformation);

                long arcOffset = 0;
                foreach(XP3FileSegment segm in xp3File.Segments)
                {
                    //自动扩容
                    if (segm.DecompressedSize > bufferLen)
                    {
                        bufferLen = (int)segm.DecompressedSize;
                        ArrayPool<byte>.Shared.Return(buffer);
                        buffer = ArrayPool<byte>.Shared.Rent(bufferLen);
                    }
                    stream.Position = segm.FileOffset;

                    //读取封包资源
                    int readLen = stream.Read(buffer, 0, (int)segm.CompressedSize);

                    //文件已压缩
                    if (segm.IsCompressed)
                    {
                        using Stream decompressed = Zlib.CreateDecompressStream(new MemoryStream(buffer, 0, readLen));
                        readLen = decompressed.Read(buffer, 0, (int)decompressed.Length);
                    }

                    //解密
                    filter.Decrypt(buffer.AsSpan()[0..readLen], arcOffset);
                    outFs.Write(buffer, 0, readLen);

                    arcOffset += readLen;
                }
                outFs.Flush();
                outFs.Close();
            }
        }


        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            this.mStream?.Dispose();
            this.mStream = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="packageName">封包名</param>
        /// <param name="stream">文件流</param>
        public XP3Archive(string packageName, Stream stream)
        {
            this.PackageName = packageName;
            this.mStream = stream;
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="filePath">封包全路径</param>
        /// <returns></returns>
        public static XP3Archive CreateInstance(string filePath)
        {
            XP3Archive archive = null;
            try
            {
                archive = new(Path.GetFileNameWithoutExtension(filePath), File.OpenRead(filePath));
                archive.ParseFileEntry();
            }
            catch
            {
                archive?.Dispose();
                archive = null;
            }
            return archive;
        }


        /// <summary>
        /// 文件名hash
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static uint StringHash(string name)
        {
            uint hash = 0;
            ReadOnlySpan<byte> s = MemoryMarshal.Cast<char, byte>(name.AsSpan());
            for(int i = 0; i< s.Length; ++i)
            {
                hash = 0x1000193 * hash ^ s[i];
            }
            return hash;
        }

        /// <summary>
        /// 批量计算hash
        /// </summary>
        /// <param name="strings">字符串表</param>
        /// <returns>字典对象 Key->Hash字符串 Value->原字符串</returns>
        public static Dictionary<string, string> CalculateHashMulit(List<string> strings)
        {
            Dictionary<string, string> maps = new(strings.Count);

            foreach(string s in strings)
            {
                maps.TryAdd(XP3Archive.StringHash(s).ToString("X8"), s);
            }
            return maps;
        }
    }
}
