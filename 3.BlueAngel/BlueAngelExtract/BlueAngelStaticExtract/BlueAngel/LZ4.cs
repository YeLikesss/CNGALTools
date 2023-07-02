using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K4os.Compression.LZ4;

namespace BlueAngel
{
    /// <summary>
    /// Lz4
    /// </summary>
    public class LZ4Helper
    {
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="compressedData">压缩数据</param>
        /// <param name="uncompressLength">解压缩长度</param>
        /// <returns>解压缩后数据</returns>
        public static byte[] Decompress(byte[] compressedData, int uncompressLength)
        {
            byte[] buffer = new byte[uncompressLength];
            //LZ4解压
            LZ4Codec.Decode(compressedData, 0, compressedData.Length, buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="compressedData">压缩数据</param>
        /// <param name="decompressData">解压缩数据</param>
        /// <returns>解压缩后数据</returns>
        public static void Decompress(byte[] compressedData, byte[] decompressData)
        {
            //LZ4解压
            LZ4Codec.Decode(compressedData, 0, compressedData.Length, decompressData, 0, decompressData.Length);
        }
    }
}
