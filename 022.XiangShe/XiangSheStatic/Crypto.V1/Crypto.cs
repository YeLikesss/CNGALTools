using System;
using System.Text;
using System.Security.Cryptography;

namespace XiangSheStatic.Crypto.V1
{
    /// <summary>
    /// Hash模式
    /// </summary>
    public enum HashMode
    {
        MD5,
        SHA1,
        SHA256,
        SHA512,
    }

    /// <summary>
    /// 算法类型
    /// </summary>
    [Flags]
    public enum AlgorithmType : uint
    {
        /// <summary>
        /// 不加密
        /// </summary>
        None = 0x00000000u,
        /// <summary>
        /// AES ECB加密
        /// </summary>
        AESECB = 0x00000001u,
        /// <summary>
        /// AES CFB加密
        /// </summary>
        AESCFB = 0x00000002u,
        /// <summary>
        /// Gzip压缩
        /// </summary>
        GZip = 0x00000004u,
        /// <summary>
        /// XOR 加密
        /// </summary>
        XOR = 0x00000008u,
    }

    /// <summary>
    /// 加密类
    /// </summary>
    public class Crypto
    {
        /// <summary>
        /// 计算Hash
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="mode">模式</param>
        /// <returns>Hash值</returns>
        public static byte[] HashBytes(byte[] data, HashMode mode)
        {
            switch (mode)
            {
                case HashMode.MD5:
                {
                    return MD5.HashData(data);
                }
                case HashMode.SHA256:
                {
                    return SHA256.HashData(data);
                }
                case HashMode.SHA512:
                {
                    return SHA512.HashData(data);
                }
                default:
                {
                    return Array.Empty<byte>();
                }
            }
        }

        /// <summary>
        /// AES ECB解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">解密key</param>
        /// <returns>解密后数据</returns>
        public static byte[] AESDecryptECB(byte[] data, string key)
        {
            using Aes aes = Aes.Create();

            aes.Key = Crypto.HashBytes(Encoding.UTF8.GetBytes(key), HashMode.SHA256);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            return aes.DecryptEcb(data, PaddingMode.PKCS7);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="salt"></param>
        /// <returns>解密后数据</returns>
        public static byte[] AESDecryptCFB(byte[] data, string key, byte[] salt)
        {
            using PasswordDeriveBytes pw = new(key, salt);
            using Aes aes = Aes.Create();

            aes.Key = pw.GetBytes(16);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            Span<byte> seed = stackalloc byte[16];
            Span<byte> xorKey = stackalloc byte[16];

            long position = 0L;
            long length = data.LongLength;

            byte[] dest = new byte[length];

            //CFB解密
            long blockPos = 0L;
            do
            {
                ++blockPos;
                BitConverter.TryWriteBytes(seed, blockPos);

                aes.EncryptEcb(seed, xorKey, PaddingMode.None);

                int procLen = (int)Math.Min(length - position, 16L);
                for(int i = 0; i < procLen; ++i)
                {
                    dest[position] = (byte)(data[position] ^ xorKey[i]);
                    ++position;
                }
            }
            while (position < length);

            return dest;
        }

        /// <summary>
        /// XOR解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">key</param>
        public static void XORDecrypt(byte[] data, string key)
        {
            byte[] xorKey = Encoding.UTF8.GetBytes(key);

            for(long i = 0; i < data.LongLength; ++i)
            {
                byte k = xorKey[i % xorKey.LongLength];
                data[i] ^= (byte)(k ^ (i % k));
            }
        }
    }
}
