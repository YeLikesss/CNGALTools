using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace OurshowStatic
{
    /// <summary>
    /// AGP封包
    /// </summary>
    public class AGPArchiveV1
    {
        /// <summary>
        /// 文件表
        /// </summary>
        public class FileEntry
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string Name { get; init; } = string.Empty;
            /// <summary>
            /// 文件偏移
            /// </summary>
            public uint Offset { get; init; }
            /// <summary>
            /// 文件大小
            /// </summary>
            public uint FileSize { get; init; }
            /// <summary>
            /// 实际大小
            /// </summary>
            public uint ActualSize { get; init; }

            /// <summary>
            /// 是否压缩
            /// </summary>
            public bool IsCompress { get; init; }
        }

        private string mPath = string.Empty;
        private string mName = string.Empty;

        private readonly List<FileEntry> mEntries = new();
        private uint mBaseOffset;

        private bool mIsValid = false;

        /// <summary>
        /// 封包绝对路径
        /// </summary>
        public string FullPath => this.mPath;
        /// <summary>
        /// 封包名称
        /// </summary>
        public string Name => this.mName;
        /// <summary>
        /// 封包是否有效
        /// </summary>
        public bool IsValid => this.mIsValid;


        /// <summary>
        /// 尝试解析封包
        /// </summary>
        /// <param name="path">封包路径</param>
        /// <returns>True解析成功 False解析失败</returns>
        public bool TryParse(string path)
        {
            this.Clear();
            this.mPath = path;
            this.mName = Path.GetFileNameWithoutExtension(path);

            if (!File.Exists(path))
            {
                return false;
            }

            using FileStream inFs = File.OpenRead(path);
            using BinaryReader inBr = new(inFs);

            if(inBr.ReadUInt32() != 0x31504741u)
            {
                return false;
            }

            this.mBaseOffset = inBr.ReadUInt32();

            List<FileEntry> entries = this.mEntries;

            List<(uint, string)> dirEntries = new();
            {
                uint count = inBr.ReadUInt32();
                dirEntries.Add((count, "root"));
            }

            for(int i = 0; i < dirEntries.Count; ++i)
            {
                (uint, string) dirEntry = dirEntries[i];
                uint count = dirEntry.Item1;
                string rootName = dirEntry.Item2;

                for(uint j = 0u; j < count; ++j)
                {
                    uint fileSize = inBr.ReadUInt32();
                    uint actualSize = inBr.ReadUInt32();
                    uint offset = inBr.ReadUInt32();
                    bool isDirectory = inBr.ReadBoolean();
                    bool isCompress = inBr.ReadBoolean();

                    inFs.Position += 2L;

                    int strLen = inBr.ReadInt32();

                    inFs.Position += 4L;

                    string name = Encoding.UTF8.GetString(inBr.ReadBytes(strLen));

                    //跳过 \0
                    inFs.Position += 1L;

                    string fileName = Path.Combine(rootName, name);

                    if (isDirectory)
                    {
                        //文件夹
                        dirEntries.Add((fileSize, fileName));
                    }
                    else
                    {
                        entries.Add(new()
                        {
                            Name = fileName,
                            Offset = offset,
                            FileSize = fileSize,
                            ActualSize = actualSize,
                            IsCompress = isCompress,
                        });
                    }
                }
            }

            this.mIsValid = true;

            return true;
        }

        /// <summary>
        /// 解包
        /// </summary>
        /// <returns>True解包成功 False解包失败</returns>
        public bool Extract()
        {
            if (!this.mIsValid)
            {
                return false;
            }

            string pkgPath = this.mPath;
            if (!File.Exists(pkgPath))
            {
                return false;
            }

            string extractDir = Path.Combine(Path.GetDirectoryName(pkgPath)!, "Static_Extract", this.mName);

            using FileStream inFs = File.OpenRead(pkgPath);
            foreach(FileEntry entry in this.mEntries)
            {
                inFs.Position = this.mBaseOffset + entry.Offset;

                byte[] orgData = new byte[entry.FileSize];
                inFs.Read(orgData);

                string extractPath = Path.Combine(extractDir, entry.Name);
                {
                    if(Path.GetDirectoryName(extractPath) is string dir && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                if (entry.IsCompress)
                {
                    byte[] decData = CompressLZ77.Decompress(orgData, 0u, entry.FileSize, entry.ActualSize);
                    File.WriteAllBytes(extractPath, decData);
                }
                else
                {
                    File.WriteAllBytes(extractPath, orgData);
                }
            }
            return true;
        }

        /// <summary>
        /// 清空
        /// </summary>
        private void Clear()
        {
            this.mPath = string.Empty;
            this.mName = string.Empty;

            this.mEntries.Clear();
            this.mBaseOffset = 0u;

            this.mIsValid = false;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AGPArchiveV1()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">封包路径</param>
        public AGPArchiveV1(string path)
        {
            this.TryParse(path);
        }
    }
}