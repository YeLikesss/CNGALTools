using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace EngineCore
{
    public class QuickLZ
    {
        /// <summary>
        /// Lz4解压
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static byte[] Decompress(Span<byte> data)
        {
            bool isCompress = (data[0] & 1) == 1;

            int compressSize = BitConverter.ToInt32(data.Slice(1, 4));
            int uncompressSize = BitConverter.ToInt32(data.Slice(5, 4));

            byte[] dest = new byte[uncompressSize];

            if (isCompress)
            {
                Decompress_Unsafe(data.Slice(9), dest);
            }
            else
            {
                data.Slice(9).CopyTo(dest);
            }

            return dest;
        }

        private unsafe static void Decompress_Unsafe(Span<byte> compressed, Span<byte> decompressed)
        {
            fixed (byte* ptr = &compressed[0])
            {
                fixed(byte* ptr2 = &decompressed[0])
                {
                    int rawSize = decompressed.Length;

                    int num = 0;
                    int i = 0;
                    uint num2 = 1U;
                    int num3 = rawSize - 6 - 4 - 1;
                    uint num4 = 0U;
                    for (; ; )
                    {
                        if (num2 == 1U)
                        {
                            num2 = new Span<uint>(ptr + num, 4)[0];
                            num += 4;
                            if (i <= num3)
                            {
                                num4 = new Span<uint>(ptr + num, 4)[0];
                            }
                        }
                        if ((num2 & 1U) == 1U)
                        {
                            num2 >>= 1;
                            uint num5;
                            uint num6;
                            if ((num4 & 3U) == 0U)
                            {
                                num5 = (num4 & 255U) >> 2;
                                num6 = 3U;
                                num++;
                            }
                            else if ((num4 & 2U) == 0U)
                            {
                                num5 = (num4 & 65535U) >> 2;
                                num6 = 3U;
                                num += 2;
                            }
                            else if ((num4 & 1U) == 0U)
                            {
                                num5 = (num4 & 65535U) >> 6;
                                num6 = ((num4 >> 2) & 15U) + 3U;
                                num += 2;
                            }
                            else if ((num4 & 127U) != 3U)
                            {
                                num5 = (num4 >> 7) & 131071U;
                                num6 = ((num4 >> 2) & 31U) + 2U;
                                num += 3;
                            }
                            else
                            {
                                num5 = num4 >> 15;
                                num6 = ((num4 >> 7) & 255U) + 3U;
                                num += 4;
                            }
                            uint num7 = (uint)((long)i - (long)((ulong)num5));
                            ptr2[i] = ptr2[num7];
                            (ptr2 + i)[1] = (ptr2 + num7)[1];
                            (ptr2 + i)[2] = (ptr2 + num7)[2];
                            int num8 = 3;
                            while ((long)num8 < (long)((ulong)num6))
                            {
                                (ptr2 + i)[num8] = (ptr2 + num7)[num8];
                                num8++;
                            }
                            i += (int)num6;
                            num4 = new Span<uint>(ptr + num, 4)[0];
                        }
                        else
                        {
                            if (i > num3)
                            {
                                break;
                            }
                            ptr2[i] = ptr[num];
                            i++;
                            num++;
                            num2 >>= 1;
                            num4 = (uint)((((int)num4 >> 8) & 65535) | ((int)(ptr + num)[2] << 16) | ((int)(ptr + num)[3] << 24));
                        }
                    }
                    while (i <= rawSize - 1)
                    {
                        if (num2 == 1U)
                        {
                            num += 4;
                            num2 = 2147483648U;
                        }
                        ptr2[i] = ptr[num];
                        i++;
                        num++;
                        num2 >>= 1;
                    }
                }
            }
            

        }
    }
}
