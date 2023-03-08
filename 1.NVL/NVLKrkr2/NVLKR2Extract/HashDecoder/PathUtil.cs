using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Utils.PathProcess
{
    /// <summary>
    /// 路径遍历器
    /// </summary>
    public class PathUtil
    {
        /// <summary>
        /// 枚举所有文件路径 (全路径)
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <returns></returns>
        public static List<string> EnumerateFullName(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, string.Empty, SearchOption.AllDirectories).ToList();
        }

        /// <summary>
        /// 枚举所有文件路径 (全路径)
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <param name="searchPattern">扫描参数</param>
        /// <returns></returns>
        public static List<string> EnumerateFullName(string directoryPath, string searchPattern)
        {
            return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories).ToList();
        }

        /// <summary>
        /// 枚举当前文件夹文件路径 (全路径)
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <returns></returns>
        public static List<string> EnumerateCurrentDirectoryFullPath(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, string.Empty, SearchOption.TopDirectoryOnly).ToList();
        }

        /// <summary>
        /// 枚举当前文件夹文件路径 (全路径)
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <param name="searchPattern">扫描参数</param>
        /// <returns></returns>
        public static List<string> EnumerateCurrentDirectoryFullPath(string directoryPath, string searchPattern)
        {
            return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly).ToList();
        }

        /// <summary>
        /// 枚举所有文件名
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <returns></returns>
        public static List<string> EnumerateFileName(string directoryPath)
        {
            return PathUtil.EnumerateFullName(directoryPath).ConvertAll(s => Path.GetFileName(s));
        }

        /// <summary>
        /// 枚举所有文件路径 (相对路径)
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <returns></returns>
        public static List<string> EnumerateRelativeName(string directoryPath)
        {
            return PathUtil.EnumerateFullName(directoryPath).ConvertAll(s => s[(directoryPath.Length + 1)..]);
        }

        /// <summary>
        /// 枚举所有文件夹路径 (全路径)
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static List<string> EnumerateFullDirectory(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath, string.Empty, SearchOption.AllDirectories).ToList();
        }

        /// <summary>
        /// 枚举所有文件夹路径 (相对路径)
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <returns></returns>
        public static List<string> EnumerateRelativeDirectory(string directoryPath)
        {
            return PathUtil.EnumerateFullDirectory(directoryPath).ConvertAll(s => s[(directoryPath.Length + 1)..]);
        }

        /// <summary>
        /// 枚举所有文件名
        /// <para>KR标准格式</para>
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <returns></returns>
        public static List<string> EnumerateKirikiriFileName(string directoryPath)
        {
            return PathUtil.EnumerateFileName(directoryPath).ConvertAll(s => s.ToLower());
        }

        /// <summary>
        /// 枚举所有文件路径 (相对路径)
        /// <para>KR标准格式</para>
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <returns></returns>
        public static List<string> EnumerateKirikiriRelativeName(string directoryPath)
        {
            return PathUtil.EnumerateRelativeName(directoryPath).ConvertAll(s => 
            {
                s = s.Replace('\\', '/');
                s = s.ToLower();
                return s;
            });
        }

        /// <summary>
        /// 枚举所有文件夹路径 (相对路径)
        /// <para>KR标准格式</para>
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <returns></returns>
        public static List<string> EnumerateKirikiriRelativeDirectory(string directoryPath)
        {
            return PathUtil.EnumerateRelativeDirectory(directoryPath).ConvertAll(s => 
            {
                s = s.Replace('\\', '/');
                s = s.ToLower();
                s += "/";
                return s;
            });
        }

        /// <summary>
        /// 删除空文件夹
        /// </summary>
        /// <param name="path">文件夹根目录路径</param>
        public static void DeleteEmptyDirectory(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            foreach (string dir in dirs)
            {
                DeleteEmptyDirectory(dir);
            }

            dirs = Directory.GetDirectories(path);

            if (dirs.Length == 0 && files.Length == 0)
            {
                Directory.Delete(path);
            }
        }
    }
}
