using System;
using System.IO;
using System.Runtime.InteropServices;
using HamidashiCreativeStatic;

namespace ConsoleExecute
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //此处填写你得游戏路径
            string gameDirectory = "D:\\Galgame Reverse\\Hamidashi\\Creative v105";     
            for (int i = 0; i < 256; ++i)
            {
                string path = Path.Combine(gameDirectory, $"hamidashi.{i:D3}.pfs");
                SWArtemisArchive? archive = SWArtemisArchive.CreateInstance(path);
                archive?.Extract();
            }
            Console.WriteLine("\n\n============请按任意键退出============\n\n");
            Console.ReadKey();
        }
    }
}