using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;

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
            MemoryStream compressed = new(data);
            MemoryStream decompressed = new();
            InflaterInputStream zlibInput = new(compressed);
            zlibInput.CopyTo(decompressed);
            return decompressed.ToArray();
        }
    }
}
