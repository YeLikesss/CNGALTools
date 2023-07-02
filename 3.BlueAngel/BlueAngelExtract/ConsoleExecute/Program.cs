using System;
using System.Collections.Generic;
using BlueAngel.StarlightofAeons;
using System.Linq;
using BlueAngel.V1;
using BlueAngel;
using System.Threading.Tasks;
using System.IO;


namespace ConsoleExecute
{
    class Program
    {
        static void Main(string[] args)
        {
            //获取exe启动参数
            List<string> filePaths = Environment.GetCommandLineArgs().ToList();
            if (filePaths.Count < 2)
            {
                return;
            }
            //移除自身文件路径
            filePaths.RemoveAt(0);
            //解包
            Parallel.ForEach(filePaths, filepath =>
            {
                Console.WriteLine(string.Concat(filepath,"    开始解包"));
                Archive archive = new(filepath);
                ArchiveCrypto.SubstitutionBoxInitialize(out archive.mTableKey32_1, out archive.mTableKey32_2, out archive.mTableKey32_3,
                                                        out archive.mTableKey32_4, out archive.mTableKey32_5, out archive.mTableKey32_6,
                                                        out archive.mTableKey32_7, out archive.mTableKey32_8, out archive.mTableKey32_9,
                                                        out archive.mTableKey8_1, out archive.mTableKey8_2);
                Console.WriteLine("静态表生成完毕");
                archive.Extract();
            });
            Console.WriteLine("\n\n========请按任意键退出程序========");
            Console.ReadKey();
        }
    }
}
