using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace FileExtractor
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
                DefaultExt = ".pack",
                Filter = "pack封包(*.pack)|*.pack|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "贽之匣庭 - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using StreamWriter logger = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extractor.log"), false, System.Text.Encoding.Unicode);
                TextWriter orgOut = Console.Out;
                Console.SetOut(logger);
                foreach (string p in ofd.FileNames)
                {
                    ExfsPackage package = new(p);
                    package.Extract();
                }
                Console.SetOut(orgOut);

                Console.WriteLine("===== 贽之匣庭 - 提取成功 =====");
                Console.Read();
            }
        }
    }
}