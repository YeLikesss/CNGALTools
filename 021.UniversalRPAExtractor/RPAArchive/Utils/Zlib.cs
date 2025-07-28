using System;
using System.IO.Compression;
using System.IO;

namespace RPAArchive.Utils
{
    public class Zlib
    {
        /// <summary>
        /// Zlib数据解压
        /// </summary>
        /// <param name="compressData">Zlib压缩数据</param>
        /// <returns>解压后数据</returns>
        public static byte[] Decompress(byte[] compressData)
        {
            using MemoryStream compressed = new(compressData, false);
            using MemoryStream decompressed = new();
            using ZLibStream zlib = new(compressed, CompressionMode.Decompress);
            zlib.CopyTo(decompressed);
            return decompressed.ToArray();
        }
    }
}
