using System.IO;
using System.Text;
using System;

namespace EngineCoreStatic
{
    /// <summary>
    /// HAC流读写扩展
    /// </summary>
    public class HACStreamExtend
    {
        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <returns>字符串</returns>
        public unsafe static string ReadString(Stream stream)
        {
            string s = string.Empty;

            int strLen = 0;
            if (stream.Read(new Span<byte>(&strLen, sizeof(int))) == sizeof(int))
            {
                if (strLen > 0)
                {
                    byte[] buf = new byte[strLen * 2];
                    if (stream.Read(buf) == buf.Length)
                    {
                        s = Encoding.Unicode.GetString(buf);
                    }
                }
            }

            return s;
        }

        /// <summary>
        /// 读取字节序列
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <returns>字节序列</returns>
        public unsafe static byte[] ReadBytes(Stream stream)
        {
            byte[] data = Array.Empty<byte>();

            int bytesLen = 0;
            if (stream.Read(new Span<byte>(&bytesLen, sizeof(int))) == sizeof(int))
            {
                if (bytesLen > 0)
                {
                    data = new byte[bytesLen];
                    stream.Read(data);
                }
            }

            return data;
        }
    }
}
