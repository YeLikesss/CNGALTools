using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace BoneNailStatic.Package.V1
{
    /// <summary>
    /// AES-CTR模式
    /// </summary>
    public class AesCtr
    {
        /// <summary>
        /// 解密资源
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="output">输出</param>
        /// <param name="length">长度</param>
        /// <param name="key">32字节解密key</param>
        /// <param name="iv">16字节iv</param>
        public static bool Decrypt(Stream input, Stream output, long length, byte[] key, byte[] iv)
        {
            if (!input.CanRead)
            {
                return false;
            }
            if (!output.CanWrite)
            {
                return false;
            }

            long len = Math.Min(input.Length - input.Position, length);
            if (len == 0L)
            {
                return false;
            }

            byte[] counter = new byte[16];      //计数器
            byte[] block = new byte[16];        //AES上下文
            Array.Copy(iv, counter, 8);         //只取8字节

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            using ICryptoTransform enctyptor = aes.CreateEncryptor();

            Span<byte> data = stackalloc byte[16];
            long pos = 0L;
            while (pos < len)
            {
                enctyptor.TransformBlock(counter, 0, 16, block, 0);
                AesCtr.IncrementCounter(counter);

                int cnt = input.Read(data);
                for (int i = 0; i < cnt; ++i)
                {
                    data[i] ^= block[i];
                }
                output.Write(data[0..cnt]);

                pos += cnt;
            }

            return true;
        }

        /// <summary>
        /// AES计数器自增
        /// </summary>
        /// <param name="counter">计数数组</param>
        private static void IncrementCounter(byte[] counter)
        {
            for (int i = counter.Length - 1; i >= 8; i--)
            {
                if (++counter[i] != 0)
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 加密类
    /// </summary>
    public class Crypto
    {
        private readonly byte[] mKey;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="args">加密参数</param>
        public Crypto(ICryptoArguments args)
        {
            this.mKey = args.Key;
        }

        /// <summary>
        /// 生成key
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns>32字节Key</returns>
        public byte[] GenerateKey(string content)
        {
            return this.GenerateKeyInternal("bundle-key|", content);
        }

        /// <summary>
        /// 生成IV
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns>32字节IV</returns>
        public byte[] GenerateIV(string content)
        {
            return this.GenerateKeyInternal("bundle-iv|", content);
        }

        /// <summary>
        /// 生成key
        /// </summary>
        /// <param name="hdr">标识头</param>
        /// <param name="content">内容</param>
        /// <returns>32字节key</returns>
        private byte[] GenerateKeyInternal(string hdr, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("内容不能为空", nameof(content));
            }

            string sk = string.Concat(hdr, content.Trim().ToLowerInvariant());

            using HMACSHA256 mac = new(this.mKey);
            return mac.ComputeHash(Encoding.UTF8.GetBytes(sk));
        }
    }
}
