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

            if(Path.GetDirectoryName(arguments[1]) is string gameDir)
            {
                //设置资源文件解密key与导出路径
                ArchiveFile archiveFile = new()
                {
                    Aes128Key = VainRiser.Aes128Key,
                    Aes128IV = VainRiser.Aes128IV,
                    ExtractOutputDir = Path.Combine(gameDir, "Extract")
                };

                //循环解密
                archiveSubFolder.ForEach(folder =>
                {
                    archiveFile.Extract(string.Empty, new(Path.Combine(gameDir, folder)));
                });

            }

            Console.WriteLine("\n========请按任意键退出程序========");
            Console.ReadKey();
        }
    }
}
