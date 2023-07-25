using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using XP3Archive;

namespace XP3
{
    public class Archive
    {
        private readonly string mPackagePath = string.Empty;   //封包路径
        private readonly string mPackageName = string.Empty;       //封包名
        private readonly string mExtractDirectory = string.Empty;       //导出路径
        private readonly IXP3Filter? mFilter = null;        //加密

        /// <summary>
        /// 解包
        /// </summary>
        public void Extract()
        {
            if (File.Exists(this.mPackagePath) && !string.IsNullOrEmpty(this.mExtractDirectory))
            {
                using FileStream mStream = File.OpenRead(this.mPackagePath);
                using BinaryReader fileReader = new(mStream);

                //读取文件表信息偏移
                {
                    mStream.Position = 11;
                    mStream.Position = fileReader.ReadInt64();
                }

                //读文件表信息
                {
                    byte flag = fileReader.ReadByte();
                    if (flag == 0x80)
                    {
                        mStream.Position += 8;
                        mStream.Position = fileReader.ReadInt64();
                    }
                    else
                    {
                        mStream.Position -= 1;
                    }
                }

                XP3Archive.XP3Info xp3Info = new()
                {
                    Compress = fileReader.ReadByte(),
                    CompressedSize = fileReader.ReadInt64(),
                    DecompressedSize = fileReader.ReadInt64()
                };

                //读取分析文件表并读取文件
                List<XP3Archive.XP3File> xp3Files = new();
                {
                    byte[] indexData = fileReader.ReadBytes((int)xp3Info.CompressedSize);
                    //文件表压缩检测
                    if (xp3Info.IsCompressed)
                    {
                        indexData = Zlib.Decompress(indexData);
                    }

                    //初始化文件表读取器
                    using MemoryStream memIndexData = new(indexData, false);
                    using BinaryReader indexDataReader = new(memIndexData);
                    memIndexData.Position = 0;

                    //循环分析
                    while (memIndexData.Position < memIndexData.Length)
                    {
                        XP3Archive.XP3File mXP3File = new();
                        //顺序读取各个字段

                        //文件信息
                        mXP3File.FileSign = indexDataReader.ReadUInt32();
                        mXP3File.FileInfoSize = indexDataReader.ReadInt64();

                        //保存文件信息起始位置
                        long fileInfoPos = memIndexData.Position;


                        //文件基本信息
                        mXP3File.InfoSign = indexDataReader.ReadUInt32();
                        mXP3File.BaseInfoSize = indexDataReader.ReadInt64();

                        //保存文件基本信息起始位置
                        long baseInfoPos = memIndexData.Position;

                        mXP3File.Protect = indexDataReader.ReadUInt32();
                        mXP3File.FileOriginalSize = indexDataReader.ReadInt64();
                        mXP3File.FileActuallySize = indexDataReader.ReadInt64();
                        mXP3File.FileNameLength = indexDataReader.ReadUInt16();      //读取字符串长度
                        mXP3File.FileNameUTF16LE = Encoding.Unicode.GetString(indexDataReader.ReadBytes(mXP3File.FileNameLength * 2));   //读取字符串

                        memIndexData.Position = baseInfoPos + mXP3File.BaseInfoSize;    //设置下一块起始点

                        //文件段信息
                        mXP3File.SegmSign = indexDataReader.ReadUInt32();
                        mXP3File.FileSegmSize = indexDataReader.ReadInt64();

                        //保存文件段信息起始位置
                        long segmInfoPos = memIndexData.Position;

                        mXP3File.Segments = new((int)mXP3File.FileSegmSize / 28);

                        for (int i = 0; i < mXP3File.FileSegmSize / 28; ++i)
                        {
                            XP3Archive.XP3FileSegment segment = new()
                            {
                                Compress = indexDataReader.ReadUInt32(),
                                FileOffset = indexDataReader.ReadInt64(),
                                DecompressedSize = indexDataReader.ReadInt64(),
                                CompressedSize = indexDataReader.ReadInt64()
                            };

                            mXP3File.Segments.Add(segment);
                        }

                        memIndexData.Position = segmInfoPos + mXP3File.FileSegmSize;        //设置下一块起始点

                        //文件Hash信息
                        mXP3File.AdlrSign = indexDataReader.ReadUInt32();
                        mXP3File.FileAdlrSize = indexDataReader.ReadInt64();

                        mXP3File.Hash = indexDataReader.ReadUInt32();


                        //设置下一个表起始点
                        memIndexData.Position = fileInfoPos + mXP3File.FileInfoSize;

                        //添加到文件表数组
                        xp3Files.Add(mXP3File);
                    }
                }

                // 解密并导出文件
                foreach(XP3Archive.XP3File mXP3File in xp3Files)
                {
                    using MemoryStream buffer = new();

                    foreach(XP3Archive.XP3FileSegment mSegm in mXP3File.Segments)
                    {
                        mStream.Position = mSegm.FileOffset;
                        byte[] data = fileReader.ReadBytes((int)mSegm.CompressedSize);

                        if (mSegm.IsCompressed)
                        {
                            data = Zlib.Decompress(data);
                        }

                        buffer.Write(data);
                        buffer.Flush();
                    }

                    string mExtractFileFullPath = Path.Combine(this.mExtractDirectory, mXP3File.FileNameUTF16LE);
                    {
                        if(Path.GetDirectoryName(mExtractFileFullPath) is string dir)
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }

                    using FileStream outFs = new(mExtractFileFullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

                    int size = (int)buffer.Length;

                    this.mFilter?.Decrypt(buffer.GetBuffer().AsSpan()[0..size], mXP3File.Hash);

                    outFs.Write(buffer.GetBuffer(), 0, size);
                    outFs.Flush();
                }
            }
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">封包路径</param>
        /// <param name="filter">加密</param>
        public Archive(string path, IXP3Filter? filter = null)
        {
            this.mPackagePath = path;
            this.mPackageName = Path.GetFileNameWithoutExtension(path);

            if(Path.GetDirectoryName(path) is string dir)
            {
                this.mExtractDirectory = Path.Combine(dir, "Static_Extract", this.mPackageName);
            }

            this.mFilter = filter;
        }
    }
}
