using System;

namespace OpaiStatic.Filter
{
    public abstract class XorFilterBase : IFilter
    {
        /// <summary>
        /// 解密Key
        /// </summary>
        public virtual byte[] Key { get; } = Array.Empty<byte>();

        public void Decrypt(Span<byte> data, long blockOffset)
        {
            byte[] key = this.Key;
            int keyLen = key.Length;
            if (keyLen != 0)
            {
                int keyIdx = (int)(blockOffset % keyLen);
                for(int i = 0; i < data.Length; ++i)
                {
                    data[i] ^= key[keyIdx];
                    keyIdx++;
                    if (keyIdx == keyLen)
                    {
                        keyIdx = 0;
                    }
                }
            }
        }
    }
}
