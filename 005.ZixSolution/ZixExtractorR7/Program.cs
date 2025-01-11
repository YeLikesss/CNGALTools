using System;
using System.IO;
using System.Windows.Forms;
using Extractor.ZixRenpy7V1.Crypto;
using Extractor.ZixRenpy7V1.Renpy;

namespace ZixExtractorR7
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Zix Renpy 7 版本解包\n\n");
            Console.WriteLine("请选择游戏:");
            Console.WriteLine("1. 时间记忆:碎片");

            if (Console.ReadLine() is string s && int.TryParse(s, out int ordinal))
            {
                using FolderBrowserDialog fbd = new()
                {
                    Description = "Zix Renpy 7 - 请选择游戏文件夹",
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
                        1 => new AeonOnMosaicAnemone(),
                        _ => null,
                    };

                    if(game != null)
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

                        Console.WriteLine("===== Zix Renpy 7 --- 提取完毕 =====");
                        Console.Read();
                    }
                }
            }
        }
    }
}