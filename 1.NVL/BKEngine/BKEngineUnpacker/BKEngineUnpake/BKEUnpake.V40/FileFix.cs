using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BKEUnpake.V40
{
    /// <summary>
    /// 文件修复
    /// </summary>
    public class FileFix
    {
        /// <summary>
        /// zstd文件头
        /// </summary>
        private static byte[] fixedHeader = new byte[]
        {
            0x28,0xB5,0x2F,0xFD
        };
        /// <summary>
        /// 修复文件资源的压缩文件头
        /// </summary>
        /// <param name="filedata">文件数据流</param>
        /// <returns></returns>
        public static byte[] CompressedResourcesFix(byte[] filedata)
        {
            List<byte> fileDataList= filedata.ToList();
            fileDataList.InsertRange(0, fixedHeader);
            return fileDataList.ToArray();
        }
    }
}