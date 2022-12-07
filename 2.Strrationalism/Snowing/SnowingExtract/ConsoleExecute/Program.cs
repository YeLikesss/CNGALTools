using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Snowing;
using Snowing.Games;

namespace ConsoleExecute
{
    class Program
    {
        static void Main(string[] args)
        {
            //资源文件夹
            List<string> archiveSubFolder = new List<string>()
            {
                "Live2D",
                "Story",
                "Textures",
                "BGM",
                "Voices",
                "SEs"
            };

            /************拖拽游戏exe到程序上运行*************/

            //获取控制台exe启动参数
            string[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length < 2)
            {
                return;
            }

            List<string> gamePathList = arguments.ToList();
            //移除自身路径
            gamePathList.RemoveAt(0);

            //获取游戏exe目录
            FileInfo gamePathInfo = new FileInfo(gamePathList[0]);
            //获取游戏文件夹
            string gameDir = string.Concat(gamePathInfo.DirectoryName, "/");

            //获取资源文件夹
            List<DirectoryInfo> archiveDirs = archiveSubFolder.ConvertAll(subFolder =>
                new DirectoryInfo(string.Concat(gameDir, subFolder))
            );

            //设置资源文件解密key与导出路径
            ArchiveFile archiveFile = new ArchiveFile();
            archiveFile.Aes128Key = VainRiser.Aes128Key;
            archiveFile.Aes128IV = VainRiser.Aes128IV;
            archiveFile.ExtractOutputDir = string.Concat(gameDir, "Extract/");

            //循环解密
            archiveDirs.ForEach(archiveDir =>
            {
                archiveFile.Extract(string.Empty, archiveDir);
            });

            Console.WriteLine("\n========请按任意键退出程序========");
            Console.ReadKey();
        }
    }
}
