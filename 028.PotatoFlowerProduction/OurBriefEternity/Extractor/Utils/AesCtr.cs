using System;
using System.Security.Cryptography;

namespace Extractor.Utils
{
    internal class AesCtr
    {
        /// <summary>
        /// 解密资源
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">解密key</param>
        /// <param name="nonce">12字节iv</param>
        public static void Decrypt(byte[] data, byte[] key, byte[] nonce)
        {
            if (data.LongLength == 0L)
            {
                return;
            }

            byte[] counter = new byte[16];
            byte[] block = new byte[16];
            Array.Copy(nonce, counter, 12);

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            using ICryptoTransform enctyptor = aes.CreateEncryptor();

            long position = 0L;
            while (position < data.LongLength)
            {
                enctyptor.TransformBlock(counter, 0, 16, block, 0);
                AesCtr.IncrementCounter(counter);

                long cnt = Math.Min(16L, data.LongLength - position);
                for(long i = 0L; i < cnt; ++i)
                {
                    data[position + i] ^= block[i];
                }
                position += cnt;
            }
        }

        /// <summary>
        /// AES计数器自增
        /// </summary>
        /// <param name="counter">计数数组</param>
        private static void IncrementCounter(byte[] counter)
        {
            for (int i = counter.Length - 1; i >= 0; i--)
            {
                if (++counter[i] != 0)
                { 
                    break;
                }
            }
        }
    }
}
