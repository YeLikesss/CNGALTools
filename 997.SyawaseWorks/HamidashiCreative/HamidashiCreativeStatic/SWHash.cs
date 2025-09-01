using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;

namespace HamidashiCreativeStatic
{
    /// <summary>
    /// Hash类
    /// </summary>
    public class SWHash
    {
        /// <summary>
        /// 64位Hash
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>Hash值</returns>
        public static ulong Hash64(ReadOnlySpan<byte> data)
        {
            int length = data.Length;
            if(length == 0)
            {
                return 0;
            }

            int alignPack8Length = (((length - 1) >> 3) + 1) << 3;

            byte[] buf = ArrayPool<byte>.Shared.Rent(alignPack8Length);
            Span<byte> block = buf.AsSpan().Slice(0, alignPack8Length);

            block.Clear();
            data.CopyTo(block);

            ulong hash = 0x679318571558439D * (ulong)length;
            {
                Span<ulong> blockPack8 = MemoryMarshal.Cast<byte, ulong>(block);

                for(int i = 0; i < blockPack8.Length; ++i)
                {
                    ulong v0 = hash ^ blockPack8[i];

                    ulong v2 = Math.BigMul(v0, 0x1A6EC39A279322C7, out ulong v1);

                    hash = v2 ^ v1;
                }
            }
            
            ArrayPool<byte>.Shared.Return(buf);
            return hash;
        }
    }
}
