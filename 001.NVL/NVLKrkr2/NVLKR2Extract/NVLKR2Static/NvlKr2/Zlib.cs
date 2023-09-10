using System;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;


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
            using InflaterInputStream zlibInput = new(s);
            MemoryStream decompressed = new();
            zlibInput.CopyTo(decompressed);
            decompressed.Position = 0;
            return decompressed;
        }
    }
}
