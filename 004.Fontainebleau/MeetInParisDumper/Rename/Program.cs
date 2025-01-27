using System;
using System.IO;
using System.Windows.Forms;

namespace Rename
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
                Filter = "所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "花都之恋 - pvr资源重命名",
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string resFile in ofd.FileNames)
                {
                    string fileNameNoExtension = Path.GetFileNameWithoutExtension(resFile);
                    if (Path.GetExtension(fileNameNoExtension) == ".pvr")
                    {
                        string filename = resFile.Replace(".pvr.png", ".png", StringComparison.OrdinalIgnoreCase);
                        File.Move(resFile, filename, true);
                    }
                }
                Console.WriteLine("===== 花都之恋 - 重命名成功 =====");
                Console.Read();
            }
        }
    }
}
