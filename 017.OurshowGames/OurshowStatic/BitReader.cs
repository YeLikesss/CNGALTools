using System;
using System.Collections.Generic;

namespace OurshowStatic
{
    /// <summary>
    /// 位读取
    /// </summary>
    public class BitReader
    {
        /// <summary>
        /// 大端读取位
        /// </summary>
        /// <param name="dest">目标</param>
        /// <param name="destByteOffset">目标字节偏移</param>
        /// <param name="destBitOffset">目标位偏移(0 - 7)</param>
        /// <param name="src">源</param>
        /// <param name="srcByteOffset">源字节偏移</param>
        /// <param name="srcBitOffset">源位偏移(0 - 7)</param>
        /// <param name="count">长度</param>
        public static void ReadBE(Span<byte> dest, int destByteOffset, int destBitOffset,
                                  ReadOnlySpan<byte> src, int srcByteOffset, int srcBitOffset, 
                                  int count)
        {
            if(count <= 0)
            {
                return;
            }

            destBitOffset %= 8;
            srcBitOffset %= 8;

            do
            {
                byte v = (byte)((src[srcByteOffset] >> (7 - srcBitOffset)) & 1);

                v <<= 7 - destBitOffset;

                dest[destByteOffset] |= v;

                ++destBitOffset;
                ++srcBitOffset;

                destByteOffset += destBitOffset / 8;
                srcByteOffset += srcBitOffset / 8;

                destBitOffset %= 8;
                srcBitOffset %= 8;
            }
            while (--count != 0);
        }

        /// <summary>
        /// 大端测试位
        /// </summary>
        /// <param name="src">源</param>
        /// <param name="srcByteOffset">源字节偏移</param>
        /// <param name="srcBitOffset">源位偏移(0 - 7)</param>
        public static bool TestBE(ReadOnlySpan<byte> src, int srcByteOffset, int srcBitOffset)
        {
            srcBitOffset %= 8;
            return ((src[srcByteOffset] >> (7 - srcBitOffset)) & 1) != 0;
        }
    }
}
