using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IrregularsStatic;

namespace ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //设置你的游戏文件夹
            string gameDir = "D:\\Galgame Reverse\\MOBIUS BAND Demo";

            List<string> resDirs = new(3)
            {
                "settings",
                "data",
                "localization",
            };

            MobiusBand mobiusBand = new();

            foreach(string resdir in resDirs)
            {
                List<string> resFiles = PathUtil.EnumerateFullName(Path.Combine(gameDir, resdir));

                foreach(string resFilePath in resFiles)
                {
                    using Stream stream = mobiusBand.CreateStream(resFilePath);
                    if (stream != Stream.Null)
                    {
                        string outPath = Path.Combine(gameDir, "Static_Extract", resFilePath[(gameDir.Length + 1)..]);
                        {
                            string dir = Path.GetDirectoryName(outPath);
                            if (!Directory.Exists(dir))
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
        }
    }
}