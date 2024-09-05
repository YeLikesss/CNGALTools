using NekoNovelStatic;
using System;
using System.IO;

namespace ConsoleExecute
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //游戏文件夹
            string gameDir = "D:\\Galgame Reverse\\Lucy -The Eternity She Wished For-";

            string outputDirectory = Path.Combine(gameDir, "Static_Extract");

            string[] packageFiles = Directory.GetFiles(gameDir, "*.nkpack", SearchOption.TopDirectoryOnly);
            foreach(string path in packageFiles)
            {
                NekoPackage package = new(path);
                if (package.IsVaild)
                {
                    package.Extract(outputDirectory);
                }
                else
                {
                    Console.WriteLine("错误的封包:{0}", package.PackageName);
                }
            }

            Console.WriteLine("=========请按任意键退出=========");
            Console.Read();
        }
    }
}