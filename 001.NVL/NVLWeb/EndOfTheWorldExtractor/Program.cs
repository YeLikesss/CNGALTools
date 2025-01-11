using NVLWebStatic;
using System;
using System.IO;
using System.Windows.Forms;

namespace EndOfTheWorldExtractor
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
                DefaultExt = ".asar",
                Filter = "ASAR封包(*.asar)|*.asar|所有文件(*.*)|*.*",
                Multiselect = false,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "我和她的世界末日 - 选择封包",
            };

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName;
                string directory = Path.GetDirectoryName(path);
                string extractDirectory = Path.Combine(directory, "Extract_Static");

                using ASARPackage package = ASARPackage.CreateInstance(path);
                package.Extract(extractDirectory);

                EndOfTheWorld fix = new(extractDirectory);
                fix.DecodeAsset();

                Console.WriteLine("===== 我和她的世界末日 --- 提取成功 =====");
            }
        }
    }
}