using System;
using System.Collections.Generic;

namespace EngineCore
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Until
    {
        /// <summary>
        /// 尝试查找文件类型
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="fileName">文件名</param>
        /// <returns>新的文件名</returns>
        public static string TryDetectFileType(Span<byte> data, string fileName)
        {
            if (data.Length < 8)
            {
                return fileName;
            }

            if (BitConverter.ToUInt64(data[..8]) == 0x0053467974696E55ul)
            {
                return fileName + ".asset";
            }

            if (BitConverter.ToUInt32(data[..4]) == 0x204D5053u)
            {
                return fileName + ".spm";
            }

            return fileName;
        }
    }
}
