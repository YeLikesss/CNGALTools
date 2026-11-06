using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Extractor.Utils;

namespace Extractor.LgPack
{
    /// <summary>
    /// 封包
    /// </summary>
    public class Package
    {
        /// <summary>
        /// 文件节
        /// </summary>
        private class FileChunk
        {
            /// <summary>
            /// 解密IV
            /// </summary>
            [JsonPropertyName("nonce")]
            public string Nonce { get; set; } = string.Empty;
            /// <summary>
            /// AES-GCM解密认证
            /// </summary>
            [JsonPropertyName("tag")]
            public string Auth { get; set; } = string.Empty;
            /// <summary>
            /// 区块总大小
            /// </summary>
            [JsonPropertyName("clen")]
            public long BlockLength { get; set; }
            /// <summary>
            /// 数据块大小
            /// </summary>
            [JsonPropertyName("plen")]
            public long DataLength { get; set; }
        }

        /// <summary>
        /// 文件表
        /// </summary>
        private class FileEntry
        {
            /// <summary>
            /// 路径Hash
            /// </summary>
            [JsonPropertyName("pathHash")]
            public string PathHash { get; set; } = string.Empty;
            /// <summary>
            /// 数据偏移
            /// </summary>
            [JsonPropertyName("offset")]
            public long Offset { get; set; }
            /// <summary>
            /// 区块总长度
            /// </summary>
            [JsonPropertyName("length")]
            public long BlockLength { get; set; }
            /// <summary>
            /// 数据总长度
            /// </summary>
            [JsonPropertyName("plainLength")]
            public long DataLength { get; set; }
            /// <summary>
            /// 文件区块大小
            /// </summary>
            [JsonPropertyName("chunkSize")]
            public long ChunkSize { get; set; }
            /// <summary>
            /// 文件区块
            /// </summary>
            [JsonPropertyName("chunks")]
            public List<FileChunk> Chunks { get; set; } = new();
        }

        private string mFilepath = string.Empty;            //文件路径
        private string mName = string.Empty;                //封包名称
        private List<FileEntry> mEntries = new();           //文件表
        private byte[] mKey = Array.Empty<byte>();          //AES Key
        private long mDataOffset;                           //数据起始点
        private bool mInitialized = false;                  //初始化标志

        private Package()
        {
        }

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="directory">输出目录</param>
        public void Extract(string directory)
        {
            if (this.mInitialized)
            {
                using FileStream inFs = File.OpenRead(this.mFilepath);
                foreach (FileEntry entry in this.mEntries)
                {
                    string relativePath = Path.Combine(this.mName, entry.PathHash);
                    string outPath = Path.Combine(directory, "Extract_Static", relativePath);
                    {
                        string dir = Path.GetDirectoryName(outPath)!;
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }
                    using FileStream outFs = File.Create(outPath);

                    inFs.Position = entry.Offset + this.mDataOffset;
                    foreach (FileChunk chunk in entry.Chunks)
                    {
                        long dataLen = chunk.DataLength;                //数据长度
                        long authLen = chunk.BlockLength - dataLen;     //AES-GCM验证密钥长度

                        byte[] encData = new byte[dataLen];
                        byte[] decData = new byte[dataLen];
                        inFs.Read(encData);

                        //AES-GCM验证密钥
                        byte[] auth;
                        if(authLen == 16L)
                        {
                            auth = new byte[16];
                            inFs.Read(auth);
                        }
                        else
                        {
                            auth = BinaryDataConvert.StringToHexBytes(chunk.Auth);
                        }

                        //AES IV
                        byte[] nonce = BinaryDataConvert.StringToHexBytes(chunk.Nonce);

                        using AesGcm aes = new(this.mKey);
                        aes.Decrypt(nonce, encData, auth, decData);
                        outFs.Write(decData);
                    }

                    outFs.Flush();

                    Console.WriteLine($"提取成功: {relativePath}");
                }
            }
        }


        /// <summary>
        /// 打开封包
        /// </summary>
        /// <param name="filepath">文件路径</param>
        public static Package? Open(string filepath)
        {
            if(string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
            {
                return null;
            }

            using FileStream stream = File.OpenRead(filepath);
            using BinaryReader br = new(stream);
            if(br.ReadUInt64() != 0x0A0D4B4150474C89ul)
            {
                return null;
            }
            br.ReadUInt32();

            byte[] salt = br.ReadBytes(16);         //用于生成key
            byte[] nonce = br.ReadBytes(12);        //AES IV
            byte[] auth = br.ReadBytes(16);         //AES-GCM验证密钥

            uint idxLen = br.ReadUInt32();
            byte[] encIdx = new byte[idxLen];
            stream.Read(encIdx);

            long hdrLen = stream.Position;          //头部大小

            byte[] key = Crypto.GenerateKey(salt);
            byte[] decIdx = new byte[encIdx.Length];

            using AesGcm aes = new(key);
            aes.Decrypt(nonce, encIdx, auth, decIdx);

            Package? pak = null;

            Utf8JsonReader jsonReader = new(decIdx);
            if (JsonDocument.TryParseValue(ref jsonReader, out JsonDocument? doc))
            {
                if (doc.RootElement.TryGetProperty("entries", out JsonElement entriesEle))
                {
                    if (entriesEle.Deserialize<List<FileEntry>>() is List<FileEntry> entries)
                    {
                        Package p = new()
                        {
                            mFilepath = filepath,
                            mName = Path.GetFileNameWithoutExtension(filepath),
                            mEntries = entries,
                            mKey = key,
                            mDataOffset = hdrLen,
                            mInitialized = true,
                        };
                        pak = p;
                    }
                }
                doc.Dispose();
            }

            return pak;
        }
    }
}
