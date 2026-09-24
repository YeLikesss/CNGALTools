using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SakiStatic
{
    /// <summary>
    /// Saki索引块V1
    /// </summary>
    public class SakiEntryChunkV1
    {
        /// <summary>
        /// 版本
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;
        /// <summary>
        /// 打包时间
        /// </summary>
        [JsonPropertyName("created_at")]
        public string CreateTime { get; set; } = string.Empty;
        /// <summary>
        /// 文件个数
        /// </summary>
        [JsonPropertyName("file_count")]
        public uint FileCount { get; set; }
        /// <summary>
        /// 文件表
        /// </summary>
        [JsonPropertyName("entries")]
        public List<SakiFileEntryV1> Entries { get; set; } = new();
    }

    /// <summary>
    /// Saki文件表V1
    /// </summary>
    public class SakiFileEntryV1
    {
        /// <summary>
        /// 相对路径
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;
        /// <summary>
        /// 文件偏移
        /// </summary>
        [JsonPropertyName("offset")]
        public long Offset { get; set; }
        /// <summary>
        /// 文件长度
        /// </summary>
        [JsonPropertyName("length")]
        public uint Length { get; set; }
        /// <summary>
        /// 是否文本
        /// </summary>
        [JsonPropertyName("text")]
        public bool IsText { get; set; }
        /// <summary>
        /// SHA256哈希
        /// </summary>
        [JsonPropertyName("sha256")]
        public string Hash { get; set; } = string.Empty;
    }

    /// <summary>
    /// Saki封包V1
    /// </summary>
    public class SakiPackageV1
    {
        private const uint CSignature = 0x494B4153u;

        private readonly string mPackageName;
        private readonly string mPackagePath;
        private readonly SakiEntryChunkV1 mEntryChunk;

        /// <summary>
        /// 封包名字
        /// </summary>
        public string PackageName => this.mPackageName;
        /// <summary>
        /// 封包路径
        /// </summary>
        public string PackagePath => this.mPackagePath;

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="outdirectory">输出文件夹全路径</param>
        public void Extract(string outdirectory)
        {
            string outdir = Path.Combine(outdirectory, "Static_Extract", this.mPackageName);

            using FileStream inFs = File.OpenRead(this.mPackagePath);
            foreach(SakiFileEntryV1 entry in this.mEntryChunk.Entries)
            {
                string relativePath = entry.Path;

                string outPath = Path.Combine(outdir, relativePath);
                {
                    string dir = Path.GetDirectoryName(outPath)!;
                    if(!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                byte[] data = new byte[entry.Length];
                inFs.Position = entry.Offset;
                inFs.Read(data);

                using FileStream outFs = File.Create(outPath);
                outFs.Write(data);
                outFs.Flush();

                Console.WriteLine($"[提取成功] {relativePath}");
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">封包路径</param>
        /// <param name="filechunk">封包文件表</param>
        private SakiPackageV1(string path, SakiEntryChunkV1 filechunk)
        {
            this.mPackageName = Path.GetFileNameWithoutExtension(path);
            this.mPackagePath = path;
            this.mEntryChunk = filechunk;
        }

        /// <summary>
        /// 打开封包
        /// </summary>
        /// <param name="path">封包全路径</param>
        /// <returns>打开成功:返回对象 打开失败:返回null</returns>
        public static SakiPackageV1? Open(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            if (!File.Exists(path))
            {
                return null;
            }

            using FileStream stream = File.OpenRead(path);
            using BinaryReader br = new(stream);

            uint sign = br.ReadUInt32();
            uint ver = BinaryPrimitives.ReverseEndianness(br.ReadUInt32());
            long idxOfs = BinaryPrimitives.ReverseEndianness(br.ReadInt64());
            uint idxLen = BinaryPrimitives.ReverseEndianness(br.ReadUInt32());
            if(sign != SakiPackageV1.CSignature || ver != 1u)
            {
                return null;
            }

            byte[] idx = new byte[idxLen];
            stream.Position = idxOfs;
            if (stream.Read(idx) != idx.Length)
            {
                return null;
            }
            if (JsonSerializer.Deserialize<SakiEntryChunkV1>(idx) is not SakiEntryChunkV1 fileChunk)
            {
                return null;
            }

            return new(path, fileChunk);
        }
    }
}
