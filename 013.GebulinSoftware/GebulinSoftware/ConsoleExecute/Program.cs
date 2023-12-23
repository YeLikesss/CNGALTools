using GebulinStatic;
using System;
namespace ConsoleExecute
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IExtractor extractor = new ReiwaHanjianMonogatari();
            extractor.Extract("E:\\令和罕见物语");
            Console.WriteLine("===============请按任意键退出程序==============");
            Console.Read();
        }
    }
}