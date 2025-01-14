using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OurshowStatic
{
    /// <summary>
    /// LZ77压缩
    /// </summary>
    public class CompressLZ77
    {
        /// <summary>
        /// 解压
        /// <para>警告: 仅完成算法还原 未进行动态测试 现有游戏暂未遇到压缩文件</para>
        /// </summary>
        /// <param name="src">源</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <param name="decompressSize">解压后长度</param>
        /// <returns>解压后数据</returns>
        public static byte[] Decompress(byte[] src, uint offset, uint length, uint decompressSize)
        {
            byte[] outBuf = new byte[decompressSize];
            byte[] tempBuf = new byte[0xC000];

            uint destPos = 0;
            while (length > 0 && decompressSize > destPos)
            {
                uint blockSize = 0xC000;
                ushort chunkSize = BitConverter.ToUInt16(src, (int)offset);

                offset += 2;
                length -= 2;

                CompressLZ77.Decode(tempBuf, src, (int)offset, chunkSize);
                length -= chunkSize;

                if (destPos + blockSize >= decompressSize)
                {
                    blockSize = decompressSize - destPos;
                }
                Array.Copy(tempBuf, 0, outBuf, destPos, blockSize);

                destPos += blockSize;
            }
            return outBuf;
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="dest">输出</param>
        /// <param name="src">输入</param>
        /// <param name="offset">输入偏移</param>
        /// <param name="length">输入长度</param>
        private static void Decode(byte[] dest, byte[] src, int offset, int length)
        {
            static void MoveBit(ref int byteOffset, ref int bitOffset, int offset)
            {
                bitOffset += offset;
                byteOffset += bitOffset / 8;
                bitOffset %= 8;
            }

            Span<byte> srcPtr = src.AsSpan().Slice(offset, length);
            Span<byte> destPtr = dest;

            int srcBytePos = 0;
            int srcBitPos = 0;

            int destPos = 0;

            while (srcBytePos < length)
            {
                bool flag;
                flag = BitReader.TestBE(srcPtr, srcBytePos, srcBitPos);
                MoveBit(ref srcBytePos, ref srcBitPos, 1);
                if (flag)
                {
                    int dataLenBits = -1;
                    do
                    {
                        flag = BitReader.TestBE(srcPtr, srcBytePos, srcBitPos);
                        MoveBit(ref srcBytePos, ref srcBitPos, 1);
                        ++dataLenBits;
                    }
                    while (flag);

                    int dataLen;
                    if (dataLenBits > 0)
                    {
                        int v = 0;
                        Span<byte> vptr = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref v, 1));

                        BitReader.ReadBE(vptr, (32 - dataLenBits) / 8, (32 - dataLenBits) % 8, srcPtr, srcBytePos, srcBitPos, dataLenBits);
                        MoveBit(ref srcBytePos, ref srcBitPos, dataLenBits);

                        vptr.Reverse();

                        dataLen = (1 << dataLenBits) + v + 1;
                    }
                    else
                    {
                        dataLen = 2;
                    }

                    int dataBits = 0;
                    if (destPos > 0)
                    {
                        int v = 1;
                        while (v < destPos)
                        {
                            v *= 2;
                            ++dataBits;
                        }
                    }
                    else
                    {
                        dataBits = -1;
                    }

                    {
                        int v = 0;
                        Span<byte> vptr = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref v, 1));

                        BitReader.ReadBE(vptr, (32 - dataBits) / 8, (32 - dataBits) % 8, srcPtr, srcBytePos, srcBitPos, dataBits);
                        MoveBit(ref srcBytePos, ref srcBitPos, dataBits);

                        vptr.Reverse();

                        for(int i = 0; i < dataLen; ++i)
                        {
                            destPtr[destPos + i] = destPtr[v + i];
                        }
                        destPos += v;
                    }
                }
                else
                {
                    BitReader.ReadBE(destPtr, destPos, 0, srcPtr, srcBytePos, srcBitPos, 8);
                    MoveBit(ref srcBytePos, ref srcBitPos, 8);
                    ++destPos;
                }
            }
        }
    }
}
