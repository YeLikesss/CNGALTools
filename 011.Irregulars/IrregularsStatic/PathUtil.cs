using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IrregularsStatic
{
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
    }
}
