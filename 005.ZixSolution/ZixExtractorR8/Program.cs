using System;
using System.IO;
using System.Windows.Forms;
using Extractor.ZixRenpy8V1.Crypto;
using Extractor.ZixRenpy8V1.Renpy;

namespace ZixExtractorR8
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Zix Renpy 8 版本解包\n\n");
            Console.WriteLine("请选择游戏:");
            Console.WriteLine("1. 忆夏之铃");
            Console.WriteLine("2. 夏空的蒲公英");
            Console.WriteLine("3. 山的桃源乡 海的乌托邦");

            if (Console.ReadLine() is string s && int.TryParse(s, out int ordinal))
            {
                using FolderBrowserDialog fbd = new()
                {
                    Description = "Zix Renpy 8 - 请选择游戏文件夹",
                    ShowNewFolderButton = false,
                    AutoUpgradeEnabled = true,
                    UseDescriptionForTitle = true,
                };
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string gameDir = fbd.SelectedPath;

                    RenpyPath renpyPath = new(gameDir);
                    string[] modulePaths = renpyPath.GetAllModuleFilesFullPath();
                    string extractPath = renpyPath.GetExtractPath();
                    string[] archiveFilePaths = renpyPath.GetAllArchiveFilesFullPath();

                    object game = ordinal switch
                    {
                        1 => new SummerMemoryOfBell(),
                        2 => new DandelionsInTheSky(),
                        3 => new TheNeverlandOfTheMountainAndSea(),
                        _ => null,
                    };

                    if (game != null)
                    {
                        IExtractor extractor = game as IExtractor;
                        IKeyInformation keyInformation = game as IKeyInformation;

                        //解密模块
                        Crypto128 crypto = new(keyInformation);
                        foreach (var p in modulePaths)
                        {
                            string relativePath = renpyPath.GetRelativePath(p);
                            string extractFullPath = Path.Combine(extractPath, renpyPath.FixExtension(relativePath));
                            crypto.Decrypt(p, extractFullPath);
                        }
                        //提取封包
                        foreach (var p in archiveFilePaths)
                        {
                            extractor.Extract(p, extractPath);
                        }
                        extractor.ExtractScript(extractPath);

                        Console.WriteLine("===== Zix Renpy 8 --- 提取完毕 =====");
                        Console.Read();
                    }
                }
            }
        }
    }
}