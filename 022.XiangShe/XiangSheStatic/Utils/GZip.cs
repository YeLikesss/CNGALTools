using System;
using System.IO;
using System.IO.Compression;

namespace XiangSheStatic.Utils
{
    /// <summary>
    /// GZip压缩
    /// </summary>
    public class GZip
    {
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="data">压缩数据</param>
        /// <returns>解压后数据</returns>
        public static byte[] Decompress(byte[] data)
        {
            using MemoryStream inMs = new(data, false);
            using MemoryStream outMs = new();
            using GZipStream gzStream = new(inMs, CompressionMode.Decompress);

            gzStream.CopyTo(outMs);

            return outMs.ToArray();
        }
    }
}
