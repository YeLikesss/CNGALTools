using System;
using System.IO;
using System.Windows.Forms;

namespace _3001Pages
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
                DefaultExt = ".bbj",
                Filter = "游戏封包(*.bbj)|*.bbj|所有文件(*.*)|*.*",
                Multiselect = false,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "三千零一页 - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                string directory = Path.GetDirectoryName(filePath)!;
                string extractDir = Path.Combine(directory, "Static_Extract");
                if (!Directory.Exists(extractDir))
                {
                    Directory.CreateDirectory(extractDir);
                }

                string extractPath = Path.Combine(extractDir, Path.GetFileNameWithoutExtension(filePath) + ".asset");

                using FileStream inFs = File.OpenRead(filePath);
                using FileStream outFs = File.Create(extractPath);

                //去除头部垃圾数据
                inFs.Seek(0x21L, SeekOrigin.Begin);

                while (inFs.Position < inFs.Length)
                {
                    byte v = (byte)inFs.ReadByte();
                    outFs.WriteByte(v);
                }
                outFs.Flush();

                Console.WriteLine($"转换成功: {Path.GetFileName(filePath)}");
                Console.Read();
            }
        }
    }
}