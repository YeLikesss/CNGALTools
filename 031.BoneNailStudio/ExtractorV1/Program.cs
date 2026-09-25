using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using BoneNailStatic.Package.V1;

namespace ExtractorV1
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<TitleBase> titles = new()
            {
                new ThePiperOfDawn(),
            };

            Console.WriteLine("========= 请选择游戏 ========");
            for (int i = 0; i < titles.Count; ++i)
            {
                Console.WriteLine($"{i}: {titles[i].Name}");
            }

            string? sel = Console.ReadLine();
            if (!string.IsNullOrEmpty(sel) && int.TryParse(sel, out int idx))
            {
                using OpenFileDialog ofd = new()
                {
                    AddExtension = true,
                    AutoUpgradeEnabled = true,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    DefaultExt = ".bytes",
                    Filter = "bytes索引(*.bytes)|*.bytes|所有文件(*.*)|*.*",
                    Multiselect = false,
                    RestoreDirectory = true,
                    ShowHelp = false,
                    Title = "骨钉工作室 - 请选择Manifest描述",
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string path = ofd.FileName;
                    byte[] manifestBytes = File.ReadAllBytes(path);

                    YooManifest? manifest = Serializer.DeserializeManifest(manifestBytes);
                    if (manifest is not null)
                    {
                        TitleBase title = titles[idx];
                        manifest.Extract(Path.GetDirectoryName(path)!, new Crypto(title));
                    }
                    else
                    {
                        Console.WriteLine($"错误的Manifest文件: {path}");
                    }
                }
            }

            Console.WriteLine("========== 请按任意键退出 ==========");
            Console.Read();
        }
    }
}