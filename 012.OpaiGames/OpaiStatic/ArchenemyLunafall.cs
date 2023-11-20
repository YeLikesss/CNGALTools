using System;
using System.Collections.Generic;
using System.IO;
using OpaiStatic.Extractor;
using OpaiStatic.Filter;

namespace OpaiStatic
{
    /// <summary>
    /// 执谕者：坠月之兆
    /// </summary>
    public class ArchenemyLunafall : IExtractor
    {
        public class ArchenemyLunafallFilter : XorFilterBase
        {
            public override byte[] Key { get; } = new byte[] { 0x0A, 0x2B, 0x36, 0x6F, 0x0B };
        }

        public void Extract(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath) && !Directory.Exists(rootPath))
            {
                return;
            }

            string[] subFolders = new string[]
            {
                "resources\\Audio",
                "resources\\Graphics",
            };

            IFilter filter = new ArchenemyLunafallFilter();
            Span<byte> buf = stackalloc byte[16 * 1024];

            foreach(string folder in subFolders)
            {
                foreach(string path in Directory.EnumerateFiles(Path.Combine(rootPath, folder), "*.*", SearchOption.AllDirectories))
                {
                    string ext = Path.GetExtension(path);
                    if(ext == ".webm")
                    {
                        continue;
                    }

                    string relativePath = path[(rootPath.Length + 1)..];
                    string outPath = Path.Combine(rootPath, "Static_Extract", relativePath);
                    {
                        if (Path.GetDirectoryName(outPath) is string dir)
                        {
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                        }
                    }

                    using FileStream inFs = File.OpenRead(path);
                    using FileStream outFs = File.Create(outPath);

                    long blockOffset = 0;
                    while (blockOffset < inFs.Length)
                    {
                        int readLen = inFs.Read(buf);
                        filter.Decrypt(buf[..readLen], blockOffset);
                        outFs.Write(buf[..readLen]);
                        blockOffset += readLen;
                    }
                    outFs.Flush();
                    Console.WriteLine("Extract:{0}", relativePath);
                }
            }
        }
    }
}
