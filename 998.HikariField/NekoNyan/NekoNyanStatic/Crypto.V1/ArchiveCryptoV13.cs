using System;
using System.Collections.Generic;
using System.Text;

namespace NekoNyanStatic.Crypto.V1
{
    internal class ArchiveCryptoV13 : ArchiveCryptoV11
    {
        protected override void KeyGenerator(Span<byte> tablePtr, uint key)
        {
            uint k1 = key * 0x00001704u + 0x0000A140u;
            uint k2 = k1 << 0x07 ^ k1;

            for (int i = 0; i < 256; ++i)
            {
                k1 = k1 - key + k2;
                k2 = k1 + 0x155u;
                k1 *= k2 & 0xDCu;
                tablePtr[i] = (byte)k1;
                k1 >>= 2;
            }
        }

        protected override void Decrypt(Span<byte> data, uint key)
        {
            Span<byte> table = stackalloc byte[256];
            this.KeyGenerator(table, key);
            for (int i = 0; i < data.Length; ++i)
            {
                byte temp = data[i];
                temp ^= table[i % 235];
                temp += 0x1F;
                temp += table[i % 87];
                temp ^= 0xA5;
                data[i] = temp;
            }
        }
    }
}
