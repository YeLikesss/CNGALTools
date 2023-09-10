using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ZstdNet;

namespace BKEngine
{
    /// <summary>
    /// zstd压缩解压
    /// </summary>
    public class ZstdHelper
    {
        /// <summary>
        /// 创建zstd解压缩流
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Stream CreateDecompressStream(Stream s)
        {
            MemoryStream ms = new(1024 * 1024 * 16);
            using DecompressionStream zstd = new(s);

            int temp = zstd.ReadByte();
            while (temp != -1)
            {
                ms.WriteByte((byte)temp);
                temp = zstd.ReadByte();
            }

            ms.Position = 0;
            return ms;
        }
    }
}
