using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using RPAArchive.Utils;

namespace RPAArchive
{
    /// <summary>
    /// RPA封包v3
    /// </summary>
    public class RenpyRPAv3 : RenpyRPA
    {
        /// <summary>
        /// 文件表
        /// </summary>
        public class FileEntry
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName { get; init; }
            /// <summary>
            /// 偏移
            /// </summary>
            public long Offset { get; init; }
            /// <summary>
            /// 长度
            /// </summary>
            public int Length { get; init; }
            /// <summary>
            /// 头字节
            /// </summary>
            public byte[] Header { get; init; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="fileName">文件名</param>
            /// <param name="offset">文件偏移</param>
            /// <param name="length">文件长度</param>
            /// <param name="header">文件头</param>
            public FileEntry(string fileName, long offset, int length, byte[] header)
            {
                this.FileName = fileName;
                this.Offset = offset;
                this.Length = length;
                this.Header = header;
            }
        }

        private readonly List<FileEntry> mFileEntries = new();

        /// <summary>
        /// 文件表
        /// </summary>
        public ReadOnlyCollection<FileEntry> Entries => this.mFileEntries.AsReadOnly();

        public override RenpyRPAVersion Version => RenpyRPAVersion.RPAv3;

        public override int Count => this.mFileEntries.Count;

        public override bool Extract(IProgress<string>? progressCallBack = null)
        {
            string pkgPath = this.mFilePath;
            string pkgName = this.mName;
            if (!File.Exists(pkgPath))
            {
                progressCallBack?.Report($"文件不存在: {pkgPath}");
                return false;
            }

            string outputDirectory = Path.GetDirectoryName(pkgPath)!;
            using FileStream inFs = File.OpenRead(pkgPath);

            //提取文件
            foreach (FileEntry entry in this.mFileEntries)
            {
                string outPath = Path.Combine(outputDirectory, "Static_Extract", pkgName, entry.FileName);
                {
                    string dir = Path.GetDirectoryName(outPath)!;
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    using FileStream outFs = File.Create(outPath);

                    if (entry.Header.Length != 0)
                    {
                        outFs.Write(entry.Header);
                    }

                    byte[] buf = new byte[entry.Length];

                    inFs.Position = entry.Offset;
                    inFs.Read(buf);

                    outFs.Write(buf);
                    outFs.Flush();

                    progressCallBack?.Report($"提取成功: {pkgName}/{entry.FileName}");
                }
            }
            return true;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">封包路径</param>
        private RenpyRPAv3(string filePath) : base(filePath)
        {
        }

        /// <summary>
        /// 创建封包
        /// </summary>
        /// <param name="path">封包路径</param>
        /// <returns>封包对象</returns>
        public static RenpyRPAv3? Create(string path)
        {
            using FileStream inFs = File.OpenRead(path);

            if (inFs.Length <= 40L)
            {
                return null;
            }

            //读取头部信息
            Span<byte> header = stackalloc byte[40];
            inFs.Read(header);

            //检查标记
            if(Encoding.UTF8.GetString(header[..8]) != "RPA-3.0 ")
            {
                return null;
            }

            //读取文件表偏移与key
            if (!long.TryParse(Encoding.UTF8.GetString(header[8..24]), NumberStyles.HexNumber, null, out long indexOffset) ||
                !int.TryParse(Encoding.UTF8.GetString(header[25..33]), NumberStyles.HexNumber, null, out int key))
            {
                return null;
            }

            //反序列化文件表
            object indexObj;
            {
                inFs.Position = indexOffset;

                byte[] compressed = new byte[inFs.Length - indexOffset];
                inFs.Read(compressed, 0, compressed.Length);

                byte[] uncompressed = Zlib.Decompress(compressed);

                indexObj = Pickle.Decode(uncompressed);
            }
            if (indexObj is not Hashtable entries)
            {
                return null;
            }

            RenpyRPAv3 rpav3 = new(path);

            //解析文件表
            List<FileEntry> fileEntries = rpav3.mFileEntries;
            fileEntries.Capacity = entries.Count;
            foreach (DictionaryEntry entry in entries)
            {
                string fileName = (entry.Key as string)!;

                if ((entry.Value as ArrayList)?[0] is not object[] infos)
                {
                    return null;
                }

                long offset = Convert.ToInt64(infos[0]);
                int length = Convert.ToInt32(infos[1]);

                offset ^= key;
                length ^= key;

                byte[] prefix = infos[2] switch
                {
                    null => Array.Empty<byte>(),
                    string s => Encoding.UTF8.GetBytes(s),
                    byte[] bytes => bytes,
                    _ => throw new InvalidDataException($"不支持的数据前缀 {infos[2].GetType().Name}"),
                };

                fileEntries.Add(new(fileName, offset, length, prefix));
            }

            return rpav3;
        }
    }
}
