using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using FutureRadioStatic;

namespace ConsoleExecute
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> pckNames = new(16)
            {
                "adult.bin",
                "bgm.bin",
                "def.bin",
                "image.bin",
                "scripts.bin",
                "sound.bin",
            };

            //此处填入你的游戏封包文件夹路径  Your Game Archives Directory Path
            string pckDirectoryPath = "E:\\Future Radio\\The Future Radio and the Artificial Pigeons_Data\\StreamingAssets";

            string extractDirectory = Path.Combine(pckDirectoryPath, "Static_Extract");
            foreach(string pck in pckNames)
            {
                BinArchive binArchive = BinArchive.CreateInstance(Path.Combine(pckDirectoryPath, pck));
                binArchive?.Extract(extractDirectory);
                binArchive?.Dispose();
            }
        }
    }
}