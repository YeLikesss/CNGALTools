using System;
using System.IO;
using System.Windows.Forms;
using EngineCoreStatic;

namespace SeparateHeartsExtractorV1
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
                DefaultExt = ".hac",
                Filter = "HAC封包(*.hac)|*.hac|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "SeparateHearts V1 - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                {
                    string outDir = Path.Combine(Path.GetDirectoryName(file)!, "Static_Extract");
                    using HACPackage pkg = new(file);
                    if (pkg.IsVaild)
                    {
                        pkg.Extract(outDir);
                    }
                }

                Console.WriteLine("===== SeparateHearts V1 - 提取完成 =====");
                Console.Read();
            }
        }
    }
}