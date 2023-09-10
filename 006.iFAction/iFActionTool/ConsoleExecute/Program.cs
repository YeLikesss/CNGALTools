using System;

namespace ConsoleExecute
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("请拖拽目标文件到exe运行");
                return;
            }

            IFAction.V1.Archive archive = new();
            archive.Extract(args[0]);

            Console.WriteLine("===========提取完成==========");
            Console.ReadKey();
        }
    }
}
