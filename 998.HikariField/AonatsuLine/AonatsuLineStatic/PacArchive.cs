using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace AonatsuLineStatic
{
    public class PacArchive
    {
        /// <summary>
        /// 文件信息
        /// </summary>
        public struct FileEntry
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
            /// 文件长度
            /// </summary>
            public long Length;
        }

        /// <summary>
        /// 文件表模式
        /// </summary>
        public enum EntryMode
        {
            EightByteMode = 0,
            TenByteMode = 1
        }

        /// <summary>
        /// 封包名
        /// </summary>
        public string PackageName { get; private set; }
        /// <summary>
        /// 压缩标记
        /// </summary>
        public bool IsCompressed { get; private set; }
        /// <summary>
        /// 文件表模式
        /// </summary>
        public EntryMode FileEntryMode { get; private set; }

        private string mPackageFullPath;


        /// <summary>
        /// 提取资源
        /// </summary>
        /// <param name="extractDirectory">导出目录</param>
        /// <returns></returns>
        public bool Extract(string extractDirectory)
        {
            if (!File.Exists(this.mPackageFullPath))
            {
                return false;
            }

            extractDirectory = Path.Combine(extractDirectory, this.PackageName);

            using FileStream mFileStream = File.OpenRead(this.mPackageFullPath);
            using BinaryReader mBinaryReader = new(mFileStream);
            List<FileEntry> fileEntries = new(256);

            {
                mFileStream.Position = 16;
                Span<byte> nameBuffer = stackalloc byte[32];
                //循环读取文件表
                while (mFileStream.Position < mFileStream.Length)
                {
                    FileEntry entry = new();

                    mFileStream.Read(nameBuffer);

                    //获取文件名长度
                    int strLen = 0;
                    for (; strLen < 32; ++strLen)
                    {
                        if (nameBuffer[strLen] == 0)
                        {
                            break;
                        }
                    }

                    entry.FileName = Encoding.UTF8.GetString(nameBuffer.Slice(0, strLen));

                    if (this.FileEntryMode == EntryMode.TenByteMode)
                    {
                        //10字节保存偏移
                        entry.Offset = mBinaryReader.ReadInt64() + 20;          //文件位置位于entry后20字节处
                        mFileStream.Position += 2;
                        //10字节保存大小
                        entry.Length = mBinaryReader.ReadInt64();
                        mFileStream.Position += 2;
                    }
                    else if (this.FileEntryMode == EntryMode.EightByteMode)
                    {
                        //8字节保存偏移与大小
                        entry.Offset = mBinaryReader.ReadInt64() + 16;      //文件位置位于entry后16字节处
                        entry.Length = mBinaryReader.ReadInt64();
                    }


                    mFileStream.Seek(entry.Length, SeekOrigin.Current);

                    fileEntries.Add(entry);
                }
            }


            //读取并提取文件
            foreach(FileEntry entry in CollectionsMarshal.AsSpan(fileEntries))
            {


                mFileStream.Position = entry.Offset;
                byte[] data = new byte[entry.Length];
                mFileStream.Read(data);

                if (this.IsCompressed)
                {
                    data = QuickLZ.Decompress(data);
                }

                string extractPath = Path.Combine(extractDirectory, Until.TryDetectFileType(data, entry.FileName));

                {
                    string dir = Path.GetDirectoryName(extractPath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                File.WriteAllBytes(extractPath, data);

            }
            return true;
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="packageFullPath">封包全路径</param>
        /// <param name="entrymode">索引读取方式</param>
        /// <param name="isCompressed">压缩标记</param>
        public PacArchive(string packageFullPath, EntryMode entrymode, bool isCompressed = false)
        {
            this.mPackageFullPath = packageFullPath;
            this.PackageName = Path.GetFileNameWithoutExtension(packageFullPath);
            this.IsCompressed = isCompressed;
            this.FileEntryMode = entrymode;
        }
    }
}