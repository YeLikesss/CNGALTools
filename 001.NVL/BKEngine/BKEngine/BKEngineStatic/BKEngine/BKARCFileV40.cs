using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BKEngine
{
    public class BKARCFileV40 : BKARCFileBase
    {
        /// <summary>
        /// 文件头
        /// </summary>
        public class FileHeaderV40
        {
            /// <summary>
            /// 文件头标记
            /// </summary>
            public ulong Signature;
            /// <summary>
            /// 解密Key1
            /// </summary>
            public uint Key1;
            /// <summary>
            /// 解密Key2
            /// </summary>
            public uint Key2;
            /// <summary>
            /// 解密Key3
            /// </summary>
            public uint Key3;
            /// <summary>
            /// 文件头结束全0
            /// </summary>
            public uint Ender;
            /// <summary>
            /// 获取文件头标记是否合法
            /// </summary>
            public bool IsVaild => (this.Signature & 0x0000FFFFFFFFFFFF) == 0x0000044352414B42;

            /// <summary>
            /// 文件表头偏移
            /// </summary>
            /// <returns></returns>
            public virtual uint FileEntryHeaderOffset()
            {
                return this.Transform(this.Key3) + 0x10;
            }

            /// <summary>
            /// 读取文件表头
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            public virtual FileEntryHeaderV40 ReadFileEntry(Stream s)
            {
                using BinaryReader br = new(s, Encoding.Default, true);
                FileEntryHeaderV40 entryHeader = new()
                {
                    CompressedSize = this.Transform(br.ReadUInt32()),
                    UncompressedSize = this.Transform(br.ReadUInt32()),
                    EntryKey = br.ReadUInt32(),
                    Ender = br.ReadUInt32()
                };
                return entryHeader;
            }

            /// <summary>
            /// 变换
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            protected virtual uint Transform(uint value)
            {
                value ^= this.Key1 & 0x00FFFF00;
                value -= 0x6C078965;
                value ^= this.Key2;
                value -= 0x5E89F12A;
                value ^= this.Key1;
                return value;
            }

            /// <summary>
            /// 结构大小
            /// </summary>
            public static int StructSize => 24;
        }

        /// <summary>
        /// 文件表头
        /// </summary>
        public class FileEntryHeaderV40
        {
            /// <summary>
            /// 压缩文件长度
            /// </summary>
            public uint CompressedSize;
            /// <summary>
            /// 解压缩文件长度
            /// </summary>
            public uint UncompressedSize;
            /// <summary>
            /// 解密文件表压缩包Key
            /// </summary>
            public uint EntryKey;
            /// <summary>
            /// 文件表头结束全0
            /// </summary>
            public uint Ender;

            /// <summary>
            /// 结构大小
            /// </summary>
            public static int StructSize => 16;
        }

        /// <summary>
        /// 文件表前缀
        /// </summary>
        public class EntryPrefixV40
        {
            public uint Unknow1;
            /// <summary>
            /// 子文件/文件夹数量
            /// </summary>
            public int SubCount;
            public uint Unknow3;
            /// <summary>
            /// 结束符0
            /// </summary>
            public uint Ender;
        }

        /// <summary>
        /// 文件数据类型
        /// </summary>
        public enum FileType : uint
        {
            /// <summary>
            /// 一般资源
            /// </summary>
            NormalArchive = 0,
            /// <summary>
            /// 文件夹
            /// </summary>
            Directory = 1,
            /// <summary>
            /// 压缩资源
            /// </summary>
            CompressedArchive = 2
        }



        /// <summary>
        /// 文件夹信息
        /// </summary>
        public class DirectoryEntryV40
        {
            /// <summary>
            /// 文件夹名称
            /// </summary>
            public string DirectoryName;
            /// <summary>
            /// 目录下子文件夹
            /// </summary>
            public List<DirectoryEntryV40> Diretories = new();
            /// <summary>
            /// 目录下子文件
            /// </summary>
            public List<FileEntryV40> Files = new();

        }

        /// <summary>
        /// 文件信息
        /// </summary>
        public class FileEntryV40
        {
            /// <summary>
            /// 文件大小
            /// </summary>
            public uint FileSize;
            /// <summary>
            /// 文件偏移
            /// </summary>
            public uint FileOffset;
            /// <summary>
            /// 文件Key(普通资源可用)
            /// </summary>
            public uint Key;
            /// <summary>
            /// 解压后长度(压缩资源可用)
            /// </summary>
            public uint ArchiveSize;
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName;

            /// <summary>
            /// 压缩标记
            /// </summary>
            public bool Compressed;
        }

        /// <summary>
        /// 文件表
        /// </summary>
        public DirectoryEntryV40 Entries { get; protected set; }

        public override bool CheckHeader()
        {
            if (this.IsDispose)
            {
                return false;
            }

            ulong header = 0;
            unsafe
            {
                this.mStream.Read(new(&header, sizeof(ulong)));
                return (header & 0x0000FFFFFFFFFFFF) == 0x0000044352414B42;
            }
        }

        public override void Extract(string outputDirectory)
        {
            if (this.IsDispose)
            {
                return;
            }
            this.ExtractInternal(Path.Combine(outputDirectory, this.PackageName), this.Entries);
        }

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="outputDirectory">导出文件夹</param>
        /// <param name="directoryEntry">文件夹表</param>
        protected virtual void ExtractInternal(string outputDirectory, DirectoryEntryV40 directoryEntry)
        {
            foreach(var dirEntry in directoryEntry.Diretories)
            {
                this.ExtractInternal(Path.Combine(outputDirectory, dirEntry.DirectoryName), dirEntry);
            }

            int bufferLength = 1024 * 1024 * 16;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferLength);  //初始16M缓存

            Stream stream = this.mStream; 

            foreach (var fileEntry in directoryEntry.Files)
            {
                string outPath = Path.Combine(outputDirectory, fileEntry.FileName);
                //新建文件夹
                {
                    string dir = Path.GetDirectoryName(outPath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                uint offset = fileEntry.FileOffset;
                uint size = fileEntry.FileSize;

                //自动扩容
                if (bufferLength < size + 4)
                {
                    bufferLength = (int)(size + 4);
                    ArrayPool<byte>.Shared.Return(buffer);
                    buffer = ArrayPool<byte>.Shared.Rent(bufferLength);
                }

                using FileStream outFs = new(outPath, FileMode.Create, FileAccess.ReadWrite);

                stream.Position = offset;

                //压缩
                if (fileEntry.Compressed)
                {
                    //0x28,0xB5,0x2F,0xFD
                    buffer[0] = 0x28;
                    buffer[1] = 0xB5;
                    buffer[2] = 0x2F;
                    buffer[3] = 0xFD;

                    int readLen = stream.Read(buffer, 4, (int)size);

                    using Stream decompressStream = ZstdHelper.CreateDecompressStream(new MemoryStream(buffer, 0, readLen + 4, false));
                    decompressStream.CopyTo(outFs);
                }
                else
                {
                    //不压缩
                    int readLen = stream.Read(buffer, 0, (int)size);
                    this.DecryptFile(buffer, fileEntry.Key);
                    outFs.Write(buffer, 0, readLen);
                }
                outFs.Flush();
            }
            ArrayPool<byte>.Shared.Return(buffer);
        }

        public override void ParseEntry()
        {
            if (this.IsDispose)
            {
                return;
            }

            Stream stream = this.mStream;
            stream.Position = 0;

            //读取文件头信息
            FileHeaderV40 fileHeader = this.ReadFileHeader(stream);

            //读取文件表头信息
            stream.Position = fileHeader.FileEntryHeaderOffset();
            FileEntryHeaderV40 entryHeader = fileHeader.ReadFileEntry(stream);

            //读取文件表
            int entrySize = (int)entryHeader.CompressedSize;
            byte[] entryBuffer = ArrayPool<byte>.Shared.Rent(entrySize);
            int readLen = stream.Read(entryBuffer, 0, entrySize);

            //解密文件表
            this.DecryptEntry(entryBuffer, (uint)readLen, entryHeader.EntryKey);

            using Stream entryStream = ZstdHelper.CreateDecompressStream(new MemoryStream(entryBuffer, 0, readLen, false));

            //反序列化文件表
            this.Entries = this.EntryDeserializer(entryStream);

            ArrayPool<byte>.Shared.Return(entryBuffer);
        }

        /// <summary>
        /// 获取文件头
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected virtual FileHeaderV40 ReadFileHeader(Stream s)
        {
            using BinaryReader br = new(s, Encoding.Default, true);
            return new()
            {
                Signature = br.ReadUInt64(),
                Key1 = br.ReadUInt32(),
                Key2 = br.ReadUInt32(),
                Key3 = br.ReadUInt32(),
                Ender = br.ReadUInt32(),
            };
        }

        /// <summary>
        /// 解密文件表
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">长度</param>
        /// <param name="key">表key</param>
        /// <returns></returns>
        protected virtual uint DecryptEntry(byte[] data, uint length, uint key)
        {
            for(uint index = 0; index < length; ++index)
            {
                uint value = data[index];
                key ^= value;
                value *= length - index;
                value += 0x5D588B65;
                key += value;
                data[index] ^= (byte)key;
            }
            return key;
        }

        /// <summary>
        /// 反序列化文件表
        /// </summary>
        /// <param name="entryStream"></param>
        protected virtual DirectoryEntryV40 EntryDeserializer(Stream entryStream)
        {
            using BinaryReader br = new(entryStream, Encoding.Default, true);
            EntryPrefixV40 prefix = new()
            {
                Unknow1 = br.ReadUInt32(),
                SubCount = br.ReadInt32(),
                Unknow3 = br.ReadUInt32(),
                Ender = br.ReadUInt32(),
            };

            DirectoryEntryV40 directoryEntry = new()
            {
                DirectoryName = string.Empty,
            };

            for (int i = 0; i < prefix.SubCount; ++i)
            {
                //读取类型
                FileType type = (FileType)br.ReadUInt32();

                //文件夹
                if (type == FileType.Directory)
                {
                    directoryEntry.Diretories.Add(this.ReadDirectory(entryStream));
                }
                else
                {
                    //文件
                    directoryEntry.Files.Add(this.ReadFileEntry(entryStream, type));
                }
            }
            return directoryEntry;
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

        /// <summary>
        /// 读取文件夹
        /// </summary>
        /// <param name="entryStream"></param>
        /// <returns></returns>
        protected virtual DirectoryEntryV40 ReadDirectory(Stream entryStream)
        {
            using BinaryReader br = new(entryStream, Encoding.Default, true);

            int subCount = br.ReadInt32();        //子目录 子文件数量
            _ = br.ReadUInt32();
            _ = br.ReadUInt32();
            string dirName = this.EntryReadFileName(entryStream);

            //对齐
            this.EntryAlignment(entryStream);

            DirectoryEntryV40 directoryEntry = new()
            {
                DirectoryName = dirName
            };

            for (int i = 0; i < subCount; ++i)
            {
                //读取类型
                FileType type = (FileType)br.ReadUInt32();

                //文件夹
                if (type == FileType.Directory)
                {
                    directoryEntry.Diretories.Add(this.ReadDirectory(entryStream));
                }
                else
                {
                    directoryEntry.Files.Add(this.ReadFileEntry(entryStream, type));
                }
            }
            return directoryEntry;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="entryStream"></param>
        /// <param name="type">文件类型</param>
        /// <returns></returns>
        protected virtual FileEntryV40 ReadFileEntry(Stream entryStream, FileType type)
        {
            using BinaryReader br = new(entryStream, Encoding.Default, true);

            uint fileSize = br.ReadUInt32();
            uint fileOffset = br.ReadUInt32();
            uint fileKeyOrDecompressLength = br.ReadUInt32();
            string fileName = this.EntryReadFileName(entryStream);

            FileEntryV40 fileEntry = new()
            {
                FileSize = fileSize,
                FileOffset = fileOffset,
                FileName = fileName,
            };

            //普通文件
            if(type == FileType.NormalArchive)
            {
                fileEntry.ArchiveSize = fileEntry.FileSize;
                fileEntry.Key = fileKeyOrDecompressLength;
                fileEntry.Compressed = false;
            }
            else if(type == FileType.CompressedArchive)
            {
                //压缩文件
                fileEntry.ArchiveSize = fileKeyOrDecompressLength;
                fileEntry.Key = 0;
                fileEntry.Compressed = true;
            }

            //对齐
            this.EntryAlignment(entryStream);

            return fileEntry;
        }

        /// <summary>
        /// 文件表对齐 
        /// </summary>
        /// <param name="entryStream"></param>
        protected virtual void EntryAlignment(Stream entryStream)
        {
            //8字节对齐
            long pos = entryStream.Position;
            pos = ((pos - 1) & -8) + 8;
            entryStream.Position = pos;
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">key</param>
        protected virtual void DecryptFile(byte[] data, uint key, uint archiveOffset = 0)
        {
            byte xorKey = (byte)((key ^ 0xDF) + 0x17);
            uint xorLength = (((key ^ 0x000000EA) & 0x000001FF) + 0x00000200) & 0xFFFFFFF8;

            if (archiveOffset < xorLength)
            {
                xorLength -= archiveOffset;

                xorLength = Math.Min(xorLength, (uint)data.Length);

                //解密
                for(uint index = 0; index < xorLength; ++index)
                {
                    data[index] ^= xorKey;
                }
            }
        }
        
    }
}
