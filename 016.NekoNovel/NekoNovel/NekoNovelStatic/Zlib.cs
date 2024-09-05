using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

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
            MemoryStream compressed = new(data, offset, length);
            MemoryStream decompressed = new();
            InflaterInputStream zlibInput = new(compressed);
            zlibInput.CopyTo(decompressed);
            return decompressed.ToArray();
        }
    }
}
