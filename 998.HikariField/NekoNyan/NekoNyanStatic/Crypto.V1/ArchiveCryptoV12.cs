using System;
using System.Collections.Generic;
using System.Text;

namespace NekoNyanStatic.Crypto.V1
{
    internal class ArchiveCryptoV12 : ArchiveCryptoV10
    {
        protected override void Decrypt(Span<byte> data, uint key)
        {
            Span<byte> table = stackalloc byte[256];
            this.KeyGenerator(table, key);
            for (int i = 0; i < data.Length; ++i)
            {
                byte temp = data[i];
                temp ^= table[i % 253];
                temp += table[i % 59];
                temp ^= 0x99;
                data[i] = temp;
            }
        }

    }
}
