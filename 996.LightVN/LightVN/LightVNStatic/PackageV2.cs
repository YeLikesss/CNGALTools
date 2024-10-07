using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace LightVNStatic
{
    public class PackageV2 : IPackage
    {
        private readonly string mCurrentDirectory;
        private readonly string mFileListRelativePath;
        private readonly ICryptoFilter? mFilter;
        
        public bool Extract()
        {
            string currentDir = this.mCurrentDirectory;
            string fileListRelativePath = this.mFileListRelativePath;
            string fileListPath = Path.Combine(currentDir, fileListRelativePath);
            string extractDir = Path.Combine(currentDir, "Extract_Static");

            ICryptoFilter? filter = this.mFilter;

            if (File.Exists(fileListPath))
            {
                byte[] listData = File.ReadAllBytes(fileListPath);

                filter?.Decrypt(listData, -1);

                using MemoryStream listDataMs = new(listData, false);
                if(JsonSerializer.Deserialize<Dictionary<string,string>>(listDataMs) is Dictionary<string, string> dict)
                {
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
                    foreach(KeyValuePair<string, string> filePathPair in dict)
                    {
                        string inFile = Path.Combine(currentDir, filePathPair.Value);
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

                            Console.WriteLine("提取成功:{0}", filePathPair.Key);
                        }
                        else
                        {
                            Console.WriteLine("文件不存在:{0}", filePathPair.Value);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("文件列表反序列化失败:{0}", fileListRelativePath);
                    return false;
                }
                return true;
            }
            else
            {
                Console.WriteLine("文件列表不存在:{0}", fileListRelativePath);
                return false;
            }
        }

        /// <summary>
        /// 封包构造函数
        /// </summary>
        /// <param name="gameDirectory">游戏当前目录</param>
        /// <param name="fileListRelativePath">文件列表(相对于游戏当前目录路径)</param>
        /// <param name="filter">解密器</param>
        public PackageV2(string gameDirectory, string fileListRelativePath, ICryptoFilter? filter)
        {
            this.mCurrentDirectory = gameDirectory;
            this.mFileListRelativePath = fileListRelativePath;
            this.mFilter = filter;
        }
    }
}
