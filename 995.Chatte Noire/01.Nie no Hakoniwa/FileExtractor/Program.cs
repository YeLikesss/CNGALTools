using System;
using System.Diagnostics;
using System.IO;

namespace FileExtractor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 0)
            {
                using StreamWriter logger = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extractor.log"), false, System.Text.Encoding.Unicode);
                TextWriter orgOut = Console.Out;
                Console.SetOut(logger);
                foreach (string arg in args)
                {
                    ExfsPackage package = new(arg);
                    package.Extract();
                }
                Console.SetOut(orgOut);
                Console.WriteLine("==========请按任意键退出==========");
            }
            else
            {
                Console.WriteLine("请拖拽封包到Exe上");
            }
            Console.Read();
        }
    }
}