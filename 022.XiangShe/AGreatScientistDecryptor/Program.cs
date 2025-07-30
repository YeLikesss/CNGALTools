using System;
using System.IO;
using System.Windows.Forms;
using XiangSheStatic.Crypto.V1;

namespace AGreatScientistDecryptor
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using FolderBrowserDialog fbd = new()
            {
                Description = "[大科学家] - 请选择Unity资源文件夹",
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                UseDescriptionForTitle = true,
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string gameDir = fbd.SelectedPath;

                GameDataBase gameData = new AGreatScientist();
                ResourceExtractor extractor = new(gameData);
                extractor.Extract(gameDir);

                Console.WriteLine("================ [大科学家] 提取完成 ====================\r\n");
            }

            Console.WriteLine("===============请按任意键退出程序==============");
            Console.Read();
        }
    }
}