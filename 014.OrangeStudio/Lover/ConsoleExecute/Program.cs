using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ConsoleExecute
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //情人节:不见不散

            //图像解码
            {
                IEnumerable<string> imgPaths = args.Where(path => Path.GetExtension(path) == ".dat");
                foreach(string imgPath in imgPaths)
                {
                    Bitmap? bitmap = ImageDecoder.Load(imgPath);
                    if (bitmap != null)
                    {
                        bitmap.Save(Path.ChangeExtension(imgPath, ".png"), ImageFormat.Png);
                        bitmap.Dispose();
                        Console.WriteLine("图像解码成功:{0}", Path.GetFileName(imgPath));
                    }
                }
            }

            Console.WriteLine("=======================按任意键退出======================");
            Console.Read();
        }
    }
}