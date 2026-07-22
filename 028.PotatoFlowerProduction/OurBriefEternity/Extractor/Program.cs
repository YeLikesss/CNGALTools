using System;
using System.IO.Packaging;
using System.Windows.Forms;

namespace Extractor
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            byte[] aeskey = new byte[]
            {
                0x6C, 0x39, 0x2A, 0xFB, 0x08, 0x41, 0x25, 0x31, 0xC9, 0x72, 0x7C, 0x85, 0xDD, 0x1F, 0x01, 0x5D,
                0x41, 0x43, 0x51, 0x35, 0xBE, 0xBF, 0xCB, 0x3A, 0x76, 0x63, 0xAC, 0xB4, 0xC5, 0xCF, 0x69, 0x62,
            };

            using OpenFileDialog ofd = new()
            {
                AddExtension = true,
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".rpa",
                Filter = "rpa封包(*.rpa)|*.rpa|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "永恒与星辰与日常 - 选择封包",
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] files = ofd.FileNames;
                foreach (string f in files)
                {
                    RenpyRPAv2E pkg = new(f, aeskey);
                    if (pkg.Initialized)
                    {
                        pkg.Extract();
                    }
                    else
                    {
                        Console.WriteLine($"[{pkg.Name}]{pkg.LastError}");
                    }
                }
            }
            Console.WriteLine("==========请按任意键退出==========");
            Console.Read();
        }
    }
}