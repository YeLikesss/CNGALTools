using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using IrregularsStatic;

namespace IrregularsExtractorV1
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<(string, List<string>, GameInformationBase)> list = new()
            {
                ("MOBIUS BAND* [Demo]", new(){ "settings", "data", "localization"}, new MobiusBand()),
                ("MOBIUS BAND*", new(){ "settings", "data" }, new MobiusBand()),
            };

            Console.WriteLine("Irregular V1 选择游戏:");
            for(int i = 0; i < list.Count; ++i)
            {
                Console.WriteLine("{0}: {1}", i, list[i].Item1);
            }
            Console.WriteLine();

            if (Console.ReadLine() is string sid && int.TryParse(sid, out int id))
            {
                if (id < list.Count)
                {
                    using FolderBrowserDialog fbd = new()
                    {
                        Description = "Irregular V1 - 请选择游戏文件夹",
                        ShowNewFolderButton = false,
                        AutoUpgradeEnabled = true,
                        UseDescriptionForTitle = true
                    };

                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string gameDir = fbd.SelectedPath;
                        List<string> resDirs = list[id].Item2;
                        GameInformationBase gameInformation = list[id].Item3;

                        foreach (string resdir in resDirs)
                        {
                            List<string> resFiles = PathUtil.EnumerateFullName(Path.Combine(gameDir, resdir));
                            foreach (string resFilePath in resFiles)
                            {
                                using Stream stream = gameInformation.CreateDecryptStream(resFilePath);
                                if (stream != Stream.Null)
                                {
                                    string outPath = Path.Combine(gameDir, "Static_Extract", resFilePath[(gameDir.Length + 1)..]);
                                    {
                                        if (Path.GetDirectoryName(outPath) is string dir && !Directory.Exists(dir))
                                        {
                                            Directory.CreateDirectory(dir);
                                        }
                                        using FileStream outFs = new(outPath, FileMode.Create, FileAccess.ReadWrite);
                                        stream.CopyTo(outFs);
                                        outFs.Flush();
                                    }
                                }
                            }
                        }

                        Console.WriteLine("===== Irregular V1 - 解密完成 =====");
                        Console.Read();
                    }
                }
            }
        }
    }
}