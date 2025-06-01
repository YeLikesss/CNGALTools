using System;
using System.Windows.Forms;
using HappyLiveShowUpStatic;

namespace ConsoleExecute
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("=====================HappyLiveShowup Decryptor=================\n");
            Console.WriteLine("请选择游戏版本:");
            for (uint i = 0u; i < (uint)GameVersion.Max; ++i)
            {
                Console.WriteLine($"{i}:{(GameVersion)i}");
            }

            if(Console.ReadLine() is string s && Enum.TryParse<GameVersion>(s, out GameVersion ver) && ver != GameVersion.Max)
            {
                Console.WriteLine($"已选版本:{ver}");
                using OpenFileDialog ofd = new()
                {
                    AddExtension = true,
                    AutoUpgradeEnabled = true,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    DefaultExt = ".pack",
                    Filter = "pack封包(*.pack)|*.pack|所有文件(*.*)|*.*",
                    Multiselect = true,
                    RestoreDirectory = true,
                    ShowHelp = false,
                    Title = "HappyLiveShowup Decryptor - 选择封包",
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (string path in ofd.FileNames)
                    {
                        SWDataPack? pack = SWDataPack.TryOpen(path, ver);
                        if(pack is not null)
                        {
                            pack.Decrypt();
                            Console.WriteLine($"{pack.FileName}解密完成");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("版本输入错误");
            }
            Console.Read();
        }
    }
}