using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;

namespace BKEngine
{
    /// <summary>
    /// bz2压缩解压
    /// </summary>
    public class BZip2Helper
    {
        /// <summary>
        /// 创建解压缩流
        /// </summary>
        /// <param name="s">流</param>
        /// <returns></returns>
        public static Stream CreateDecompressStream(Stream s)
        {
            MemoryStream ms = new(1024 * 1024 * 16);
            using BZip2InputStream unbZip2 = new(s);

            int temp =  unbZip2.ReadByte();
            while (temp != -1)
            {
                ms.WriteByte((byte)temp);
                temp = unbZip2.ReadByte();
            }

            ms.Position = 0;
            return ms;
        }
    }
}
