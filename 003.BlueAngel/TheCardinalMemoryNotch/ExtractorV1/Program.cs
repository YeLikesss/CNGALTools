using System;
using System.Windows.Forms;
using EngineCoreStatic;

namespace ExtractorV1
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
                DefaultExt = ".spk",
                Filter = "spk封包(*.spk)|*.spk|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "绯色的记忆之痕 - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string packPath in ofd.FileNames)
                {
                    SPKArchive archive = new(packPath);
                    archive.Extract();
                }
                Console.WriteLine("==== 绯色的记忆之痕 - 提取成功 ====");
                Console.Read();
            }
        }
    }
}