using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;


namespace EngineCoreStatic
{
    /// <summary>
    /// HAC无封包文件
    /// </summary>
    public class HACDirectFile
    {
        private readonly string mResourceDirectory;             //资源文件夹
        private readonly string mExtractDirectory;              //提取文件夹

        /// <summary>
        /// 提取
        /// </summary>
        public void Extract()
        {
            string resDirectory = this.mResourceDirectory;
            string extractDirectory = this.mExtractDirectory;
            if (!Directory.Exists(resDirectory))
            {
                Console.WriteLine("资源文件夹不存在: {0}", resDirectory);
                return;
            }
            
            string[] files = Directory.GetFiles(resDirectory, "*.*", SearchOption.AllDirectories);
            foreach(string path in files)
            {
                string relativePath = path[(resDirectory.Length + 1)..];
                string relativeDirectory = Path.GetDirectoryName(relativePath)!;
                string fileName = Path.GetFileName(relativePath);
                string extension = Path.GetExtension(fileName).ToLower();
                string outputDirectory = Path.Combine(extractDirectory, relativeDirectory);

                Action createOutputDirectoryFunc = () => 
                {
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }
                };

                switch (extension)
                {
                    case ".hgp":
                    {
                        createOutputDirectoryFunc();
                        using FileStream inStream = File.OpenRead(path);

                        HACHgpImageDecoder hgpDecoder = new(inStream, fileName);
                        hgpDecoder.ExtractPNG(outputDirectory);
                        Console.WriteLine("成功: {0}", relativePath);

                        break;
                    }
                    case ".tex":
                    {
                        createOutputDirectoryFunc();
                        using FileStream inStream = File.OpenRead(path);

                        HACTexImageDecoder texDecoder = new(inStream, fileName);
                        texDecoder.ExtractToPNG(outputDirectory);
                        Console.WriteLine("成功: {0}", relativePath);

                        break;
                    }
                    case ".htp":
                    {
                        createOutputDirectoryFunc();
                        string tilePath = Path.Combine(resDirectory, relativeDirectory + ".htl");
                        using FileStream htpStream = File.OpenRead(path);
                        using FileStream htlStream = File.OpenRead(tilePath);

                        HTPImageDecoder htpDecoder = new(htpStream, fileName);
                        htpDecoder.ExtractToPNG(outputDirectory, htlStream);
                        Console.WriteLine("成功: {0}", relativePath);

                        break;
                    }
                    case ".htl":
                    {
                        break;
                    }

                    case ".ogg":
                    case ".ogv":
                    {
                        //不提取
                        break;
                    }
                    default:
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resourceDirectory">资源路径</param>
        public HACDirectFile(string resourceDirectory)
        {
            this.mResourceDirectory = resourceDirectory;

            if(Path.GetDirectoryName(resourceDirectory) is string curDir)
            {
                string folderName = resourceDirectory[(curDir.Length + 1)..];
                this.mExtractDirectory = Path.Combine(curDir, "Static_Extract", folderName);
            }
            else
            {
                this.mExtractDirectory = string.Empty;
            }
        }
    }
}
