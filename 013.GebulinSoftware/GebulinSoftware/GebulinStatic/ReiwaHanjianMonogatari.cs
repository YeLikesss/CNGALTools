using System;
using System.Collections.Generic;
using System.IO;

namespace GebulinStatic
{
    /// <summary>
    /// 《令和罕见物语》
    /// </summary>
    public class ReiwaHanjianMonogatari : IExtractor
    {
        private readonly string mSubFolder = "asset\\image";

        public void Extract(string gameRootDirectory)
        {
            string imgPath = Path.Combine(gameRootDirectory, this.mSubFolder);
            if (Directory.Exists(imgPath))
            {
                foreach(string filePath in Directory.EnumerateFiles(imgPath, "*.*", SearchOption.AllDirectories))
                {
                    string relativePath = filePath[(gameRootDirectory.Length + 1)..];
                    string outPath = Path.Combine(gameRootDirectory, "Extract_Static", relativePath);

                    if(Path.GetDirectoryName(outPath) is string outDir && !Directory.Exists(outDir))
                    {
                        Directory.CreateDirectory(outDir);
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

                    Console.WriteLine("Dec Success:{0}", relativePath);
                }
            }
        }
    }
}
