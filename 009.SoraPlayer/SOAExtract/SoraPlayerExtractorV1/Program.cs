using System;
using System.Windows.Forms;
using SoraPlayerStatic;

namespace SoraPlayerExtractorV1
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
                DefaultExt = ".soa",
                Filter = "SoraPlayer封包(*.soa)|*.soa|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "SoraPlayer V1 - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (var packPath in ofd.FileNames)
                {
                    Archive archive = new(packPath);
                    archive.Extract();
                }
                Console.WriteLine("==== SoraPlayer V1 - 提取成功 ====");
                Console.Read();
            }
        }
    }
}