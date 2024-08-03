using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using EngineCoreStatic;

namespace ConsoleExecute
{
    internal class Program
    {
        unsafe static void Main(string[] args)
        {
            //设置你的游戏路径
            string gameDir = "D:\\Galgame Reverse\\Loulan";
            string redir = Path.Combine(gameDir, "Re");
            string outDir = Path.Combine(gameDir, "Staric_Extract");

            string[] files = Directory.GetFiles(gameDir, "*.hac", SearchOption.TopDirectoryOnly);
            foreach(string file in files)
            {
                using HACPackage pkg = new(file);
                if (pkg.IsVaild)
                {
                    pkg.Extract(outDir);
                }
            }
            Console.WriteLine("提取完成");
            Console.Read();
        }
    }
}