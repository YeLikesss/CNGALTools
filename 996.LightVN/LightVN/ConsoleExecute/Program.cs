using System;
using System.IO;
using LightVNStatic;


namespace ConsoleExecute
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ICryptoFilter filter = new UenaFarFireworks();
            for (int i = 0; i < args.Length; ++i)
            {
                string pkgPath = args[i];
                string fileName = Path.GetFileName(pkgPath);
                PackageV1 package = new(pkgPath, filter);
                if (package.Extract())
                {
                    Console.WriteLine("Extract Success:{0}",fileName);
                }
                else
                {
                    Console.WriteLine("Extract Faild:{0}", fileName);
                }
            }
            Console.Read();
        }
    }
}