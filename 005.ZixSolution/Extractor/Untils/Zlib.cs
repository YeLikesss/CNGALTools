using System.IO;
using System.IO.Compression;

namespace Extractor.Untils
{
    /// <summary>
    /// Zlib
    /// </summary>
    public class Zlib
    {
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="data">压缩数据</param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            using MemoryStream compressed = new(data, false);
            using MemoryStream decompressed = new();
            using ZLibStream zlib = new(compressed, CompressionMode.Decompress);
            zlib.CopyTo(decompressed);
            return decompressed.ToArray();
        }
    }
}
