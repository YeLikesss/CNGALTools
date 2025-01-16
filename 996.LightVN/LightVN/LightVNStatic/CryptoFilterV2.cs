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
                UInt128 v2 = v1;   
                UInt128 v3 = 0x47AE147AE147AE15ul;
                ulong v4 = (ulong)((v2 * v3) >> 64);
                ulong v5 = ((((v1 - v4) >> 1) + v4) >> 4) * 25ul;
                ulong v6 = v1 - v5;

                byte k = key[v6];

                if (i != 0)
                {
                    data[dataLen - i] ^= k;
                }
                data[i] ^= k;
            }
        }
    }
}
