using Snowing.Games;
using Snowing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace VainRiserExtractor
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //资源文件夹
            List<string> archiveSubFolder = new()
            {
                "Live2D",
                "Story",
                "Textures",
                "BGM",
                "Voices",
                "SEs"
            };

            FolderBrowserDialog fbd = new()
            {
                Description = "空梦 - 请选择游戏文件夹",
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                UseDescriptionForTitle = true
            };
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                string gameDir = fbd.SelectedPath;
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

                Console.WriteLine("\n======== 空梦 --- 提取成功 ========");
                Console.Read();
            }
        }
    }
}