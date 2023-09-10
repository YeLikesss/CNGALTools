using System;
using System.Security.Cryptography;

namespace NVLWebStatic
{
    public class Crypto
    {
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">key</param>
        /// <param name="offset">偏移</param>
        public static void Decrypt(Span<byte> data, Span<byte> key, int offset = 0)
        {
            int keyLen = key.Length;
            int keyIndex = offset % keyLen;
            int dataLen = data.Length;

            for(int i = 0; i < dataLen; ++i)
            {
                data[i] ^= key[keyIndex];

                ++keyIndex;

                if (keyIndex == keyLen)
                {
                    keyIndex = 0;
                }
            }
        }

        /// <summary>
        /// AES128解密 CFB128模式 Padding=None
        /// <para>该函数只是兼容Windows 7 在Windows 7下可用</para>
        /// <para>.Net API Windows7下不支持CFB128模式</para>
        /// <para>如使用高版本系统 可以替换成.Net API 内置的CFB解密</para>
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">16字节Key</param>
        /// <param name="iv">16字节IV</param>
        public static void AES128CFB128Decrypt(Span<byte> data, int length, byte[] key, byte[] iv)
        {
            int dataLen = length;
            int dataPos = 0;

            Span<byte> input = stackalloc byte[16];
            Span<byte> output = stackalloc byte[16];

            Aes aes = Aes.Create();
            aes.Key = key;

            //第一次使用IV
            iv.CopyTo(input);

            while (dataLen > dataPos)
            {
                aes.EncryptEcb(input, output, PaddingMode.None);

                int blockLen = Math.Min(dataLen - dataPos, 16);

                //使用带解密密文作为输入
                data.Slice(dataPos, blockLen).CopyTo(input);

                for(int i = 0; i < blockLen; ++i)
                {
                    data[dataPos + i] ^= output[i];
                }
                dataPos += blockLen;
            }
        }

    }
}