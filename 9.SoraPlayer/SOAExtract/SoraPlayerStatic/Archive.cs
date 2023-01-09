using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SoraPlayerStatic
{
    public class Archive
    {
        private string mFileName;       //封包名
        private string mExtractDirectory;       //导出路径
        private FileStream mFileStream;     //当前文件流

        /// <summary>
        /// 解包
        /// </summary>
        public bool Extract()
        {
            FileStream mStream = this.mFileStream;

            if (mStream == null || mStream.Length <= 0)
            {
                return false;
            }

            //初始化文件读取器
            using BinaryReader fileReader = new(mStream);

            //读取文件表信息偏移
            mStream.Position = 0x20;
            long xp3InfoOffset = fileReader.ReadInt64();

            //读文件表信息
            mStream.Position = xp3InfoOffset;
            XP3Archive.XP3Info xp3Info = new()
            {
                Compress = fileReader.ReadByte(),
                CompressedSize = fileReader.ReadInt64(),
                DecompressedSize = fileReader.ReadInt64()
            };

            byte[] indexData = fileReader.ReadBytes((int)xp3Info.CompressedSize);
            //文件表压缩检测
            if (xp3Info.IsCompressed)
            {
                indexData = Zlib.Decompress(indexData);
            }

            //初始化文件表读取器
            using MemoryStream memIndexData = new (indexData);
            using BinaryReader indexDataReader = new (memIndexData);
            memIndexData.Position = 0;

            //读取分析文件表并读取文件
            List<XP3Archive.XP3File> xp3Files = new ();
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

            // 解密并导出文件
            foreach(var mXP3File in CollectionsMarshal.AsSpan(xp3Files))
            {
                using MemoryStream buffer = new();

                foreach(var mSegm in CollectionsMarshal.AsSpan(mXP3File.Segments))
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
               
                //合并获得文件全路径
                string mExtractFileFullPath = Path.Combine(this.mExtractDirectory, mXP3File.FileNameUTF16LE);
                //检查文件夹是否存在  不存在则创建
                if (Directory.Exists(Path.GetDirectoryName(mExtractFileFullPath)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(mExtractFileFullPath));
                }
                //写入文件
                File.WriteAllBytes(mExtractFileFullPath, buffer.ToArray());
            }

            return true;
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path"></param>
        public Archive(string path)
        {
            this.mFileStream = new (path, FileMode.Open, FileAccess.Read, FileShare.Read);
            this.mFileName = Path.GetFileNameWithoutExtension(path);
            this.mExtractDirectory = Path.Combine(Path.GetDirectoryName(path), "Extract", this.mFileName);
        }
    }
}
