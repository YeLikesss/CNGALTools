using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extractor.ZixRenpy8V1.Renpy
{
    public class FileEntry
    {
        /// <summary>
        /// 资源偏移
        /// </summary>
        public long Offset { get; set; }
        /// <summary>
        /// 资源大小
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// 资源头
        /// </summary>
        public byte[] Header { get; set; }


        public FileEntry(object[] fileInfo)
        {

            if (fileInfo[0] is long)
            {
                this.Offset = (long)fileInfo[0];
            }
            else if (fileInfo[0] is int)
            {
                this.Offset = (int)fileInfo[0];
            }

            if (fileInfo[1] is long)
            {
                this.Size = (long)fileInfo[1];
            }
            else if (fileInfo[1] is int)
            {
                this.Size = (int)fileInfo[1];
            }

            this.Header = fileInfo[2] as byte[];
        }
    }
}
