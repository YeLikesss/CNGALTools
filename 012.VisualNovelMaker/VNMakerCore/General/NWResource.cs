using System;
using System.Collections.Generic;
using System.IO;

namespace VNMakerCore.General
{
    /// <summary>
    /// nw.js资源类
    /// </summary>
    public class NWResource
    {
        /// <summary>
        /// 提取单个文件
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <param name="filter">解密器</param>
        /// <param name="progress">进度回调</param>
        public static void ExtractFile(string file, ICryptoFilter? filter, IProgress<string>? progress)
        {
            if (!File.Exists(file))
            {
                progress?.Report($"文件不存在: {file}");
                return;
            }

            if (!NWPath.IsValidPath(file))
            {
                progress?.Report($"文件不是VNMaker资源: {file}");
                return;
            }

            string currentDir = NWPath.GetRootDirectory(file);
            string relativePath = NWPath.GetRelativePath(file);
            string extractPath = Path.Combine(currentDir, "Static_Extract", relativePath);
            {
                if (Path.GetDirectoryName(extractPath) is string dir && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            byte[] buf = new byte[4096];
            long offset = 0L;

            using FileStream inFs = File.OpenRead(file);
            using FileStream outFs = File.Create(extractPath);

            while (inFs.Position < inFs.Length)
            {
                int readLen = inFs.Read(buf, 0, buf.Length);

                filter?.Decrypt(buf, offset, 0, readLen);
                outFs.Write(buf, 0, readLen);

                offset += readLen;
            }
            outFs.Flush();

            progress?.Report($"成功: {relativePath}");
        }

        /// <summary>
        /// 提取多个文件
        /// </summary>
        /// <param name="files">文件路径列表</param>
        /// <param name="filter">解密器</param>
        /// <param name="progress">进度回调</param>
        public static void ExtractFiles(List<string> files, ICryptoFilter? filter, IProgress<string>? progress)
        {
            foreach(string s in files)
            {
                NWResource.ExtractFile(s, filter, progress);
            }
        }
    }
}
