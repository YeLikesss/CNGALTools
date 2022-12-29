using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractor.ZixRenpy8V1.Renpy
{
    public interface IRPAExtractor
    {
        /// <summary>
        /// 提取资源
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <param name="extractPath">导出全路径</param>
        public void Extract(string filePath, string extractPath);
    }
}
