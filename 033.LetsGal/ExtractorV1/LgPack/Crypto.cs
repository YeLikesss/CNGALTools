using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;

namespace Extractor.LgPack
{
    /// <summary>
    /// 加密类
    /// </summary>
    public class Crypto
    {
        private static readonly byte[] smTable = new byte[0x40]
        {
            0xA5, 0xF8, 0xC1, 0x03, 0x9E, 0x7B, 0x2D, 0x60, 0x45, 0xA9, 0xC3, 0xE1, 0x8F, 0x7B, 0x0D, 0x2C,
            0x9B, 0x2E, 0x5F, 0x81, 0xC4, 0xA7, 0xD0, 0x36, 0x2E, 0x9F, 0x1B, 0x85, 0x74, 0xC6, 0xA3, 0xD0,
            0x3F, 0x7C, 0x1A, 0x9E, 0x5D, 0x2B, 0x80, 0x64, 0xF1, 0xE3, 0xA7, 0xC9, 0x0B, 0x4D, 0x6E, 0x82,
            0x5F, 0x28, 0xE1, 0xB9, 0x74, 0x0C, 0x6A, 0x3D, 0x18, 0xE5, 0xF2, 0xB9, 0x0C, 0x7A, 0x4D, 0x63
        };

        /// <summary>
        /// 生成key
        /// </summary>
        /// <param name="input">输入</param>
        public unsafe static byte[] GenerateKey(byte[] input)
        {
            byte[] ctx = new byte[0x20];
            fixed(byte* table = Crypto.smTable)
            {
                Vector128<byte> s0 = Avx2.LoadVector128(table + 0x00u);
                Vector128<byte> s1 = Avx2.LoadVector128(table + 0x10u);
                Vector128<byte> s2 = Avx2.LoadVector128(table + 0x20u);
                Vector128<byte> s3 = Avx2.LoadVector128(table + 0x30u);

                Vector128<byte> xmm0 = Avx2.Xor(s1, s2);
                Vector128<byte> xmm1 = Avx2.Xor(s0, s3);

                MemoryMarshal.Write(ctx.AsSpan(0x00, 0x10), ref xmm0);
                MemoryMarshal.Write(ctx.AsSpan(0x10, 0x10), ref xmm1);
            }
            for (int i = 0; i < ctx.Length; ++i)
            {
                ctx[i] ^= input[i % input.Length];
            }
            byte[] skey = Encoding.UTF8.GetBytes("lgpak/v1/native");

            using SHA256 sha256 = SHA256.Create();

            //首次
            sha256.TransformBlock(ctx, 0, ctx.Length, null, 0);
            sha256.TransformBlock(skey, 0, skey.Length, null, 0);
            sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            Array.Copy(sha256.Hash!, ctx, ctx.Length);

            for(int i = 0; i < 4096; ++i)
            {
                sha256.Initialize();
                byte[] hash = sha256.ComputeHash(ctx);
                Array.Copy(hash, ctx, ctx.Length);
            }

            return ctx;
        }
    }
}
