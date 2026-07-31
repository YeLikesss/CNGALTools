using System;
using System.Windows.Forms;

namespace StaticExtractor
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
                DefaultExt = ".xp3",
                Filter = "xp3封包(*.xp3)|*.xp3|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "绯色的记忆之痕V2 - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string packPath in ofd.FileNames)
                {
                    SPKArchiveV2 archive = new(packPath);
                    archive.Extract();
                }
                Console.WriteLine("==== 绯色的记忆之痕V2 - 提取成功 ====");
                Console.Read();
            }
        }
    }
    
}