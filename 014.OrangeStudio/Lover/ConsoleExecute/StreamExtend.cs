using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleExecute
{
    /// <summary>
    /// 流扩展
    /// </summary>
    internal class StreamExtend
    {
        /// <summary>
        /// 读取结构
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="s">流</param>
        /// <returns>返回值</returns>
        public static T Read<T>(Stream s) where T : struct
        {
            Span<byte> buf = stackalloc byte[Unsafe.SizeOf<T>()];
            s.Read(buf);
            return MemoryMarshal.Read<T>(buf);
        }
    }
}
