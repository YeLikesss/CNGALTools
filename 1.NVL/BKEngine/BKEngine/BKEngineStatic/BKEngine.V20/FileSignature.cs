using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BKEngine.V20
{
    public class FileSignature
    {
        /// <summary>
        /// 文件头特征
        /// </summary>
        private static readonly byte[] fileHeader = new byte[]
        {
            0x42,0x4B,0x41,0x52,0x43,0x02
        };
        /// <summary>
        /// 获取文件头
        /// </summary>
        public static byte[] Header => FileSignature.fileHeader;
    }
}
