using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections;

namespace PygmaGameStatic
{
    /// <summary>
    /// WJZRenpy封包V1
    /// </summary>
    public class WJZRenpyPackageV1
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

        private readonly string mPackagePath;
        private string mLastError = string.Empty;

        /// <summary>
        /// 封包路径
        /// </summary>
        public string PackagePath => this.mPackagePath;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string LastError => this.mLastError;

        /// <summary>
        /// 提取
        /// </summary>
        /// <param name="outputDirectory">输出路径</param>
        /// <returns>True提取成功 False提取失败</returns>
        public bool Extract(string outputDirectory)
        {
            this.mLastError = string.Empty;

            string pkgPath = this.mPackagePath;

            if(Path.GetExtension(pkgPath) != ".blend")
            {
                this.mLastError = "错误的扩展名";
                return false;
            }

            if (!File.Exists(pkgPath))
            {
                this.mLastError = "封包不存在";
                return false;
            }

            string pkgName = Path.GetFileNameWithoutExtension(pkgPath);

            using FileStream inFs = File.OpenRead(pkgPath);

            Span<byte> header = stackalloc byte[40];

            if (inFs.Read(header) != header.Length)
            {
                this.mLastError = "封包文件长度错误";
                return false;
            }
            if (Encoding.UTF8.GetString(header[..8]) != "WJZ-4.9 ")
            {
                this.mLastError = "文件标记不一致";
                return false;
            }

            if (!long.TryParse(Encoding.UTF8.GetString(header[8..24]), NumberStyles.HexNumber, null, out long indexOffset) ||
                !int.TryParse(Encoding.UTF8.GetString(header[25..33]), NumberStyles.HexNumber, null, out int key))
            {
                this.mLastError = "文件格式不正确";
                return false;
            }

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
                this.mLastError = "Pickle文件表反序列化错误";
                return false;
            }

            //解析文件表
            List<FileEntry> fileEntries = new(entries.Count);
            foreach(DictionaryEntry entry in entries)
            {
                string fileName = (entry.Key as string)!;

                if ((entry.Value as ArrayList)?[0] is not object[] infos)
                {
                    this.mLastError = "文件表格式错误";
                    return false;
                }

                //与标准Renpy封包相反
                //标准renpy offset = [0] length = [1]

                long offset = Convert.ToInt64(infos[1]);
                int length = Convert.ToInt32(infos[0]);

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

            //提取文件
            foreach(FileEntry entry in fileEntries)
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

                    Console.WriteLine($"提取成功: {pkgName}/{entry.FileName}");
                }
            }

            return true;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">封包绝对路径</param>
        public WJZRenpyPackageV1(string filePath)
        {
            this.mPackagePath = filePath;
        }
    }
}
