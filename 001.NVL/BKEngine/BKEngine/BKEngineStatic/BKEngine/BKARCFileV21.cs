using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BKEngine
{
    /// <summary>
    /// V21版加密
    /// </summary>
    public class BKARCFileV21 : BKARCFileV20
    {
        protected override string EntryReadFileName(Stream entryStream)
        {
            uint hash = 0;
            unsafe
            {
                entryStream.Read(new(&hash, sizeof(uint)));
            }
            return hash.ToString("X8");
        }

        /// <summary>
        /// 文本Hash
        /// </summary>
        /// <param name="s">文本字符串</param>
        /// <returns></returns>
        public static uint StringHash(string s)
        {
            ReadOnlySpan<char> str = s.AsSpan();
            uint result = 0x811C9DC5;
            for (int i = 0; i < str.Length; ++i)
            {
                result = (result ^ str[i]) * 0x01000193;
            }
            return result;
        }
    }
}
