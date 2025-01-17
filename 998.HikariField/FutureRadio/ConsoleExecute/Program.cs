using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FutureRadioStatic;

namespace ConsoleExecute
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
                DefaultExt = ".bin",
                Filter = "bin封包(*.bin)|*.bin|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "未来ラジオと人工鳩 [官中Steam版] - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach(string path in ofd.FileNames)
                {
                    string extractDirectory = Path.Combine(Path.GetDirectoryName(path), "Static_Extract");
                    using BinArchive? binArchive = BinArchive.CreateInstance(path);
                    binArchive?.Extract(extractDirectory);
                }

                Console.WriteLine("==== 未来ラジオと人工鳩 [官中Steam版] - 解包成功 ====");
                Console.Read();
            }
        }
    }
}