using System;

namespace LightVNStatic
{
    /// <summary>
    /// 解密V1版
    /// </summary>
    public abstract class CryptoFilterV1
    {
        /// <summary>
        /// 解密key
        /// </summary>
        public abstract byte[] Key { get; }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">完整文件数据</param>
        public virtual void Decrypt(Span<byte> data)
        {
            byte[] key = this.Key;
            int keyLen = key.Length;

            int dataLen = data.Length;

            int decLen = Math.Min(dataLen, 100);

            for(int i = 0; i < decLen; ++i)
            {
                byte k = key[i % keyLen];

                data[i] ^= k;
                data[dataLen - i - 1] ^= k;
            }
        }
    }
}