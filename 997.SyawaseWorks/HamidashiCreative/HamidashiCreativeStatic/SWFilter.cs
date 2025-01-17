using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HamidashiCreativeStatic
{
    /// <summary>
    /// 加密接口
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">输入</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        public void Decrypt(byte[] data, long offset, int length);
    }

    /// <summary>
    /// 加密类
    /// </summary>
    public class SWFilterV1 : IFilter
    {
        private readonly uint mKey;
        public void Decrypt(byte[] data, long offset, int length)
        {
            Span<byte> key = stackalloc byte[4];
            BitConverter.TryWriteBytes(key, this.mKey);

            int keyIdx = (int)(offset % 4);

            for(int i = 0; i < length; ++i)
            {
                data[i] ^= key[keyIdx];

                keyIdx++;
                if (keyIdx == 4)
                {
                    keyIdx = 0;
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">解密key</param>
        public SWFilterV1(uint key)
        {
            this.mKey = key;
        }
    }
}
