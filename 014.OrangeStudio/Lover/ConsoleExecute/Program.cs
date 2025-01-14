using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ConsoleExecute
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //情人节:不见不散

            using OpenFileDialog ofd = new()
            {
                AddExtension = true,
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".dat",
                Filter = "dat封包(*.dat)|*.dat|所有文件(*.*)|*.*",
                Multiselect = true,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "情人节:不见不散 - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string imgPath in ofd.FileNames)
                {
                    using Bitmap? bitmap = ImageDecoder.Load(imgPath);
                    if (bitmap is not null)
                    {
                        bitmap.Save(Path.ChangeExtension(imgPath, ".png"), ImageFormat.Png);
                        Console.WriteLine("图像解码成功:{0}", Path.GetFileName(imgPath));
                    }
                }
                Console.WriteLine("===== 情人节:不见不散 - 图像解码成功 =====");
                Console.Read();
            }
        }
    }
}