using OurshowStatic;
using System;
using System.IO;
using System.Windows.Forms;

namespace OurshowExtractorV1
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
                DefaultExt = ".agp",
                Filter = "AGP封包(*.agp)|*.agp|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "Ourshow Games V1 - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AGPArchiveV1 archiveV1 = new();
                foreach(string path in ofd.FileNames)
                {
                    if (archiveV1.TryParse(path))
                    {
                        archiveV1.Extract();
                    }
                    else
                    {
                        Console.WriteLine("{0} 封包错误", archiveV1.Name);
                    }
                }

                Console.WriteLine("====== Ourshow Games V1 ======");
                Console.Read();
            }
        }
    }
}