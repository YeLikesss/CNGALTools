using PygmaGameStatic;
using System;
using System.IO;
using System.Windows.Forms;

namespace ExtractorV1
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using FolderBrowserDialog fbd = new()
            {
                Description = "Pygma Game V1 - 请选择游戏文件夹",
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                UseDescriptionForTitle = true,
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string gameDir = fbd.SelectedPath;

                string[] pkgs = Directory.GetFiles(gameDir, "*.blend", SearchOption.AllDirectories);
                foreach(string path in pkgs)
                {
                    WJZRenpyPackageV1 packageV1 = new(path);
                    if (!packageV1.Extract(gameDir))
                    {
                        Console.WriteLine($"错误: [{Path.GetFileName(packageV1.PackagePath)}]{packageV1.LastError}");
                    }
                }
                Console.WriteLine("================ Pygma Game V1 提取完成 ====================\r\n");
            }

            Console.WriteLine("===============请按任意键退出程序==============");
            Console.Read();
        }
    }
}