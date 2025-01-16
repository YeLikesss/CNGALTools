using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LightVNStatic;

namespace LightVNExtractorV1
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<CryptoFilterV1> list = new()
            {
                new UenaFarFireworks(),
            };

            Console.WriteLine("请选择游戏:");
            for (int i = 0; i < list.Count; ++i)
            {
                Console.WriteLine("{0}. {1}", i, list[i]);
            }


            if(Console.ReadLine() is string sid && int.TryParse(sid, out int id))
            {
                if (id < list.Count)
                {
                    using OpenFileDialog ofd = new()
                    {
                        AddExtension = true,
                        AutoUpgradeEnabled = true,
                        CheckFileExists = true,
                        CheckPathExists = true,
                        DefaultExt = ".vndat",
                        Filter = "vndat封包(*.vndat)|*.vndat|所有文件(*.*)|*.*",
                        Multiselect = true,
                        RestoreDirectory = true,
                        ShowHelp = false,
                        Title = "LightVN V1 - 选择封包",
                    };
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        PackageV1 package = new(list[id]);
                        foreach(string path in ofd.FileNames)
                        {
                            package.Extract(path);
                        }

                        Console.WriteLine("===== Light.VN V1 - 提取完成 =====");
                        Console.Read();
                    }
                }
            }
        }
    }
}