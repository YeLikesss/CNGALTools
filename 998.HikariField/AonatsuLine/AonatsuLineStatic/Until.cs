using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AonatsuLineStatic
{
    public class Until
    {
        /// <summary>
        /// 尝试查找文件类型
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="fileName">文件名</param>
        /// <returns>新的文件名</returns>
        public static string TryDetectFileType(Span<byte> data,string fileName)
        {
            if (data.Length < 8)
            {
                return fileName;
            }

            if (BitConverter.ToUInt64(data.Slice(0, 8)) == 0x0053467974696E55)
            {
                return fileName + ".asset";
            }

            if (BitConverter.ToUInt32(data.Slice(0, 4)) == 0x204D5053)
            {
                return fileName + ".spm";
            }

            return fileName;

        }
    }
}
