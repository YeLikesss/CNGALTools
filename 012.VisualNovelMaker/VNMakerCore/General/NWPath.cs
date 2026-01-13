using System;
using System.Collections.Generic;
using System.IO;

namespace VNMakerCore.General
{
    /// <summary>
    /// nw.js框架路径类
    /// </summary>
    public class NWPath
    {
        /// <summary>
        /// 获取根路径
        /// </summary>
        /// <param name="path">绝对路径</param>
        public static string GetRootDirectory(string path)
        {
            string rootDir = string.Empty;
            if (!string.IsNullOrWhiteSpace(path))
            {
                string? dir = Path.GetDirectoryName(path);
                while(dir is not null)
                {
                    if (File.Exists(Path.Combine(dir, "nw.dll")) || File.Exists(Path.Combine(dir, "snapshot_blob.bin")))
                    {
                        rootDir = dir;
                        break;
                    }
                    dir = Path.GetDirectoryName(dir);
                }
            }
            return rootDir;
        }

        /// <summary>
        /// 获取相对路径
        /// </summary>
        /// <param name="path">绝对路径</param>
        public static string GetRelativePath(string path)
        {
            string root = NWPath.GetRootDirectory(path);
            if (string.IsNullOrEmpty(root))
            {
                return string.Empty;
            }

            return path[(root.Length + 1)..];
        }

        /// <summary>
        /// 获取是否为nw.js框架路径
        /// </summary>
        /// <param name="path">绝对路径</param>
        public static bool IsValidPath(string path)
        {
            return !string.IsNullOrEmpty(NWPath.GetRootDirectory(path));
        }
    }
}
