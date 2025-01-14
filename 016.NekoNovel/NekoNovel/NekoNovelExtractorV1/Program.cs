using NekoNovelStatic;
using System;
using System.IO;
using System.Windows.Forms;

namespace NekoNovelExtractorV1
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using OpenFileDialog ofd = new()
            {
                AddExtension = true,
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".nkpack",
                Filter = "nkpack封包(*.nkpack)|*.nkpack|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "NekoNovel V1 - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string path in ofd.FileNames)
                {
                    string outputDirectory = Path.Combine(Path.GetDirectoryName(path)!, "Static_Extract");
                    NekoPackage package = new(path);
                    if (package.IsVaild)
                    {
                        package.Extract(outputDirectory);
                    }
                    else
                    {
                        Console.WriteLine("错误的封包:{0}", package.PackageName);
                    }
                }

                Console.WriteLine("===== NekoNovel V1 - 提取成功 =====");
                Console.Read();
            }
        }
    }
}