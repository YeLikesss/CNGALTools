using PygmaGameStatic;
using System.IO;
using System.Windows.Forms;
using System;

namespace ExtractorV2
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using FolderBrowserDialog fbd = new()
            {
                Description = "Pygma Game V2 - 请选择游戏文件夹",
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                UseDescriptionForTitle = true,
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string gameDir = fbd.SelectedPath;

                string[] pkgs = Directory.GetFiles(gameDir, "*.dll", SearchOption.AllDirectories);
                foreach (string path in pkgs)
                {
                    HLXRenpyPackageV2 packageV2 = new(path);
                    if (!packageV2.Extract(gameDir))
                    {
                        Console.WriteLine($"错误: [{Path.GetFileName(packageV2.PackagePath)}]{packageV2.LastError}");
                    }
                }
                Console.WriteLine("================ Pygma Game V2 提取完成 ====================\r\n");
            }

            Console.WriteLine("===============请按任意键退出程序==============");
            Console.Read();
        }
    }
}