using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueAngel.V1;
using System.IO;
using System.Threading.Tasks;

namespace BlueAngel.StarlightofAeons
{

    public class Archive
    {
        private string mFileName;
        private string mExtractDirectory;
        private FileStream mFileStream;

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
        public bool Extract()
        {
            if (this.mFileStream == null || this.mFileStream.Length <= 0)
            {
                if (SystemConfig.ConsoleLogEnable)
                {
                    Console.WriteLine("文件数据不存在");
                }
                return false;
            }

            //初始化读取器
            BinaryReader binReader = new BinaryReader(this.mFileStream);

            //读取文件表信息偏移
            this.mFileStream.Position = 0x10;
            long xp3InfoOffset = BitConverter.ToInt64(this.Decrypt(binReader.ReadBytes(8)));

            //读取文件表信息
            XP3Archive.XP3Info xp3Info = new();
            this.mFileStream.Position = xp3InfoOffset;
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
            MemoryStream memInfoData = new MemoryStream(infoData);
            BinaryReader infoDataReader = new BinaryReader(memInfoData,Encoding.Unicode);   
            memInfoData.Position = 0;

            //读取分析文件表并读取文件
            List<XP3Archive.XP3File> xp3Files = new List<XP3Archive.XP3File>();
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

                //读取文件
                this.mFileStream.Position = (long)mXP3File.FileOffset;
                mXP3File.FileData = binReader.ReadBytes((int)mXP3File.CompressedSize);
                //添加到文件表数组
                xp3Files.Add(mXP3File);
            }

            //释放资源
            infoDataReader.Close();
            infoDataReader.Dispose();
            infoDataReader = null;
            memInfoData.Close();
            memInfoData.Dispose();
            memInfoData = null;
            binReader.Close();
            binReader.Dispose();
            binReader = null;
            this.mFileStream.Close();
            this.mFileStream.Dispose();
            this.mFileStream = null;

            //打印log
            if (SystemConfig.ConsoleLogEnable)
            {
                Console.Write(string.Concat(this.mFileName, "封包"));
                Console.Write("文件表分析完成    ");
                Console.WriteLine("文件读取完成");
            }

            // 解密并导出文件
            Parallel.ForEach(xp3Files, mXP3File => 
            {
                this.Decrypt(mXP3File.FileData);        //解密文件

                byte[] buffer;
                //判断是否压缩
                if (mXP3File.IsCompressed)
                {
                    buffer = LZ4Helper.Decompress(mXP3File.FileData, (int)mXP3File.DecompressedSize);
                }
                else
                {
                    buffer = mXP3File.FileData;
                }

                //合并获得文件全路径
                string mExtractFileFullPath = string.Concat(this.mExtractDirectory, mXP3File.FileNameUTF16LE);
                //检查文件夹是否存在  不存在则创建
                if (Directory.Exists(Path.GetDirectoryName(mExtractFileFullPath)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(mExtractFileFullPath));
                }
                //写入文件
                File.WriteAllBytes(mExtractFileFullPath, buffer);
                //打印log
                if (SystemConfig.ConsoleLogEnable)
                {
                    Console.WriteLine(string.Concat(this.mFileName, "/",mXP3File.FileNameUTF16LE, "    提取成功"));
                }
            });

            return true;
        }



        /// <summary>
        /// 资源解密
        /// </summary>
        /// <param name="archiveData">资源数据</param>
        public byte[] Decrypt(byte[] archiveData)
        {
            //获取初始key
            byte[] key = ArchiveCrypto.CreateOriginalKey16((uint)archiveData.Length);

            //生成256解密表
            List<uint> key256 = ArchiveCrypto.CreateDecryptTable256((uint)archiveData.Length, mRound, this.mTableKey8_1, this.mTableKey32_2);

            //解密数据
            ArchiveCrypto.DecryptArchive(archiveData, archiveData.Length,
                                         mRound, key, key256,
                                         this.mTableKey32_3, this.mTableKey32_4,
                                         this.mTableKey32_7, this.mTableKey32_8,
                                         this.mTableKey8_1);
            return archiveData;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">文件路径</param>
        public Archive(string path)
        {
            this.mFileStream = new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read);
            this.mFileName = Path.GetFileNameWithoutExtension(path);
            this.mExtractDirectory = string.Concat(Path.GetDirectoryName(path), "/Extract/", this.mFileName, "/");
        }

    }
}
