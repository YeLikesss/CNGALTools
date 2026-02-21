using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace IdolForging
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
                DefaultExt = ".bundle",
                Filter = "游戏封包(*.bundle)|*.bundle|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "偶像调教事件簿 - 选择封包",
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] files = ofd.FileNames;
                if(files.Length != 0)
                {
                    string decDir = Path.Combine(Path.GetDirectoryName(files[0])!, "Static_Extract");
                    if (!Directory.Exists(decDir))
                    {
                        Directory.CreateDirectory(decDir);
                    }

                    using Aes aes = Aes.Create();
                    aes.Padding = PaddingMode.Zeros;
                    {
                        byte[] iv = new byte[16];
                        for (int i = 0; i < iv.Length; ++i)
                        {
                            iv[i] = (byte)i;
                        }
                        aes.IV = iv;
                    }
                    aes.Key = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOP");
                    aes.Mode = CipherMode.ECB;

                    foreach (string file in files)
                    {
                        string filename = Path.GetFileName(file);

                        using FileStream inFs = File.OpenRead(file);
                        using FileStream outFs = File.Create(Path.Combine(decDir, filename));

                        using ICryptoTransform crypto = aes.CreateDecryptor();
                        using CryptoStream cs = new(inFs, crypto, CryptoStreamMode.Read, false);

                        cs.CopyTo(outFs);
                        outFs.Flush();

                        Console.WriteLine($"解密成功: {filename}");
                    }
                }
            }
            Console.WriteLine("================按任意键退出===================");
            Console.Read();
        }
    }
}