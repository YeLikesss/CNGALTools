using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace StaticExtractor
{
    public class SPKArchiveV2
    {
        private readonly string mPackagePath = string.Empty;   //封包路径
        private readonly string mPackageName = string.Empty;   //封包名
        private readonly string mExtractDirectory = string.Empty;   //导出路径
        private readonly SwecCrypto mFilter = new();            //加密类

        /// <summary>
        /// 解包
        /// </summary>
        public void Extract()
        {
            if (File.Exists(this.mPackagePath) && !string.IsNullOrEmpty(this.mExtractDirectory))
            {
                using FileStream mStream = File.OpenRead(this.mPackagePath);
                using BinaryReader fileReader = new(mStream);

                //标志头
                Span<byte> sign = stackalloc byte[3];
                mStream.Read(sign);

                if (Encoding.ASCII.GetString(sign) == "SPK")
                {
                    mStream.Position = fileReader.ReadByte();

                    byte flag = fileReader.ReadByte();
                    if (flag == 0x80)
                    {
                        mStream.Position += 8L;
                        mStream.Position = fileReader.ReadInt64();
                    }
                    else
                    {
                        mStream.Position -= 1L;
                    }

                    //文件表信息
                    XP3IndexInfo xp3IndexInfo = new()
                    {
                        Compress = fileReader.ReadByte(),
                        CompressedSize = fileReader.ReadInt64(),
                        DecompressedSize = fileReader.ReadInt64()
                    };

                    //读取分析文件表并读取文件
                    List<XP3Entry> xp3Entries = new();
                    {
                        byte[] indexData = new byte[xp3IndexInfo.CompressedSize];
                        mStream.Read(indexData);

                        this.mFilter.Decrypt(indexData);        //解密

                        if (xp3IndexInfo.IsCompressed)
                        {
                            indexData = LZ4Helper.Decompress(indexData, (int)xp3IndexInfo.DecompressedSize);
                        }
                        using MemoryStream memIndexData = new(indexData, false);
                        using BinaryReader indexDataReader = new(memIndexData);

                        while (memIndexData.Position < memIndexData.Length)
                        {
                            XP3Entry entry = new();
                            //顺序读取各个字段

                            //文件信息
                            entry.FileSign = indexDataReader.ReadUInt32();
                            entry.FileInfoSize = indexDataReader.ReadInt64();

                            //保存文件信息起始位置
                            long fileInfoPos = memIndexData.Position;


                            //文件基本信息
                            entry.InfoSign = indexDataReader.ReadUInt32();
                            entry.BaseInfoSize = indexDataReader.ReadInt64();

                            //保存文件基本信息起始位置
                            long baseInfoPos = memIndexData.Position;

                            entry.Protect = indexDataReader.ReadUInt32();
                            entry.FileOriginalSize = indexDataReader.ReadInt64();
                            entry.FileActuallySize = indexDataReader.ReadInt64();
                            entry.FileNameLength = indexDataReader.ReadUInt16();      //读取字符串长度
                            //读取字符串   这个封包字符串长度包含(L'\0')
                            {
                                byte[] bytes = indexDataReader.ReadBytes(entry.FileNameLength * 2);
                                Span<char> str = MemoryMarshal.Cast<byte, char>(bytes);

                                int end = str.IndexOf('\0');
                                if (end >= 0)
                                {
                                    str = str[0..end];
                                }

                                entry.FileNameUTF16LE = new(str);
                            }

                            memIndexData.Position = baseInfoPos + entry.BaseInfoSize;    //设置下一块起始点

                            //文件段信息
                            entry.SegmSign = indexDataReader.ReadUInt32();
                            entry.FileSegmSize = indexDataReader.ReadInt64();

                            //保存文件段信息起始位置
                            long segmInfoPos = memIndexData.Position;

                            entry.Segments = new((int)entry.FileSegmSize / 28);

                            for (int i = 0; i < entry.FileSegmSize / 28; ++i)
                            {
                                XP3FileSegment segment = new()
                                {
                                    Compress = indexDataReader.ReadUInt32(),
                                    FileOffset = indexDataReader.ReadInt64(),
                                    DecompressedSize = indexDataReader.ReadInt64(),
                                    CompressedSize = indexDataReader.ReadInt64()
                                };

                                entry.Segments.Add(segment);
                            }

                            memIndexData.Position = segmInfoPos + entry.FileSegmSize;        //设置下一块起始点

                            //文件Hash信息
                            entry.AdlrSign = indexDataReader.ReadUInt32();
                            entry.FileAdlrSize = indexDataReader.ReadInt64();

                            entry.Hash = indexDataReader.ReadUInt32();


                            //设置下一个表起始点
                            memIndexData.Position = fileInfoPos + entry.FileInfoSize;

                            //添加到文件表数组
                            xp3Entries.Add(entry);
                        }
                    }

                    // 解密并导出文件
                    foreach (XP3Entry entry in xp3Entries)
                    {
                        using MemoryStream buffer = new();

                        foreach (XP3FileSegment mSegm in entry.Segments)
                        {
                            mStream.Position = mSegm.FileOffset;
                            byte[] data = new byte[mSegm.CompressedSize];
                            mStream.Read(data);

                            this.mFilter.Decrypt(data);     //解密

                            if (mSegm.IsCompressed)
                            {
                                data = LZ4Helper.Decompress(data, (int)mSegm.DecompressedSize);
                            }

                            buffer.Write(data);
                        }

                        string mExtractFileFullPath = Path.Combine(this.mExtractDirectory, entry.FileNameUTF16LE);
                        {
                            if (Path.GetDirectoryName(mExtractFileFullPath) is string dir && !Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                        }

                        using FileStream outFs = File.Create(mExtractFileFullPath);

                        int size = (int)buffer.Length;

                        outFs.Write(buffer.GetBuffer(), 0, size);
                        outFs.Flush();
                    }
                }
            }
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">封包路径</param>
        public SPKArchiveV2(string path)
        {
            this.mPackagePath = path;
            this.mPackageName = Path.GetFileNameWithoutExtension(path);

            if (Path.GetDirectoryName(path) is string dir)
            {
                this.mExtractDirectory = Path.Combine(dir, "Static_Extract", this.mPackageName);
            }
        }
    }
}
