using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Decrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            List<byte> key = new List<byte>() { 0x0A, 0x2B, 0x36, 0x6F, 0x0B };
            //获取exe启动参数
            string[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length < 2)
            {
                return;
            }

            List<string> filepathList = arguments.ToList();
            //移除自身路径
            filepathList.RemoveAt(0);

            //异步批量解包
            Parallel.ForEach(filepathList, fileName =>
            {
                FileInfo fileInfo = new FileInfo(fileName);
                string outputDir = fileInfo.DirectoryName + "/Extract/";
                if (Directory.Exists(outputDir) == false)
                {
                    Directory.CreateDirectory(outputDir);   //检查文件夹是否存在 不存在则创建
                }
                //读取文件
                byte[] temp = File.ReadAllBytes(fileInfo.FullName);
                //异步解密
                Parallel.For(0, temp.Length, index => 
                {
                    int keyIndex = (index + key.Count) % 5;
                    temp[index] ^= key.ElementAt(keyIndex);
                });
                //写入文件
                string outputFilePath = outputDir + fileInfo.Name;
                File.WriteAllBytes(outputFilePath, temp);
            });
        }
    }
}
