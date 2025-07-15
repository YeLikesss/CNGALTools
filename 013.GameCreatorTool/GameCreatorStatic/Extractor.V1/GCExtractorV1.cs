using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace GameCreatorStatic.Extractor.V1
{
    /// <summary>
    /// V1版加密
    /// </summary>
    public class GCExtractorV1
    {
        private readonly string[] mEncryptFolders = new string[]
        {
            "asset\\image",
        };

        /// <summary>
        /// 提取
        /// </summary>
        /// <param name="gameRootDirectory">游戏根目录</param>
        public void Extract(string gameRootDirectory)
        {
            foreach(string encryptFolder in this.mEncryptFolders)
            {
                string resourceDirectory = Path.Combine(gameRootDirectory, encryptFolder);
                if (Directory.Exists(resourceDirectory))
                {
                    foreach(string filePath in Directory.EnumerateFiles(resourceDirectory, "*.*", SearchOption.AllDirectories))
                    {
                        string relativePath = filePath[(gameRootDirectory.Length + 1)..];
                        string outPath = Path.Combine(gameRootDirectory, "Extract_Static", relativePath);

                        {
                            string outDir = Path.GetDirectoryName(outPath)!;
                            if (!Directory.Exists(outDir))
                            {
                                Directory.CreateDirectory(outDir);
                            }
                        }

                        using FileStream outFs = File.Create(outPath);
                        byte[] buf = File.ReadAllBytes(filePath);

                        long length = buf.LongLength;
                        long fakeBytePosition = (length - 1L) / 2L;

                        {
                            buf[1] ^= buf[2];
                            buf[2] ^= buf[1];
                            buf[1] ^= buf[2];
                        }

                        outFs.Write(buf, 0, (int)fakeBytePosition);
                        outFs.Write(buf, (int)(fakeBytePosition + 1L), (int)(length - fakeBytePosition - 1L));
                        outFs.Flush();

                        Console.WriteLine("解码成功:{0}", relativePath);
                    }
                }
                else
                {
                    Console.WriteLine("资源路径不存在:{0}", resourceDirectory);
                }
            }
        }
    }
}
