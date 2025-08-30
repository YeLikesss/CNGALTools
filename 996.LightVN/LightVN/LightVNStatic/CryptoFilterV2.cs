using System;

namespace LightVNStatic
{
    /// <summary>
    /// 解密V2版
    /// </summary>
    public abstract class CryptoFilterV2 
    {
        /// <summary>
        /// 解密key
        /// </summary>
        public abstract byte[] Key { get; }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">完整文件数据</param>
        /// <param name="decLength">解码长度 (-1 完整长度)</param>
        public virtual void Decrypt(Span<byte> data, int decLength)
        {
            byte[] key = this.Key;
            int dataLen = data.Length;

            int decLen;
            if (decLength == -1)
            {
                decLen = dataLen;
            }
            else
            {
                decLen = Math.Min(dataLen, decLength);
            }

            for(int i = 0; i < decLen; ++i)
            {
                ulong v1 = (ulong)(long)i;           //movsxd reg64, reg32
                ulong v3 = Math.BigMul(v1, 0x47AE147AE147AE15ul, out ulong _);   //rdx:rax = mul reg64 (rax)
                ulong v4 = ((((v1 - v3) >> 1) + v3) >> 4) * 25ul;
                ulong v5 = v1 - v4;

                byte k = key[v5];

                if (i != 0)
                {
                    data[dataLen - i] ^= k;
                }
                data[i] ^= k;
            }
        }
    }
}
