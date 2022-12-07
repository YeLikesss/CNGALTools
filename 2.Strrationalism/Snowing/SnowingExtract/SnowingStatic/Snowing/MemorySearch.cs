using System;
using System.Collections.Generic;
using System.Linq;

namespace Snowing
{
    /// <summary>
    /// 内存查找相关
    /// </summary>
    public class MemorySearch
    {
        /// <summary>
        /// 查找byte数组第一个匹配项
        /// </summary>
        /// <param name="source">被查找数组</param>
        /// <param name="offset">起始偏移</param>
        /// <param name="pattern">查找参数</param>
        /// <returns>第一个匹配项对应索引</returns>
        public static int IndexOfFirst(byte[] source,int offset,byte[] pattern)
        {
            for(int i = offset; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
