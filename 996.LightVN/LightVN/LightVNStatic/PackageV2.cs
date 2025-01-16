using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace LightVNStatic
{
    public class PackageV2
    {
        private readonly CryptoFilterV2? mFilter;
        private readonly IGameInfoV2 mGameInfo;
        
        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="directory">游戏目录</param>
        public bool Extract(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                return false;
            }

            if (!Directory.Exists(directory))
            {
                return false;
            }

            string extractDir = Path.Combine(directory, "Extract_Static");

            IGameInfoV2 gameInfo = this.mGameInfo;
            CryptoFilterV2? filter = this.mFilter;

            if (!string.IsNullOrEmpty(gameInfo.FileListFileName))
            {
                //含文件表
                string fileListPath = Path.Combine(directory, gameInfo.FileListFileName);
                if (File.Exists(fileListPath))
                {
                    byte[] listData = File.ReadAllBytes(fileListPath);
                    filter?.Decrypt(listData, -1);

                    using MemoryStream listDataMs = new(listData, false);
                    if (JsonSerializer.Deserialize<Dictionary<string, string>>(listDataMs) is Dictionary<string, string> dict)
                    {
                        //导出文件表
                        {
                            string outFile = Path.Combine(extractDir, "list.json");
                            {
                                if (Path.GetDirectoryName(outFile) is string dir && !Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                            }
                            listDataMs.Position = 0L;

                            using FileStream outFs = File.Create(outFile);
                            listDataMs.CopyTo(outFs);
                            outFs.Flush();
                        }

                        //解密文件
                        foreach (KeyValuePair<string, string> filePathPair in dict)
                        {
                            string inFile = Path.Combine(directory, filePathPair.Value);
                            string outFile = Path.Combine(extractDir, filePathPair.Key);
                            if (File.Exists(inFile))
                            {
                                {
                                    if (Path.GetDirectoryName(outFile) is string dir && !Directory.Exists(dir))
                                    {
                                        Directory.CreateDirectory(dir);
                                    }
                                }

                                byte[] buf = File.ReadAllBytes(inFile);
                                filter?.Decrypt(buf, 100);

                                using FileStream outFs = File.Create(outFile);
                                outFs.Write(buf);
                                outFs.Flush();

                                Console.WriteLine("成功: {0}", filePathPair.Key);
                            }
                            else
                            {
                                Console.WriteLine("文件不存在: {0}", filePathPair.Value);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("文件表反序列化失败: {0}", fileListPath);
                        return false;
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("文件表不存在: {0}", fileListPath);
                    return false;
                }
            }
            else
            {
                string[] files = Directory.GetFiles(directory, "*.mcdat", SearchOption.AllDirectories);
                foreach(string inPath in files)
                {
                    if (File.Exists(inPath))
                    {
                        string extractPath = Path.Combine(extractDir, inPath[(directory.Length + 1)..]);
                        {
                            if (Path.GetDirectoryName(extractPath) is string dir && !Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                        }

                        byte[] data = File.ReadAllBytes(inPath);
                        filter?.Decrypt(data, 100);
                        File.WriteAllBytes(extractPath, data);

                        Console.WriteLine("成功: {0}", inPath);
                    }
                    else
                    {
                        Console.WriteLine("文件不存在:{0}", inPath);
                    }

                }
                return true;
            }
        }

        /// <summary>
        /// 封包构造函数
        /// </summary>
        /// <param name="gameInfo">游戏信息</param>
        /// <param name="filter">解密器</param>
        public PackageV2(IGameInfoV2 gameInfo, CryptoFilterV2? filter)
        {
            this.mGameInfo = gameInfo;
            this.mFilter = filter;
        }
    }
}
