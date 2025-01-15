using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VNMakerCore.General
{
    /// <summary>
    /// 加密接口
    /// </summary>
    public interface ICryptoFilter
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="block">数据块</param>
        /// <param name="offset">数据块在完整数据中偏移</param>
        /// <param name="position">数据块起始位置</param>
        /// <param name="length">数据块长度</param>
        public void Decrypt(byte[] block, long offset, int position, int length);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="block">数据块</param>
        /// <param name="offset">数据块在完整数据中偏移</param>
        public void Decrypt(in Span<byte> block, long offset);

        
    }

    /// <summary>
    /// 基本加密类
    /// </summary>
    public abstract class CryptoFilter : ICryptoFilter
    {
        public abstract string Description { get; }

        public void Decrypt(byte[] block, long offset, int position, int length)
        {
            this.Decrypt(block.AsSpan().Slice(position, length), offset);
        }

        public abstract void Decrypt(in Span<byte> block, long offset);
    }

    /// <summary>
    /// 异或加密类
    /// </summary>
    public abstract class XorFilter : CryptoFilter
    {
        /// <summary>
        /// 加密Key
        /// </summary>
        public abstract byte[] Key { get; }

        public override string Description => this.GetDescription();

        public override void Decrypt(in Span<byte> block, long offset)
        {
            byte[] key = this.Key;
            int keyLen = key.Length;
            if (keyLen != 0)
            {
                int keyIdx = (int)(offset % keyLen);
                for (int i = 0; i < block.Length; ++i)
                {
                    block[i] ^= key[keyIdx];
                    keyIdx++;
                    if (keyIdx == keyLen)
                    {
                        keyIdx = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 获取描述
        /// </summary>
        private string GetDescription()
        {
            StringBuilder sb = new(256);
            sb.Append(nameof(XorFilter));
            sb.Append(" [");

            byte[] key = this.Key;
            if (key.Any())
            {
                for (int i = 0; i < key.Length - 1; ++i)
                {
                    sb.AppendFormat("0x{0:X2}", key[i]);
                    sb.Append(", ");
                }

                sb.AppendFormat("0x{0:X2}", key.Last());
            }
            sb.Append(']');

            return sb.ToString();
        }
    }
}
