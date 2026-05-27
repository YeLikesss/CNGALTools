using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LightVNStatic;

namespace LightVNExtractorV2
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<IGameInfoV2> list = new()
            {
                new PutrikaFirst(),
            };

            Console.WriteLine("请选择游戏:");
            for (int i = 0; i < list.Count; ++i)
            {
                Console.WriteLine("{0}. {1}", i, list[i]);
            }

            if (Console.ReadLine() is string sid && int.TryParse(sid, out int id))
            {
                if (id < list.Count)
                {
                    using FolderBrowserDialog fbd = new()
                    {
                        Description = "LightVN V2 - 请选择游戏文件夹",
                        ShowNewFolderButton = false,
                        AutoUpgradeEnabled = true,
                        UseDescriptionForTitle = true
                    };
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        IGameInfoV2 gameInfo = list[id];

                        PackageV2 package = new(gameInfo, gameInfo as CryptoFilterV2);
                        package.Extract(fbd.SelectedPath);

                        Console.WriteLine("===== Light.VN V2 - 提取完成 =====");
                        Console.Read();
                    }
                }
            }
        }
    }
}