using System;
using System.IO;
using System.Windows.Forms;
using Extractor.LgPack;

namespace Extractor
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
                DefaultExt = ".lgpak",
                Filter = "lgpak封包(*.lgpak)|*.lgpak|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "Lets Gal V1 - 选择封包",
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] files = ofd.FileNames;
                foreach (string f in files)
                {
                    Package? pak = Package.Open(f);
                    if(pak is not null)
                    {
                        string dir = Path.GetDirectoryName(f)!;
                        pak.Extract(dir);
                    }
                }
            }
            Console.WriteLine("==========请按任意键退出==========");
            Console.Read();
        }
    }
}
