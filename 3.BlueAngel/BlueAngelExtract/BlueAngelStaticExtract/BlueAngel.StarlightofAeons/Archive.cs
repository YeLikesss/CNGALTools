using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueAngel.V1;
using System.IO;
using System.Threading.Tasks;
using System.Buffers;
using static BlueAngel.XP3Archive;

namespace BlueAngel.StarlightofAeons
{
    public class Archive
    {
        private string mFilePath;
        private string mFileName;
        private string mExtractDirectory;

        //public List<uint> mTableKey32_1 = Key.TableKey32_1;
        //public List<uint> mTableKey32_2 = Key.TableKey32_2;
        //public List<uint> mTableKey32_3 = Key.TableKey32_3;
        //public List<uint> mTableKey32_4 = Key.TableKey32_4;
        //public List<uint> mTableKey32_5 = Key.TableKey32_5;
        //public List<uint> mTableKey32_6 = Key.TableKey32_6;
        //public List<uint> mTableKey32_7 = Key.TableKey32_7;
        //public List<uint> mTableKey32_8 = Key.TableKey32_8;
        //public List<uint> mTableKey32_9 = Key.TableKey32_9;

        //public List<byte> mTableKey8_1 = Key.TableKey8_1;
        //public List<byte> mTableKey8_2 = Key.TableKey8_2;

        public List<uint> mTableKey32_1 = null;
        public List<uint> mTableKey32_2 = null;
        public List<uint> mTableKey32_3 = null;
        public List<uint> mTableKey32_4 = null;
        public List<uint> mTableKey32_5 = null;
        public List<uint> mTableKey32_6 = null;
        public List<uint> mTableKey32_7 = null;
        public List<uint> mTableKey32_8 = null;
        public List<uint> mTableKey32_9 = null;

        public List<byte> mTableKey8_1 = null;
        public List<byte> mTableKey8_2 = null;

        public int mRound = Key.Round;

