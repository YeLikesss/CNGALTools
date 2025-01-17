using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace EngineCore
{
    /// <summary>
    /// Pac封包
    /// </summary>
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

        private readonly string mPackageName;
        private readonly string mPackageFullPath;
        private readonly EntryMode mFileEntryMode;
        private readonly bool mIsCompressed;

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

            extractDirectory = Path.Combine(extractDirectory, this.mPackageName);

            using FileStream mFileStream = File.OpenRead(this.mPackageFullPath);
            using BinaryReader mBinaryReader = new(mFileStream);
            List<FileEntry> fileEntries = new(256);

            {
                mFileStream.Position = 16L;
                Span<byte> nameBuffer = stackalloc byte[32];
                //循环读取文件表
                while (mFileStream.Position < mFileStream.Length)
                {
                    FileEntry entry = new();

                    mFileStream.Read(nameBuffer);

                    //获取文件名长度
                    int strLen = nameBuffer.IndexOf((byte)0);
                    if(strLen == -1)
                    {
                        strLen = 0;
                    }

                    entry.FileName = Encoding.UTF8.GetString(nameBuffer[..strLen]);

                    if (this.mFileEntryMode == EntryMode.TenByteMode)
                    {
                        //10字节保存偏移
                        entry.Offset = mBinaryReader.ReadInt64() + 20L;          //文件位置位于entry后20字节处
                        mFileStream.Position += 2L;
                        //10字节保存大小
                        entry.Length = mBinaryReader.ReadInt64();
                        mFileStream.Position += 2L;
                    }
                    else if (this.mFileEntryMode == EntryMode.EightByteMode)
                    {
                        //8字节保存偏移与大小
                        entry.Offset = mBinaryReader.ReadInt64() + 16L;      //文件位置位于entry后16字节处
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

                if (this.mIsCompressed)
                {
                    data = QuickLZ.Decompress(data);
                }

                string extractPath = Path.Combine(extractDirectory, Until.TryDetectFileType(data, entry.FileName));
                {
                    if (Path.GetDirectoryName(extractPath) is string dir && !Directory.Exists(dir))
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
        /// <param name="packageRelativePath">封包相对路径</param>
        /// <param name="entrymode">索引读取方式</param>
        /// <param name="isCompressed">压缩标记</param>
        public PacArchive(string packageFullPath, string packageRelativePath, EntryMode entrymode, bool isCompressed = false)
        {
            this.mPackageFullPath = packageFullPath;
            this.mPackageName = packageRelativePath;
            this.mFileEntryMode = entrymode;
            this.mIsCompressed = isCompressed;
        }
    }
}