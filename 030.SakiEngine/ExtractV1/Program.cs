using System;
using System.IO;
using System.Windows.Forms;
using SakiStatic;

namespace ExtractV1
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
                DefaultExt = ".sakipak",
                Filter = "sakipak封包(*.sakipak)|*.sakipak|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "SakiEngine - 选择封包",
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] files = ofd.FileNames;
                foreach (string f in files)
                {
                    SakiPackageV1? pkg = SakiPackageV1.Open(f);
                    if(pkg is null)
                    {
                        Console.WriteLine($"[{Path.GetFileName(f)}] 非法的封包");
                        continue;
                    }
                    pkg.Extract(Path.GetDirectoryName(f)!);
                }
            }
            Console.WriteLine("==========请按任意键退出==========");
            Console.Read();
        }
    }
}