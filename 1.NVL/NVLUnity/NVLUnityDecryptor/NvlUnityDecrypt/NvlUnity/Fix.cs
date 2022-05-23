using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.MemoryMappedFiles;

namespace NvlUnity
{
    public class Fix
    {
        /// <summary>
        /// 修复UnityFs头
        /// </summary>
        /// <param name="data">原数据</param>
        /// <param name="version">Unity版本</param>
        public static void UnityFSHeader(MemoryMappedViewAccessor data,ArchiveHeader.UnityVersion version)
        {
            //查找第一个版本匹配项
            byte[] header = ArchiveHeader.UnityHeaderList.Where(ver => ver.Key == version).Select(ver => ver.Value).First();

            //替换数据
            switch (version)
            {
                case ArchiveHeader.UnityVersion.V2018_4_0_65448:
                    data.WriteArray(0, header, 0, header.Length);

                    break;
                case ArchiveHeader.UnityVersion.V2018_4_26_44060:
                    data.WriteArray(0, header, 0, header.Length);

                    break;
            }
        }
    }
}
