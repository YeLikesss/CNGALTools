using System;
using System.Windows.Forms;

namespace ConsoleExecute
{
    class Program
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
                DefaultExt = "",
                Filter = "所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "iFVN - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string packPath in ofd.FileNames)
                {
                    IFAction.V1.Archive archive = new();
                    archive.Extract(packPath);
                }
                Console.WriteLine("===========提取完成==========");
                Console.ReadKey();
            }
        }
    }
}
