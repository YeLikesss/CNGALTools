using System;
using System.IO;
using System.Text;


namespace NekoNovelStatic
{
    /// <summary>
    /// 流扩展
    /// </summary>
    internal class StreamExtend
    {
        /// <summary>
        /// 读取UTF8字符串
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <returns></returns>
        public unsafe static string ReadUTF8String(Stream stream)
        {
            uint length = 0u;
            stream.Read(new Span<byte>(&length, 4));

            if(length == 0u)
            {
                return string.Empty;
            }

            byte[] data = new byte[length];
            stream.Read(data);

            return Encoding.UTF8.GetString(data);
        }
    }
}
