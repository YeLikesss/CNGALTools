using System;
using System.Windows.Forms;
using GameCreatorStatic.Extractor.V1;

namespace ExtractorV1
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using FolderBrowserDialog fbd = new()
            {
                Description = "Game Creator V1 - 请选择游戏文件夹",
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                UseDescriptionForTitle = true,
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string gameDir = fbd.SelectedPath;
                GCExtractorV1 extractor = new();
                extractor.Extract(gameDir);

                Console.WriteLine("================ Game Creator V1 提取完成 ====================\r\n");
            }

            Console.WriteLine("===============请按任意键退出程序==============");
            Console.Read();
        }
    }
}