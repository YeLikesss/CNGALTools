using System;

namespace XP3Archive
{
    public interface IXP3Filter
    {
        public void Decrypt(Span<byte> data, uint hash, long offset = 0);
    }

    /// <summary>
    /// 彼岸花葬
    /// </summary>
    public class BiAnHuaZang : IXP3Filter
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
            return "彼岸花葬";
        }
    }

    /// <summary>
    /// 翡翠月
    /// </summary>
    public class JadeMoon : IXP3Filter
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
            return "翡翠月";
        }
    }

    /// <summary>
    /// 雪之本境 2010 光盘版
    /// </summary>
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

    /// <summary>
    /// 雪之本境 解境篇 光盘版
    /// </summary>
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

    /// <summary>
    /// 雪之本境Ex 光盘版
    /// </summary>
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

    /// <summary>
    /// 雾之本境 光盘版
    /// </summary>
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

    /// <summary>
    /// 雨港基隆
    /// </summary>
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

    /// <summary>
    /// 鸑鷟 镜花水月
    /// </summary>
    public class YveZhuoEP1 : IXP3Filter
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
            return "鸑鷟 镜花水月";
        }
    }

    /// <summary>
    /// 鸑鷟 橘子传
    /// </summary>
    public class YveZhuoOrange : IXP3Filter
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
            return "鸑鷟 橘子传";
        }
    }

    /// <summary>
    /// 叶之离别:若叶归尘 (Demo)
    /// </summary>
    public class LeaveSLeaveIfLeavesToDust_Demo : IXP3Filter
    {
        public void Decrypt(Span<byte> data, uint hash, long offset = 0)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                byte b = (byte)(data[i] ^ (byte)hash);

                // rol/ror reg8,4
                b = (byte)((b << 4) | (b >> 4));

                data[i] = b;
            }
        }

        public override string ToString()
        {
            return "叶之离别:若叶归尘 (Demo)";
        }
    }

    /// <summary>
    /// 雨夜
    /// </summary>
    public class Rain : IXP3Filter
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
            return "雨夜";
        }
    }

    /// <summary>
    /// 吉祥铃
    /// </summary>
    public class Ring : IXP3Filter
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
            return "吉祥铃";
        }
    }

    public class SummerInWaterDroplets : IXP3Filter
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
            return "水滴里的夏天";
        }
    }

    /// <summary>
    /// 宛若朝阳
    /// </summary>
    public class WanRuoZhaoYang : IXP3Filter
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
            return "宛若朝阳";
        }
    }

    /// <summary>
    /// 遗忘花园 光盘版
    /// </summary>
    public class ObliviousGarden : IXP3Filter
    {
        public void Decrypt(Span<byte> data, uint hash, long offset = 0)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                if ((offset + i) % 2 == 0)
                {
                    data[i] ^= 0x53;
                }
                else
                {
                    data[i] ^= 0x3A;
                }
            }
        }

        public override string ToString()
        {
            return "遗忘花园 光盘版";
        }
    }

    /// <summary>
    /// 紫罗兰-里: 水中倒影
    /// </summary>
    public class VioletInsideSummer : IXP3Filter
    {
        public void Decrypt(Span<byte> data, uint hash, long offset = 0)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                if ((offset + i) % 2 == 0)
                {
                    data[i] ^= 0x3D;
                }
                else
                {
                    data[i] ^= 0xE5;
                }
            }
        }

        public override string ToString()
        {
            return "紫罗兰-里: 水中倒影";
        }
    }
}
