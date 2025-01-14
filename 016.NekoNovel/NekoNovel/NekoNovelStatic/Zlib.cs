using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace NekoNovelStatic
{
    internal class Zlib
    {
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="data">压缩数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <returns>解压后数据</returns>
        public static byte[] Decompress(byte[] data, int offset, int length)
        {
            using MemoryStream compressed = new(data, offset, length, false);
            using MemoryStream decompressed = new();
            using ZLibStream zlib = new(compressed, CompressionMode.Decompress);
            zlib.CopyTo(decompressed);
            return decompressed.ToArray();
        }
    }
}
