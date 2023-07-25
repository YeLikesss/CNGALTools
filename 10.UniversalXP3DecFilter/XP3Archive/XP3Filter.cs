using System;

namespace XP3Archive
{
    public interface IXP3Filter
    {
        public void Decrypt(Span<byte> data, uint hash, long offset = 0);
    }

    public class ConspiracyFieldSnowTrapCh1 : IXP3Filter
    {
        public void Decrypt(Span<byte> data, uint hash, long offset = 0)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = (byte)~(data[i] ^ ~hash);
            }
        }
        public override string ToString()
        {
            return "雪之本境 2010 光盘版";
        }
    }

    public class ConspiracyFieldSnowTrapCh2 : IXP3Filter
    {
        public void Decrypt(Span<byte> data, uint hash, long offset = 0)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] ^= (byte)(hash >> 3);
            }
        }
        public override string ToString()
        {
            return "雪之本境 解境篇 光盘版";
        }
    }

    public class ConspiracyFieldSnowTrapEx : IXP3Filter
    {
        public void Decrypt(Span<byte> data, uint hash, long offset = 0)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] ^= (byte)(hash >> 3);
            }
        }
        public override string ToString()
        {
            return "雪之本境Ex 光盘版";
        }
    }

    public class ConspiracyFieldFogShadow : IXP3Filter
    {
        public void Decrypt(Span<byte> data, uint hash, long offset = 0)
        {
            for(int i = 0; i < data.Length; ++i)
            {
                if ((offset + i) % 2 == 0)
                {
                    data[i] ^= 0x5B;
                }
                else
                {
                    data[i] ^= 0x39;
                }
            }
        }
        public override string ToString()
        {
            return "雾之本境 光盘版";
        }
    }

    public class TheRainyPortKeelung : IXP3Filter
    {
        public void Decrypt(Span<byte> data, uint hash, long offset = 0)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = (byte)~(data[i] ^ (hash + 1));
            }
        }
        public override string ToString()
        {
            return "雨港基隆";
        }
    }
}
