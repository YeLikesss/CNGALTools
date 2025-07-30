using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using XiangSheStatic.Utils;

namespace XiangSheStatic.Crypto.V1
{
    /// <summary>
    /// 文件管理提取器
    /// </summary>
    public class FileManagerExtractor
    {
        private readonly IKeyContext mKeyContext;
        private readonly IFileManagerContext mFMContext;
        private readonly AssetExtractor mAssetExtractor;

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="encrypt">True加密 False不加密</param>
        /// <param name="key">key</param>
        /// <returns>解密后数据</returns>
        private byte[] Decrypt(byte[] data, bool encrypt, [Optional] string key)
        {
            if (encrypt)
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = this.mKeyContext.FileKey;
                }
                return Crypto.AESDecryptECB(data, key);
            }
            else
            {
                return data;
            }
        }

        /// <summary>
        /// 提取
        /// </summary>
        /// <param name="gameDirectory">游戏资源目录</param>
        public void Extract(string gameDirectory)
        {
            IFileManagerContext ctx = this.mFMContext;

            string extractDirectory = Path.Combine(gameDirectory, "Static_Extract");

            //解密全局变量配置
            {
                string relativePath = Path.Combine(ctx.FMRelativePath, ctx.FMGlobalKeyDataFileName);

                string inPath = Path.Combine(gameDirectory, relativePath);
                if (File.Exists(inPath))
                {
                    string outPath = Path.Combine(extractDirectory, relativePath);
                    {
                        string dir = Path.GetDirectoryName(outPath)!;
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }

                    byte[] data = File.ReadAllBytes(inPath);
                    data = this.Decrypt(data, true);
                    File.WriteAllBytes(outPath, data);

                    Console.WriteLine("AES-ECB解密: {0}", relativePath);
                }
            }

            //解密存档文件
            {
                string savedataPath = Path.Combine(gameDirectory, ctx.FMRelativePath, ctx.FMSaveDataDirectoryName);

                string[] paths = Directory.GetFiles(savedataPath, "*.*", SearchOption.AllDirectories);
                foreach(string inPath in paths)
                {
                    if (File.Exists(inPath))
                    {
                        string relativePath = inPath[(gameDirectory.Length + 1)..];

                        string outPath = Path.Combine(extractDirectory, relativePath);
                        {
                            string dir = Path.GetDirectoryName(outPath)!;
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                        }

                        byte[] data = File.ReadAllBytes(inPath);
                        data = this.Decrypt(data, true);
                        File.WriteAllBytes(outPath, data);

                        Console.WriteLine("AES-ECB解密: {0}", relativePath);
                    }
                }
            }

            //解密附加包
            {
                List<AssetExtractor.AssetRegions> assetRegionsList = new();

                string abPath = Path.Combine(gameDirectory, ctx.FMRelativePath, ctx.FMAssetDirectoryName);
                if (Directory.Exists(abPath))
                {
                    string[] abDirs = Directory.GetDirectories(abPath);

                    foreach(string abDir in abDirs)
                    {
                        string inPath = Path.Combine(abDir, "assetinfo.json");
                        if (File.Exists(inPath))
                        {
                            string relativePath = inPath[(gameDirectory.Length + 1)..];

                            string outPath = Path.Combine(extractDirectory, relativePath);
                            {
                                string dir = Path.GetDirectoryName(outPath)!;
                                if (!Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                            }

                            byte[] data = File.ReadAllBytes(inPath);

                            //附加包使用AssetKey解密
                            data = this.Decrypt(data, true, this.mKeyContext.AssetKey);
                            File.WriteAllBytes(outPath, data);

                            Console.WriteLine("AES-ECB解密: {0}", relativePath);

                            //反序列化分区信息
                            if (JsonSerializer.Deserialize<AssetExtractor.AssetRegions>(data) is AssetExtractor.AssetRegions assetRegions)
                            {
                                assetRegionsList.Add(assetRegions);
                            }
                        }
                    }
                }

                //提取包分区内容
                foreach (AssetExtractor.AssetRegions ar in assetRegionsList)
                {
                    this.mAssetExtractor.ExtractRegions(gameDirectory, Path.Combine(ctx.FMRelativePath, ctx.FMAssetDirectoryName), ar);
                }
            }

            //解密包资源
            {
                this.mAssetExtractor.Extract(gameDirectory);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameData">游戏数据</param>
        public FileManagerExtractor(GameDataBase gameData)
        {
            this.mKeyContext = gameData;
            this.mFMContext = gameData;
            this.mAssetExtractor = new(gameData);
        }
    }

    /// <summary>
    /// Unity包提取器
    /// </summary>
    public class AssetExtractor
    {
        /// <summary>
        /// 包文件描述
        /// </summary>
        public class AssetFile
        {
            /// <summary>
            /// 相对路径
            /// </summary>
            [JsonPropertyName("path")]
            public string Path { get; set; } = string.Empty;
            /// <summary>
            /// Hash
            /// </summary>
            [JsonPropertyName("hash")]
            public string Hash { get; set; } = string.Empty;
            /// <summary>
            /// 大小
            /// </summary>
            [JsonPropertyName("size")]
            public long Size { get; set; }
        }

        /// <summary>
        /// 包分区文件
        /// </summary>
        public class AssetRegionsFile
        {
            /// <summary>
            /// 文件名
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            /// <summary>
            /// Hash
            /// </summary>
            [JsonPropertyName("hash")]
            public string Hash { get; set; } = string.Empty;
            /// <summary>
            /// 大小
            /// </summary>
            [JsonPropertyName("size")]
            public long Size { get; set; }
            /// <summary>
            /// 引用
            /// </summary>
            [JsonPropertyName("dependencies")]
            public List<string> Dependencies { get; set; } = new();
        }

        /// <summary>
        /// 包分区
        /// </summary>
        public class AssetRegions
        {
            /// <summary>
            /// 包文件夹名称
            /// </summary>
            [JsonPropertyName("regions")]
            public string Regions { get; set; } = string.Empty;
            /// <summary>
            /// 包类型
            /// </summary>
            [JsonPropertyName("regionsType")]
            public string RegionsType { get; set; } = string.Empty;
            /// <summary>
            /// 包描述
            /// </summary>
            [JsonPropertyName("info")]
            public string Infomation { get; set; } = string.Empty;
            /// <summary>
            /// 父级文件夹
            /// </summary>
            [JsonPropertyName("parentDirectory")]
            public string ParentDirectory { get; set; } = string.Empty;
            /// <summary>
            /// 封包算法
            /// </summary>
            [JsonPropertyName("dataConverterType")]
            public AlgorithmType Algorithm { get; set; }
            /// <summary>
            /// 资源列表
            /// </summary>
            [JsonPropertyName("manifest_key")]
            public List<string> AssetKey { get; set; } = new();
            /// <summary>
            /// 文件列表
            /// </summary>
            [JsonPropertyName("manifest_value")]
            public List<AssetRegionsFile> Files { get; set; } = new();
            /// <summary>
            /// Lua
            /// </summary>
            [JsonPropertyName("iuabfodc")]
            public List<string> Lua { get; set; } = new();
        }

        private readonly IKeyContext mKeyContext;
        private readonly IAssetContext mASContext;

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="gameDirectory">游戏资源目录</param>
        /// <returns>游戏Asset文件列表</returns>
        private AssetFile[] GetFiles(string gameDirectory)
        {
            string path = Path.Combine(gameDirectory, this.mASContext.ASRelativePath, this.mASContext.ASFileListFileName);
            if (File.Exists(path))
            {
                using FileStream fs = File.OpenRead(path);
                using JsonDocument jsonDoc = JsonDocument.Parse(fs);
                JsonElement root = jsonDoc.RootElement;

                if(root.TryGetProperty("target", out JsonElement fl))
                {
                    if(fl.Deserialize<AssetFile[]>() is AssetFile[] assetFiles)
                    {
                        return assetFiles;
                    }
                }
            }
            return Array.Empty<AssetFile>();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>解密后数据</returns>
        private byte[] Decrypt(byte[] data)
        {
            return Crypto.AESDecryptECB(data, this.mKeyContext.AssetKey);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>解密后数据</returns>
        private byte[] DecryptWithSalt(byte[] data)
        {
            return Crypto.AESDecryptCFB(data, this.mKeyContext.AssetKey, this.mKeyContext.AssetSalt);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据</param>
        private void DecryptXOR(byte[] data)
        {
            Crypto.XORDecrypt(data, this.mKeyContext.AssetXORKey);
        }

        /// <summary>
        /// 提取分区包
        /// </summary>
        /// <param name="gameDirectory">游戏资源目录</param>
        /// <param name="assetRelativePath">资源相对路径</param>
        /// <param name="assetRegions">分区对象</param>
        public void ExtractRegions(string gameDirectory, string assetRelativePath, AssetRegions assetRegions)
        {
            string extractDirectory = Path.Combine(gameDirectory, "Static_Extract");

            foreach(AssetRegionsFile file in assetRegions.Files)
            {
                string relativePath = Path.Combine(assetRelativePath, assetRegions.Regions, file.Name);

                string inPath = Path.Combine(gameDirectory, relativePath);
                if (File.Exists(inPath))
                {
                    string outPath = Path.Combine(extractDirectory, relativePath);
                    {
                        string dir = Path.GetDirectoryName(outPath)!;
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }

                    byte[] data = File.ReadAllBytes(inPath);

                    if (file.Name == file.Hash)
                    {
                        //视频文件
                        File.WriteAllBytes(outPath, data);
                        Console.WriteLine("视频文件: {0}", relativePath);
                    }
                    else
                    {
                        switch (assetRegions.Algorithm)
                        {
                            default:
                            case AlgorithmType.None:
                            {
                                File.WriteAllBytes(outPath, data);
                                Console.WriteLine("无加密: {0}", relativePath);
                                break;
                            }
                            case AlgorithmType.AESECB:
                            {
                                data = this.Decrypt(data);

                                File.WriteAllBytes(outPath, data);
                                Console.WriteLine("AES-ECB解密: {0}", relativePath);
                                break;
                            }
                            case AlgorithmType.AESCFB:
                            {
                                data = this.DecryptWithSalt(data);

                                File.WriteAllBytes(outPath, data);
                                Console.WriteLine("AES-CFB解密: {0}", relativePath);
                                break;
                            }
                            case AlgorithmType.GZip:
                            {
                                data = GZip.Decompress(data);

                                File.WriteAllBytes(outPath, data);
                                Console.WriteLine("GZip解压: {0}", relativePath);
                                break;
                            }
                            case AlgorithmType.XOR:
                            {
                                this.DecryptXOR(data);

                                File.WriteAllBytes(outPath, data);
                                Console.WriteLine("XOR解密: {0}", relativePath);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 提取
        /// </summary>
        /// <param name="gameDirectory">游戏资源目录</param>
        public void Extract(string gameDirectory)
        {
            IAssetContext ctx = this.mASContext;

            string extractDirectory = Path.Combine(gameDirectory, "Static_Extract");

            List<AssetRegions> assetRegionsList = new();
            //提取包分区信息
            {
                AssetFile[] assetFiles = this.GetFiles(gameDirectory);
                foreach (AssetFile files in assetFiles)
                {
                    if (files.Path.EndsWith("assetinfo.json"))
                    {
                        string relativePath = Path.Combine(ctx.ASRelativePath, files.Path);

                        string inPath = Path.Combine(gameDirectory, relativePath);
                        if (File.Exists(inPath))
                        {
                            string outPath = Path.Combine(extractDirectory, relativePath);
                            {
                                string dir = Path.GetDirectoryName(outPath)!;
                                if (!Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                            }

                            byte[] data = File.ReadAllBytes(inPath);
                            data = this.Decrypt(data);
                            File.WriteAllBytes(outPath, data);

                            Console.WriteLine("AES-ECB解密: {0}", relativePath);

                            //反序列化分区信息
                            if(JsonSerializer.Deserialize<AssetRegions>(data) is AssetRegions assetRegions)
                            {
                                assetRegionsList.Add(assetRegions);
                            }
                        }
                    }
                }
            }

            //提取包分区内容
            foreach(AssetRegions ar in assetRegionsList)
            {
                this.ExtractRegions(gameDirectory, ctx.ASRelativePath, ar);
            }
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameData">游戏数据</param>
        public AssetExtractor(GameDataBase gameData)
        {
            this.mKeyContext = gameData;
            this.mASContext = gameData;
        }
    }

    /// <summary>
    /// 资源提取器
    /// </summary>
    public class ResourceExtractor
    {
        private readonly GameDataBase mGameData;        //游戏数据

        /// <summary>
        /// 提取
        /// </summary>
        /// <param name="gameDirectory">游戏资源目录</param>
        public void Extract(string gameDirectory)
        {
            if(!string.IsNullOrEmpty(gameDirectory) && Directory.Exists(gameDirectory))
            {
                FileManagerExtractor fe = new(this.mGameData);
                fe.Extract(gameDirectory);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gameData">游戏数据</param>
        public ResourceExtractor(GameDataBase gameData)
        {
            this.mGameData = gameData;
        }
    }
}
