using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HamidashiCreativeStatic;

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
                DefaultExt = ".pfs",
                Filter = "pfs封包(*.pfs)|*.pfs|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "Hamidashi Creative [官中Steam版] - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach(string path in ofd.FileNames)
                {
                    SWArtemisArchive? archive = SWArtemisArchive.CreateInstance(path);
                    archive?.Extract();
                }
                Console.WriteLine("\n\n===== Hamidashi Creative [官中Steam版] - 提取成功 =====\n\n");
                Console.Read();
            }
        }
    }
}