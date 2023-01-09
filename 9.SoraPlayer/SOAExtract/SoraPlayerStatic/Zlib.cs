using System;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;

namespace SoraPlayerStatic
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
            MemoryStream compressed = new(compressData);      //创建压缩数据流
            MemoryStream decompressed = new();         //创建解压数据流
            InflaterInputStream zlibInput = new(compressed);        //创建输入压缩数据流
            zlibInput.CopyTo(decompressed);         //获得解压数据流
            return decompressed.ToArray();
        }
    }
}
