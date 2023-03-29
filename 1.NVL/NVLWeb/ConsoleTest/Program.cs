using NVLWebStatic;
using System;
using System.IO;
using System.Collections.Generic;

namespace ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string gameResFolder = "D:\\Galgame Reverse\\package\\resources";       //此处填写你的游戏asar封包目录
            string outFolder = Path.Combine(gameResFolder, "Extract_Static");

            using ASARPackage package = ASARPackage.CreateInstance(Path.Combine(gameResFolder,"game.asar"));
            package.Extract(outFolder);

            EndOfTheWorld fix = new(outFolder);
            fix.DecodeAsset();
        }
    }
}