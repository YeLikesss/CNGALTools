using System;
using System.IO;
using LightVNStatic;


namespace ConsoleExecute
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string gameExe = "D:\\Galgame Reverse\\Putrika_1st\\Putrika1st.exe";
            ExtractDemoV2(gameExe);

            Console.WriteLine("=========提取完成=========");
            Console.Read();
        }


        private static void ExtractDemoV1(string[] pkgPaths)
        {
            ICryptoFilter filter = new UenaFarFireworks();
            for (int i = 0; i < pkgPaths.Length; ++i)
            {
                string pkgPath = pkgPaths[i];
                string fileName = Path.GetFileName(pkgPath);
                PackageV1 package = new(pkgPath, filter);
                if (package.Extract())
                {
                    Console.WriteLine("成功:{0}", fileName);
                }
                else
                {
                    Console.WriteLine("失败:{0}", fileName);
                }
            }
        }

        private static void ExtractDemoV2(string gameExePath)
        {
            if(Path.GetDirectoryName(gameExePath) is string dir)
            {
                PutrikaFirst game = new();

                PackageV2 package = new(dir, game.FileListRelativePath, game);
                package.Extract();
            }
        }
    }
}