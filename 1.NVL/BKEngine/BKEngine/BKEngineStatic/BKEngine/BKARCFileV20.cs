using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BKEngine
{
    /// <summary>
    /// V20版封包
    /// </summary>
    public class BKARCFileV20 : BKARCFileBase
    {
        /// <summary>
        /// 文件表
        /// </summary>
        public class FileEntryV20
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName;
            /// <summary>
            /// 文件偏移
            /// </summary>
            public uint FileOffset;
            /// <summary>
            /// 文件大小
            /// </summary>
            public uint FileSize;
            /// <summary>
            /// 资源大小
            /// </summary>
            public uint ArchiveSize;
            /// <summary>
            /// 文件类型
            /// </summary>
            public uint FileType;

            /// <summary>
            /// 获取压缩标志
            /// </summary>
            public bool Compressed => this.FileType == 1;
        }

        public override bool CheckHeader()
        {
            if (this.IsDispose)
            {
                return false;
            }

            ulong header = 0;
            unsafe
            {
                this.mStream.Read(new(&header, 6));      //读取6字节
                return header == 0x0000024352414B42;
            }
        }

        public override void ParseEntry()
        {
            if (this.IsDispose)
            {
                return;
            }

            Stream stream = this.mStream;
            using BinaryReader br = new(stream, Encoding.Default, true);

            stream.Position = stream.Length - 12;       //文件表头信息位于封包最后12字节

            uint entrySize = br.ReadUInt32();       //文件表大小
            uint fileCount = br.ReadUInt32();       //文件个数
            uint entryKey = br.ReadUInt32();        //文件表key

            byte[] entryRawData = ArrayPool<byte>.Shared.Rent((int)entrySize);

            //设置文件表位置并读取
            stream.Position = stream.Length - 12 - entrySize;   
            stream.Read(entryRawData, 0, (int)entrySize);

            //解密文件表
            this.FileKey = this.DecryptEntry(entryRawData, entrySize, entryKey);       

            //创建文件表流(Bz2压缩)
            using Stream entryStream = BZip2Helper.CreateDecompressStream(new MemoryStream(entryRawData, 0, (int)entrySize, false));
            
            //解析文件表
            this.FileEntries = this.EntryDeserializer(entryStream, (int)fileCount);

            //释放
            ArrayPool<byte>.Shared.Return(entryRawData);
        }

        public override void Extract(string outputDirectory)
        {
            if (this.IsDispose)
            {
                return;
            }

            int bufferLength = 1024 * 1024 * 16;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferLength);  //初始16M缓存

            Stream stream = this.mStream;

            //提取资源
            foreach(FileEntryV20 entry in this.FileEntries)
            {
                string outPath = Path.Combine(outputDirectory, this.PackageName, entry.FileName);

                //创建文件夹
                {
                    string dir = Path.GetDirectoryName(outPath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                uint offset = entry.FileOffset;     //文件偏移
                uint size = entry.FileSize;         //文件大小

                //自动扩容
                if (bufferLength < size + 2)
                {
                    bufferLength = (int)(size + 2);
                    ArrayPool<byte>.Shared.Return(buffer);
                    buffer = ArrayPool<byte>.Shared.Rent(bufferLength);
                }

                using FileStream outFs = new(outPath, FileMode.Create, FileAccess.ReadWrite);

                stream.Position = offset;
                //压缩
                if (entry.Compressed)
                {
                    buffer[0] = 0x42;
                    buffer[1] = 0x5A;
                    int readLen = stream.Read(buffer, 2, (int)size);

                    using Stream decompressStream = BZip2Helper.CreateDecompressStream(new MemoryStream(buffer, 0, readLen + 2, false));
                    decompressStream.CopyTo(outFs);
                }
                else
                {
                    //不压缩
                    int readLen = stream.Read(buffer, 0, (int)size);
                    this.DecryptFile(buffer, this.FileKey, size, offset);
                    outFs.Write(buffer, 0, readLen);
                }
                outFs.Flush();
            }
            ArrayPool<byte>.Shared.Return(buffer);
        }

        /// <summary>
        /// 文件解密key
        /// </summary>
        public uint FileKey { get; protected set; }

        /// <summary>
        /// 文件表
        /// </summary>
        public List<FileEntryV20> FileEntries { get; protected set; }

        /// <summary>
        /// 解密文件表
        /// </summary>
        /// <param name="rawData">数据</param>
        /// <param name="length">长度</param>
        /// <param name="key"></param>
        protected virtual uint DecryptEntry(byte[] rawData, uint length, uint key)
        {
            uint index = 0;                    //数据索引
            uint lastLength = length;           //当前剩余长度
            uint temp = key;                   //临时变量
            while (index < length)
            {
                uint data = rawData[index];

                //计算解密listkey
                temp ^= data;
                temp += 0x5D588B65;
                temp += (data * lastLength);

                rawData[index] = (byte)(data ^ temp);   //解密完成
                index++;                    //索引自增
                lastLength--;              //当前剩余长度自减
            }
            return temp;        //返回文件key
        }

        /// <summary>
        /// 解密文件内容
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">解密key</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="fileOffset">文件偏移</param>
        /// <param name="archiveOffset">资源偏移</param>
        protected virtual void DecryptFile(byte[] data, uint key, uint fileSize, uint fileOffset, uint archiveOffset = 0)
        {
            uint decryptLength;                       //需要解密的数据长度
            decryptLength = ((uint)((int)key % 0x00000023) + 2 * fileOffset + 3 * fileSize) % 0x000001E8 + 0x000000EC;  //解密需要解密的文件长度
            if (archiveOffset < decryptLength)
            {
                decryptLength -= archiveOffset;       //计算实际需要解密长度

                decryptLength = Math.Min(decryptLength, fileSize);  //实际解密长度和硬盘文件长度取最小

                //开始解密
                byte k = (byte)((7 * fileOffset + (uint)((int)key % 0x0000001B) + 5 * fileSize) % 0x000000F1 + 0x0000000B);
                for (uint index = 0; index < decryptLength; ++index)
                {
                    data[index] ^= k;
                }
            }
        }

        /// <summary>
        /// 文件表反序列化
        /// </summary>
        protected virtual List<FileEntryV20> EntryDeserializer(Stream entryStream, int fileCount)
        {
            List<FileEntryV20> fileEntries = new(fileCount);

            using BinaryReader br = new(entryStream, Encoding.Default, true);
            for (int index = 0; index < fileCount; ++index)
            {
                FileEntryV20 entry = new()
                {
                    FileName = this.EntryReadFileName(entryStream),
                    FileOffset = br.ReadUInt32(),
                    ArchiveSize = br.ReadUInt32(),
                    FileType = br.ReadUInt32(),
                };

                //资源已压缩
                if (entry.Compressed)
                {
                    entry.FileSize = br.ReadUInt32();
                }
                else
                {
                    entry.FileSize = entry.ArchiveSize;
                }

                fileEntries.Add(entry);
            }
            return fileEntries;
        }

        /// <summary>
        /// 读取文件名
        /// </summary>
        /// <param name="entryStream"></param>
        /// <returns></returns>
        protected virtual string EntryReadFileName(Stream entryStream)
        {
            long start = entryStream.Position;

            //扫描字符串\0
            while (entryStream.ReadByte() != 0) 
            {
                if (entryStream.Length == entryStream.Position)
                {
                    throw new IOException("流已到达末端");
                }
            }

            long end = entryStream.Position;
            int strLen = (int)(end - start - 1);
            byte[] buffer = ArrayPool<byte>.Shared.Rent(strLen);

            //读取字符串
            entryStream.Position = start;
            entryStream.Read(buffer, 0, strLen);
            entryStream.Position = end;

            //UTF8编码字符串
            string fileName = Encoding.UTF8.GetString(buffer, 0, strLen);

            ArrayPool<byte>.Shared.Return(buffer);

            return fileName;
        }

    }
}
