using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NVLWebStatic
{
    /// <summary>
    /// 我和他的世界末日
    /// </summary>
    public class EndOfTheWorld
    {
        /// <summary>
        /// 游戏文件夹
        /// </summary>
        public string GameFolderPath { get; } = "data\\games\\Teow";
        /// <summary>
        /// 资源文件夹
        /// </summary>
        public string AssetFolderPath { get; } = "data\\assets";
        /// <summary>
        /// 资源表文件名
        /// </summary>
        public string AssetListName { get; } = "assets.json";

        /// <summary>
        /// AES Key
        /// </summary>
        public byte[] AESKey { get; } = new byte[]
        {
            142, 134, 122, 174, 139, 75, 85, 236, 1, 134, 58, 225, 136, 147, 59, 127,
        };

        /// <summary>
        /// AES IV
        /// </summary>
        public byte[] AESIV { get; } = new byte[]
        {
            196, 132, 205, 125, 176, 20, 171, 182, 209, 64, 82, 130, 168, 238, 166, 236,
        };

        /// <summary>
        /// 游戏资源目录
        /// </summary>
        public string CurrentDirectory { get; private set; }

        /// <summary>
        /// 解码游戏资源
        /// </summary>
        public void DecodeAsset()
        {
            string assetListPath = Path.Combine(this.CurrentDirectory, this.GameFolderPath, this.AssetListName);
            if (File.Exists(assetListPath))
            {
                using FileStream assetListFs = File.OpenRead(assetListPath);
                Dictionary<string, string> assetList = JsonSerializer.Deserialize<Dictionary<string, string>>(assetListFs);

                int bufferLen = 1024 * 1024 * 4;        //4M缓存
                byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferLen);

                foreach(KeyValuePair<string, string> fileMap in assetList)
                {
                    string assetFilePath = Path.Combine(this.CurrentDirectory, this.AssetFolderPath, fileMap.Value);
                    string assetOutPath= Path.Combine(this.CurrentDirectory, this.GameFolderPath, fileMap.Key);
                    if (File.Exists(assetFilePath))
                    {
                        {
                            string dir = Path.GetDirectoryName(assetOutPath);
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                        }

                        //图像资源
                        if (string.IsNullOrEmpty(Path.GetExtension(assetFilePath)))
                        {
                            using FileStream assetInFs = File.OpenRead(assetFilePath);
                            using FileStream assetOutFs = new(assetOutPath, FileMode.Create, FileAccess.ReadWrite);

                            long fileLen = assetInFs.Length;

                            //扩容
                            if (fileLen > bufferLen)
                            {
                                ArrayPool<byte>.Shared.Return(buffer);
                                bufferLen = (int)fileLen;
                                buffer = ArrayPool<byte>.Shared.Rent(bufferLen);
                            }

                            int readLen = assetInFs.Read(buffer, 0, (int)fileLen);

                            //解密
                            Crypto.AES128CFB128Decrypt(buffer, readLen, this.AESKey, this.AESIV);

                            assetOutFs.Write(buffer, 0, readLen);
                            assetOutFs.Flush();
                        }
                        else
                        {
                            //其他资源
                            File.Copy(assetFilePath, assetOutPath, true);
                        }
                    }
                }

                ArrayPool<byte>.Shared.Return(buffer);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentDirectory">游戏资源目录</param>
        public EndOfTheWorld(string currentDirectory)
        {
            this.CurrentDirectory = currentDirectory;
        }

    }
}
