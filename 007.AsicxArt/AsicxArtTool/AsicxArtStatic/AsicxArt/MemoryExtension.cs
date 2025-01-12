using System;
using System.Collections.Generic;
using System.Text;

namespace AsicxArt
{
    /// <summary>
    /// 内存扩展
    /// </summary>
    internal static class MemoryExtension
    {
        /// <summary>
        /// 获取Unicode字符串
        /// </summary>
        /// <param name="ptr">字符指针 null结尾</param>
        public unsafe static string AsUnicodeString(this IntPtr ptr)
        {
            if(ptr == IntPtr.Zero)
            {
                return string.Empty;
            }
            return new((char*)ptr);
        }
    }
}
