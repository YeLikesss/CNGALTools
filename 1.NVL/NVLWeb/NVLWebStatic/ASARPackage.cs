using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

namespace NVLWebStatic
{
    /// <summary>
    /// 二进制型索引
    /// </summary>
    public class BinaryEntry
    {
        /// <summary>
        /// 对齐大小
        /// </summary>
        public int AlignmentSize { get; private set; }
        /// <summary>
        /// 索引大小
        /// </summary>
        public int EntrySize { get; private set; }
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// 获取文本型JSON
        /// </summary>
        /// <returns></returns>
        public string GetJSONText()
        {
            return Encoding.UTF8.GetString(this.Data);
        }

        /// <summary>
        /// 获取UTF8流JSON
        /// </summary>
        /// <returns></returns>
        public Stream CreateJSONUTF8Stream()
        {
            return new MemoryStream(this.Data, false);
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns></returns>
        public unsafe static BinaryEntry CreateInstance(Stream s, int entryLength)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(entryLength);

            s.Read(buffer, 0, entryLength);

            //解密
            Crypto.Decrypt(buffer.AsSpan().Slice(0, entryLength), new(&entryLength, sizeof(int)));

            //读取对齐大小与json长度
            BinaryEntry binaryEntry = new()
            {
                AlignmentSize = BitConverter.ToInt32(buffer, 0),
                EntrySize = BitConverter.ToInt32(buffer, 4),
            };
            binaryEntry.Data = new byte[binaryEntry.EntrySize];

            //json文件表
            Array.Copy(buffer, 8, binaryEntry.Data, 0, binaryEntry.EntrySize);

            ArrayPool<byte>.Shared.Return(buffer);

            return binaryEntry;
        }
    }

    /// <summary>
    /// ASAR封包
    /// </summary>
    public class ASARPackage : IDisposable
    {
        /// <summary>
        /// 封包头
        /// </summary>
        public class ASARHeader
        {
            /// <summary>
            /// 标记
            /// </summary>
            public uint Magic;
            /// <summary>
            /// Entry长度
            /// </summary>
            public int EntryLength;

            /// <summary>
            /// 头大小
            /// </summary>
            public const int Size = 8;
        }

        private Stream mStream = null;

        /// <summary>
        /// 封包头
        /// </summary>
        public ASARHeader Header { get; private set; }

        /// <summary>
        /// 文件表(二进制)
        /// </summary>
        public BinaryEntry Entry { get; private set; }

        /// <summary>
        /// 文件表
        /// </summary>
        public List<FileEntry> FileEntries { get; private set; }

        /// <summary>
        /// 封包名
        /// </summary>
        public string PackageName { get; private set; }

        public bool IsDisposed => this.mStream is null;

        /// <summary>
        /// 提取到目标文件夹
        /// </summary>
        /// <param name="outDirectory"></param>
        public void Extract(string outDirectory)
        {
            if (!this.IsDisposed)
            {
                long dataOffset = this.Header.EntryLength + ASARHeader.Size;

                int bufferLen = 1024 * 1024 * 4;        //4M缓存
                byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferLen);

                //存放key
                Span<byte> key = stackalloc byte[0x2C];

                Stream stream = this.mStream;
                foreach (FileEntry entry in this.FileEntries)
                {
                    string path = Path.Combine(outDirectory, entry.FilePath);
                    {
                        string dir = Path.GetDirectoryName(path);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }

                    //配置key
                    {
                        Encoding.UTF8.GetBytes(entry.Hash, key);
                        BitConverter.TryWriteBytes(key.Slice(0x28, 4), entry.Size);
                    }

                    long offset = entry.Offset + dataOffset;
                    int size = (int)entry.Size;

                    //扩容
                    if (size > bufferLen)
                    {
                        ArrayPool<byte>.Shared.Return(buffer);
                        bufferLen = size;
                        buffer = ArrayPool<byte>.Shared.Rent(bufferLen);
                    }

                    //读取
                    int readLen = stream.Read(buffer, 0, size);
                    //解密
                    Crypto.Decrypt(buffer.AsSpan()[0..readLen], key, size);

                    using FileStream outFS = new(path, FileMode.Create, FileAccess.ReadWrite);
                    outFS.Write(buffer, 0, readLen);
                    outFS.Flush();
                }
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }


        /// <summary>
        /// 解析封包
        /// </summary>
        private void Parse()
        {
            Stream stream = this.mStream;
            stream.Position = 0;

            using BinaryReader br = new(stream, Encoding.Default, true);

            //读取头
            ASARHeader header = new()
            {
                Magic = br.ReadUInt32(),
                EntryLength = br.ReadInt32()
            };
            this.Header = header;
            //读取文件表
            this.Entry = BinaryEntry.CreateInstance(stream, header.EntryLength);
            this.ParseEntry();
        }

        /// <summary>
        /// 解析文件表
        /// </summary>
        private void ParseEntry()
        {
            using Stream jsonStream = this.Entry.CreateJSONUTF8Stream();
            this.FileEntries = FileEntry.ParseEntry(jsonStream);
        }

        public void Dispose()
        {
            this.mStream?.Dispose();
            this.mStream = null;
        }

        /// <summary>
        /// 创建封包对象
        /// </summary>
        /// <param name="packagePath"></param>
        /// <returns></returns>
        public static ASARPackage CreateInstance(string packagePath)
        {
            if (File.Exists(packagePath))
            {
                ASARPackage package = new()
                {
                    mStream = File.OpenRead(packagePath),
                    PackageName = Path.GetFileNameWithoutExtension(packagePath)
                };
                package.Parse();
                return package;
            }
            else
            {
                return null;
            }
        }

    }
}
