using System;
using System.IO;
using System.IO.Compression;

namespace NVLKR2Static
{

    public class Zlib
    {
        /// <summary>
        /// 创建解压缩流
        /// </summary>
        /// <param name="s">原数据流</param>
        /// <returns></returns>
        public static Stream CreateDecompressStream(Stream s)
        {
            using ZLibStream zlib = new(s, CompressionMode.Decompress);
            MemoryStream decompressed = new();
            zlib.CopyTo(decompressed);
            decompressed.Position = 0L;
            return decompressed;
        }
    }
}
