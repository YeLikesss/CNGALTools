using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;


namespace NvlKr2Extract
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
            MemoryStream compressed = new MemoryStream(compressData);      //创建压缩数据流
            MemoryStream decompressed = new MemoryStream();         //创建解压数据流
            InflaterInputStream zlibInput = new InflaterInputStream(compressed);        //创建输入压缩数据流
            zlibInput.CopyTo(decompressed);         //获得解压数据流
            return decompressed.ToArray();          
        }
    }
}
