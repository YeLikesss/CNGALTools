using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using Extractor.ZixRenpy8V1.Crypto;
using Extractor.ZixRenpy8V1.Game;
using Extractor.ZixRenpy8V1.Renpy;

namespace ConsoleExecute
{
    class Program
    {
        static void Main(string[] args)
        {
            string gameDir = "E:\\The Neverland of the Mountain and Sea";

            RenpyPath renpyPath = new(gameDir);
            string[] modulePaths = renpyPath.GetAllModuleFilesFullPath();
            string extractPath = renpyPath.GetExtractPath();
            string[] archiveFilePaths = renpyPath.GetAllArchiveFilesFullPath();

            TheNeverlandOfTheMountainAndSea game = new();
            IRPAExtractor extractor = game;
            IKeyInformation keyInformation = game;

            //解密模块
            {
                Crypto128 crypto = new(keyInformation);
                foreach (var p in modulePaths)
                {
                    string relativePath = renpyPath.GetRelativePath(p);
                    string extractFulllPath = Path.Combine(extractPath, renpyPath.FixExtension(relativePath));
                    crypto.Decrypt(p, extractFulllPath);

                    Console.WriteLine("{0}  ---> Decrypt Success", relativePath);
                }
            }

            //提取封包
            {
                foreach (var p in archiveFilePaths)
                {
                    extractor.Extract(p, extractPath);
                }
            }

            Console.WriteLine("Extract Completed");
            Console.ReadKey();
        }
    }
}