        /// <summary>
        /// 导出文件
        /// </summary>
        /// <returns></returns>
        public void Extract()
        {
            FileStream xp3Stream = File.OpenRead(this.mFilePath);

            //初始化读取器
            using BinaryReader binReader = new(xp3Stream);

            //读取文件表信息偏移
            xp3Stream.Position = 0x10;
            long xp3InfoOffset = BitConverter.ToInt64(this.Decrypt(binReader.ReadBytes(8)));

            //读取文件表信息
            XP3Archive.XP3Info xp3Info = new();
            xp3Stream.Position = xp3InfoOffset;
            xp3Info.Compress = binReader.ReadByte();            //读压缩标记
            xp3Info.CompressedSize = BitConverter.ToUInt64(this.Decrypt(binReader.ReadBytes(8)));       //读压缩后长度
            xp3Info.DecompressedSize = BitConverter.ToUInt64(this.Decrypt(binReader.ReadBytes(8)));     //读解压后的长度

            byte[] infoData = this.Decrypt(binReader.ReadBytes((int)xp3Info.CompressedSize));       //读文件表
            //文件表压缩检测
            if (xp3Info.IsCompressed)
            {
                infoData = LZ4Helper.Decompress(infoData, (int)xp3Info.DecompressedSize);
            }

            //初始化文件表读取器
            using MemoryStream memInfoData = new(infoData, false);
            using BinaryReader infoDataReader = new(memInfoData,Encoding.Unicode);   
            memInfoData.Position = 0;

            //读取分析文件表并读取文件
            List<XP3Archive.XP3File> xp3Files = new();
            //循环分析
            while (memInfoData.Position < memInfoData.Length)
            {
                XP3Archive.XP3File mXP3File = new();
                //顺序读取各个字段
                mXP3File.FileSign = infoDataReader.ReadUInt32();
                mXP3File.FileInfoSize = infoDataReader.ReadUInt64();
                mXP3File.InfoSign = infoDataReader.ReadUInt32();
                mXP3File.BaseInfoSize = infoDataReader.ReadUInt64();
                mXP3File.Protect = infoDataReader.ReadUInt32();
                mXP3File.FileOriginalSize = infoDataReader.ReadUInt64();
                mXP3File.FileActuallySize = infoDataReader.ReadUInt64();
                mXP3File.FileNameLength = infoDataReader.ReadUInt16();      //读取字符串长度
                mXP3File.FileNameUTF16LE = Encoding.Unicode.GetString(infoDataReader.ReadBytes((mXP3File.FileNameLength - 1) * 2));   //读取字符串
                memInfoData.Position += 2;                  //越过UCS2(Unicode16LE)的"\0"
                mXP3File.SegmSign = infoDataReader.ReadUInt32();
                mXP3File.FileSegmSize = infoDataReader.ReadUInt64();
                mXP3File.Compress = infoDataReader.ReadUInt32();
                mXP3File.FileOffset = infoDataReader.ReadUInt64();
                mXP3File.DecompressedSize = infoDataReader.ReadUInt64();
                mXP3File.CompressedSize = infoDataReader.ReadUInt64();
                mXP3File.AdlrSign = infoDataReader.ReadUInt32();
                mXP3File.FileAdlrSize = infoDataReader.ReadUInt64();
                mXP3File.Key = infoDataReader.ReadUInt32();

                //添加到文件表数组
                xp3Files.Add(mXP3File);
            }

            Console.WriteLine("{0}封包文件表分析完成",this.mFileName);

            // 解密并导出文件
            foreach (XP3Archive.XP3File entry in xp3Files)
            {
                xp3Stream.Position = (long)entry.FileOffset;
                int fileSize = (int)entry.CompressedSize;

                byte[] fileData = ArrayPool<byte>.Shared.Rent(fileSize);

                //读取
                xp3Stream.Read(fileData, 0, fileSize);

                //解密
                this.Decrypt(fileData, fileSize);

                //判断是否压缩
                if (entry.IsCompressed)
                {
                    int decompressedSize = (int)entry.DecompressedSize;
                    byte[] decompressData = ArrayPool<byte>.Shared.Rent(decompressedSize);
                    LZ4Helper.Decompress(fileData, decompressData);
                    ArrayPool<byte>.Shared.Return(fileData, true);
                    fileSize = decompressedSize;
                    fileData = decompressData;
                }


                //合并获得文件全路径
                string extractFileFullPath = Path.Combine(this.mExtractDirectory, entry.FileNameUTF16LE);

                //检查文件夹是否存在  不存在则创建
                string directory = Path.GetDirectoryName(extractFileFullPath);
                if (directory is not null && Directory.Exists(directory) is false)
                {
                    Directory.CreateDirectory(directory);
                }

                //写入文件
                using FileStream exportFs = File.OpenWrite(extractFileFullPath);
                exportFs.Write(fileData, 0, fileSize);

                ArrayPool<byte>.Shared.Return(fileData, true);

                Console.WriteLine("{0}/{1}   提取成功", this.mFileName, entry.FileNameUTF16LE);
            }
        }



        /// <summary>
        /// 资源解密
        /// </summary>
        /// <param name="archiveData">资源数据</param>
        public byte[] Decrypt(byte[] archiveData)
        {
            return this.Decrypt(archiveData, archiveData.Length);
        }

        /// <summary>
        /// 资源解密
        /// </summary>
        /// <param name="archiveData">资源数据</param>
        /// <param name="dataLen">数据长度</param>
        public byte[] Decrypt(byte[] archiveData, int dataLen)
        {
            //获取初始key
            byte[] key = ArchiveCrypto.CreateOriginalKey16((uint)dataLen);

            //生成256解密表
            List<uint> key256 = ArchiveCrypto.CreateDecryptTable256((uint)dataLen, mRound, this.mTableKey8_1, this.mTableKey32_2);

            //解密数据
            ArchiveCrypto.DecryptArchive(archiveData, dataLen,
                                         mRound, key, key256,
                                         this.mTableKey32_3, this.mTableKey32_4,
                                         this.mTableKey32_7, this.mTableKey32_8,
                                         this.mTableKey8_1);
            return archiveData;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">文件路径</param>
        public Archive(string path)
        {
            this.mFilePath = path;
            this.mFileName = Path.GetFileNameWithoutExtension(path);
            this.mExtractDirectory = Path.Combine(Path.GetDirectoryName(path), "Extract", this.mFileName);
        }

    }
}
