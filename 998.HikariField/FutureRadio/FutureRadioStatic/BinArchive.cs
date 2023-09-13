using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
using System.Buffers;

namespace FutureRadioStatic
{
    /// <summary>
    /// 文件表
    /// </summary>
    public class FileEntry
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName;
        /// <summary>
        /// 文件偏移
        /// </summary>
        public long Offset;
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size;
    }

    /// <summary>
    /// 文件封包
    /// </summary>
    public class BinArchive
    {
        private Stream mStream;
        private List<FileEntry> mFileEntries;

        /// <summary>
        /// 封包名称
        /// </summary>
        public string ArchiveName { get; private set; }
        /// <summary>
        /// 原始流释放已释放
        /// </summary>
        public bool IsDispose => this.mStream is null;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="packageName">封包名</param>
        /// <param name="stream">封包流</param>
        public BinArchive(string packageName, Stream stream)
        {
            stream.Position = 0;
            this.mStream = stream;
            this.ArchiveName = packageName;
        }

        /// <summary>
        /// 检查是否合法
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckVaild()
        {
            if (this.IsDispose)
            {
                return false;
            }

            unsafe
            {
                uint header = 0;
                this.mStream.Read(new Span<byte>(&header, sizeof(uint)));
                return header == 0x40674461;
            }
        }

        /// <summary>
        /// 解析文件表
        /// </summary>
        public virtual void ParseFileEntry()
        {
            if (this.IsDispose)
            {
                return;
            }

            Stream stream = this.mStream;
            stream.Position = 4;

            List<FileEntry> fileEntries = new(256);
            using BinaryReader br = new(stream, Encoding.Unicode, true);

            stream.Position += 4;

            //循环读取文件名与文件偏移
            {
                string fileName = br.ReadString();
                while (!string.IsNullOrEmpty(fileName))
                {
                    long offset = br.ReadUInt32();
                    FileEntry fileEntry = new()
                    {
                        FileName = fileName,
                        Offset = offset,
                        Size = 0
                    };
                    fileEntries.Add(fileEntry);

                    fileName = br.ReadString();
                }
            }

            //获取最后一项大小
            {
                FileEntry fileEntry = fileEntries.Last();
                fileEntry.Size = stream.Length - fileEntry.Offset;
            }

            //获取前面项的文件大小
            for(int i = 0; i < fileEntries.Count - 1; ++i)
            {
                FileEntry thisEntry = fileEntries[i];
                FileEntry nextEntry = fileEntries[i + 1];
                thisEntry.Size = nextEntry.Offset - thisEntry.Offset;
            }

            this.mFileEntries = fileEntries;
        }


        /// <summary>
        /// 提取资源
        /// </summary>
        /// <param name="outputDirectory">导出文件夹</param>
        public void Extract(string outputDirectory)
        {
            if (this.IsDispose)
            {
                return;
            }

            int bufferLength = 1024 * 1024 * 16;    //初始16M缓存大小

            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferLength);

            Stream stream = this.mStream;
            foreach(FileEntry fileEntry in this.mFileEntries)
            {
                string outputPath = Path.Combine(outputDirectory, fileEntry.FileName);
                //新建文件夹
                {
                    string dir = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                long offset = fileEntry.Offset;
                long size = fileEntry.Size;

                //扩容
                if (size > bufferLength)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                    buffer = ArrayPool<byte>.Shared.Rent((int)size);
                    bufferLength = (int)size;
                }

                stream.Position = offset;
                int readLen = stream.Read(buffer, 0, (int)size);

                //解密
                for(int i = 0; i < readLen; ++i)
                {
                    byte key = (byte)((offset + i) * 0x9D + (long)i * 0x773);

                    buffer[i] -= key;
                }

                //图像封包
                if (Path.GetExtension(fileEntry.FileName) == ".pida")
                {
                    Console.WriteLine("Start Extract Pida ---> {0}", fileEntry.FileName);

                    string pidaArchiveName = Path.GetFileNameWithoutExtension(fileEntry.FileName);
                    PidaArchive pidaArchive = PidaArchive.CreateInstance(pidaArchiveName, new MemoryStream(buffer, 0, readLen, false));
                    pidaArchive?.Extract(Path.GetDirectoryName(outputPath));
                    pidaArchive?.Dispose();
                }

                FileStream outFs = new(outputPath, FileMode.Create, FileAccess.ReadWrite);
                outFs.Write(buffer, 0, readLen);
                outFs.Flush();
                outFs.Dispose();

                Console.WriteLine("Extract Success ---> {0}", fileEntry.FileName);
            }
            ArrayPool<byte>.Shared.Return(buffer);
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
        /// 创建文件封包对象
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <returns></returns>
        public static BinArchive CreateInstance(string filePath)
        {
            if (File.Exists(filePath))
            {
                BinArchive archive = new(Path.GetFileNameWithoutExtension(filePath), File.OpenRead(filePath));
                if (archive.CheckVaild())
                {
                    archive.ParseFileEntry();
                    return archive;
                }
                else
                {
                    archive.Dispose();
                }
            }
            return null;
        }
    }
}