using System;
using System.Collections.Generic;
using System.IO;

namespace NekoNovelStatic
{
    /// <summary>
    /// NekoPackage封包
    /// </summary>
    public class NekoPackage
    {
        /// <summary>
        /// 文件结构
        /// </summary>
        private class FileEntry
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string Name = string.Empty;
            /// <summary>
            /// 文件偏移
            /// </summary>
            public uint Offset;
            /// <summary>
            /// 实际大小
            /// </summary>
            public uint ActualSize;
            /// <summary>
            /// 文件大小
            /// </summary>
            public uint FileSize;
        }


        private string mSignature = string.Empty;       //封包标识
        private readonly List<FileEntry> mEntries = new();       //文件表

        private readonly string mPackagePath;    //封包全路径
        private readonly string mPackageName;    //封包名
        private bool mIsVaild = false;           //封包合法性

        /// <summary>
        /// 封包标记
        /// </summary>
        public string Signature => this.mSignature;

        /// <summary>
        /// 获取封包全路径
        /// </summary>
        public string PackagePath => this.mPackagePath;
        /// <summary>
        /// 获取封包名称
        /// </summary>
        public string PackageName => this.mPackageName;
        /// <summary>
        /// 获取封包合法性
        /// </summary>
        public bool IsVaild => this.mIsVaild;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">封包路径</param>
        public NekoPackage(string filePath)
        {
            this.mPackagePath = filePath;
            this.mPackageName = Path.GetFileNameWithoutExtension(filePath);
            this.Initialize(filePath);
        }

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="directory">输出目录</param>
        public void Extract(string directory)
        {
            if (!this.mIsVaild)
            {
                return;
            }

            string outputDirectory = Path.Combine(directory, this.mPackageName);
            string packagePath = this.mPackagePath;

            if (File.Exists(packagePath))
            {
                using FileStream fs = File.OpenRead(packagePath);

                List<FileEntry> entries = this.mEntries;

                for(int i = 0; i < entries.Count; ++i)
                {
                    FileEntry entry = entries[i];

                    string outputPath = Path.Combine(outputDirectory, entry.Name);

                    {
                        if(Path.GetDirectoryName(outputPath) is string dir && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }

                    byte[] orgData = new byte[entry.FileSize];

                    fs.Seek(entry.Offset, SeekOrigin.Begin);
                    fs.Read(orgData);

                    byte[] actualData = Zlib.Decompress(orgData, 0, orgData.Length);

                    using FileStream outFs = File.Create(outputPath);
                    outFs.Write(actualData);

                    Console.WriteLine("成功:{0}", outputPath[(directory.Length + 1)..]);
                }
            }
        }

        /// <summary>
        /// 初始化封包
        /// </summary>
        /// <param name="filePath">封包路径</param>
        private void Initialize(string filePath)
        {
            if (File.Exists(filePath))
            {
                using FileStream fs = File.OpenRead(filePath);
                using BinaryReader br = new(fs);

                if(StreamExtend.ReadUTF8String(fs) == "NKNOEVL PACKAGE")
                {
                    fs.Seek(-4L, SeekOrigin.End);

                    uint entryOffset = br.ReadUInt32();
                    entryOffset = ~entryOffset;

                    fs.Seek(entryOffset, SeekOrigin.Begin);

                    this.mSignature = StreamExtend.ReadUTF8String(fs);

                    List<FileEntry> entries = this.mEntries;
                    int count = br.ReadInt32();
                    entries.Capacity = count;
                    for(int i = 0; i < count; ++i)
                    {
                        string name = StreamExtend.ReadUTF8String(fs);
                        uint offset = br.ReadUInt32();
                        uint actualSize = br.ReadUInt32();
                        uint fileSize = br.ReadUInt32();

                        offset = ~offset;
                        fileSize = ~fileSize;

                        FileEntry entry = new()
                        {
                            Name = name,
                            Offset = offset,
                            ActualSize = actualSize,
                            FileSize = fileSize,
                        };

                        entries.Add(entry);
                    }
                    this.mIsVaild = true;
                }
            }
        }
    }
}