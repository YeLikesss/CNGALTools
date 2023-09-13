using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FutureRadioStatic
{
    /// <summary>
    /// 图像信息
    /// </summary>
    public class ImageInformation
    {
        /// <summary>
        /// 宽度
        /// </summary>
        public ushort Width;
        /// <summary>
        /// 高度
        /// </summary>
        public ushort Height;
        /// <summary>
        /// X偏移
        /// </summary>
        public short OffsetX;
        /// <summary>
        /// Y偏移
        /// </summary>
        public short OffsetY;
    }

    /// <summary>
    /// 图像表
    /// </summary>
    public class ImageEntry
    {
        public FileEntry Entry;
        public ImageInformation Information;
    }

    /// <summary>
    /// 图像封包
    /// </summary>
    public class PidaArchive
    {
        private Stream mStream;
        private List<ImageEntry> mImageEntries;
        private long mImageDataOffset;

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
        public PidaArchive(string packageName, Stream stream)
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
                return header == 0x6DF22373;
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

            List<ImageEntry> imageEntries = new(256);
            using BinaryReader br = new(stream, Encoding.Unicode, true);

            stream.Position += 4;

            //循环读取文件名与文件偏移和图像信息
            {
                string fileName = br.ReadString();
                while (!string.IsNullOrEmpty(fileName))
                {
                    ImageEntry imageEntry = new()
                    {
                        Entry = new(),
                        Information = new(),
                    };
                    imageEntry.Entry.FileName = fileName + ".png";
                    imageEntry.Entry.Offset = br.ReadUInt32();
                    imageEntry.Entry.Size = 0;

                    imageEntry.Information.Width = br.ReadUInt16();
                    imageEntry.Information.Height = br.ReadUInt16();
                    imageEntry.Information.OffsetX = br.ReadInt16();
                    imageEntry.Information.OffsetY = br.ReadInt16();

                    imageEntries.Add(imageEntry);

                    fileName = br.ReadString();
                }
            }

            if (imageEntries.Count > 0)
            {
                //获取最后一项大小
                {
                    ImageEntry imageEntry = imageEntries.Last();
                    imageEntry.Entry.Size = stream.Length - imageEntry.Entry.Offset - stream.Position;
                }

                //获取图像起始位置
                this.mImageDataOffset = stream.Position;

                //获取前面项的文件大小
                for (int i = 0; i < imageEntries.Count - 1; ++i)
                {
                    ImageEntry thisEntry = imageEntries[i];
                    ImageEntry nextEntry = imageEntries[i + 1];
                    thisEntry.Entry.Size = nextEntry.Entry.Offset - thisEntry.Entry.Offset;
                }
            }
            this.mImageEntries = imageEntries;
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

            int bufferLength = 1024 * 1024 * 4;    //初始4M缓存大小

            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferLength);

            Stream stream = this.mStream;
            foreach (ImageEntry imageEntry in this.mImageEntries)
            {
                string outputPath = Path.Combine(outputDirectory, this.ArchiveName, imageEntry.Entry.FileName);
                //新建文件夹
                {
                    string dir = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                long offset = imageEntry.Entry.Offset + this.mImageDataOffset;
                long size = imageEntry.Entry.Size;

                //扩容
                if (size > bufferLength)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                    buffer = ArrayPool<byte>.Shared.Rent((int)size);
                    bufferLength = (int)size;
                }

                stream.Position = offset;
                int readLen = stream.Read(buffer, 0, (int)size);

                FileStream outFs = new(outputPath, FileMode.Create, FileAccess.ReadWrite);
                outFs.Write(buffer, 0, readLen);
                outFs.Flush();
                outFs.Dispose();

                Console.WriteLine("Extract Success ---> {0}", Path.Combine(this.ArchiveName, imageEntry.Entry.FileName));
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
        /// <param name="packageName">封包名</param>
        /// <param name="stream">封包流</param>
        /// <returns></returns>
        public static PidaArchive CreateInstance(string packageName, Stream stream)
        {
            PidaArchive archive = new(packageName, stream);
            if (archive.CheckVaild())
            {
                archive.ParseFileEntry();
                return archive;
            }
            else
            {
                archive.Dispose();
            }
            return null;
        }

    }
}
