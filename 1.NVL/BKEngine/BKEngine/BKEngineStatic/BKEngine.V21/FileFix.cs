using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BKEngine
{
    /// <summary>
    /// 文件修复
    /// </summary>
    public class FileFix
    {
        /// <summary>
        /// BZip2文件头
        /// </summary>
        private static byte[] fixedHeader = new byte[] 
        { 
            0x42, 0x5A 
        };
        /// <summary>
        /// 修复文件资源的BZip2文件头
        /// </summary>
        /// <param name="filedata">文件数据流</param>
        /// <returns></returns>
        public static void  CompressedResourcesFix(List<List<byte>> filedata)
        {
            filedata.ForEach(sfileData => 
            {
                //插入文件头
                sfileData.InsertRange(0, fixedHeader);
            });
        }
    }
}
