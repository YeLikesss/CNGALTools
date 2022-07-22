using System;
using System.Collections.Generic;
using System.Collections;
using Extractor;
using Extractor.Games;
using Extractor.Renpy;
using System.Linq;
using System.Text;
using System.IO;
using System.Buffers;


namespace ConsoleExecute
{
    class Program
    {
        static void Main(string[] args)
        {


            string gameDir = "你的游戏目录/Your Game Directory";

            string targetPycDir = string.Concat(gameDir, "\\Renpy");

            string targetArchiveDir = string.Concat(gameDir, "\\game");

            string rpycDir= string.Concat(gameDir, "\\Extract");

            //Archive archive = new(AeonOnMosaicAnemone.Key, AeonOnMosaicAnemone.XorVector, AeonOnMosaicAnemone.SubstitutionBox256_1, AeonOnMosaicAnemone.SubstitutionBox256_2, AeonOnMosaicAnemone.SubstitutionBox256_3, AeonOnMosaicAnemone.SubstitutionBox256_4, AeonOnMosaicAnemone.SubstitutionBox256_5, AeonOnMosaicAnemone.SubstitutionBox256_6, AeonOnMosaicAnemone.SubstitutionBox256_7, AeonOnMosaicAnemone.SubstitutionBox256_8, string.Concat(gameDir, "\\Extract"));


            //archive.DecryptFile(string.Empty, new DirectoryInfo(targetPycDir));

            //archive.RPAExtract(new DirectoryInfo(targetArchiveDir), AeonOnMosaicAnemone.DecryptTableInfo,AeonOnMosaicAnemone.DecryptArchiveInfo,AeonOnMosaicAnemone.DecryptArchiveHeader);


            //Script.RPYCsUnpake(rpycDir);


            Console.ReadKey();

        }



    }
}
