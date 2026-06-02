using System;
using System.Windows.Forms;

namespace TheEndlessJourney
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
                Filter = "所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "无终之旅 - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] files = ofd.FileNames;
                foreach(string f in files)
                {
                    Package.Extract(f);
                }
            }

            Console.WriteLine("==========请按任意键退出==========");
            Console.Read();
        }
    }
}