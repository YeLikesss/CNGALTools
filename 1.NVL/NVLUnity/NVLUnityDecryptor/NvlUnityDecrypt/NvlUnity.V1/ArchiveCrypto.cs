using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;

namespace NvlUnity.V1
{
    public class ArchiveCrypto
    {
        /// <summary>
        /// 代码中常量key1
        /// </summary>
        public uint ConstXorKey1 { get; set; }
        /// <summary>
        /// 代码中常量key2
        /// </summary>
        public uint ConstXorKey2 { get; set; }
        /// <summary>
        /// 代码中常量Key3
        /// </summary>
        public uint ConstXorKey3 { get; set; }

        /// <summary>
        /// 资源解密
        /// </summary>
        /// <param name="data">原数据流</param>
        /// <param name="decryptData">解密数据流</param>
        /// <param name="decryptLength">解密长度</param>
        /// <param name="archiveMemoryOffsetLow">资源偏移低32位</param>
        /// <param name="archiveMemoryOffsetHigh">资源偏移高32位</param>
        public void Decrypt(MemoryMappedViewAccessor data,MemoryMappedViewAccessor decryptData, long decryptLength, uint archiveMemoryOffsetLow = 0, uint archiveMemoryOffsetHigh = 0)
        {
            /*xorKey组   0-3常量key1 4-7常量key2 8-11常量key3  总长12字节*/
            List<byte> xorKeyBytes = new List<byte>();
            xorKeyBytes.AddRange(BitConverter.GetBytes(this.ConstXorKey1));
            xorKeyBytes.AddRange(BitConverter.GetBytes(this.ConstXorKey2));
            xorKeyBytes.AddRange(BitConverter.GetBytes(this.ConstXorKey3));

            ulong archiveMemOffset = (((ulong)archiveMemoryOffsetHigh) << 32) + archiveMemoryOffsetLow;         //资源内存偏移
            int keyIndex = (int)(archiveMemOffset % (ulong)xorKeyBytes.Count);           //计算key起始异或点

            //获取头部不对齐大小(相对key剩余部分)   
            long unAlignmentLengthResidue = Math.Min(decryptLength,(xorKeyBytes.LongCount()-keyIndex)%xorKeyBytes.LongCount());
            //设置数据索引
            long index = 0;

            //解密不对齐部分数据
            if (unAlignmentLengthResidue > 0)
            {
                //读取头部未对齐数据
                byte[] buffer = new byte[unAlignmentLengthResidue];      
                data.ReadArray(index, buffer, 0, buffer.Length);

                //异步解密
                Parallel.For(0, buffer.Length, bufferIndex => 
                {
                    buffer[bufferIndex] ^= xorKeyBytes.ElementAt(keyIndex + bufferIndex);
                });

                //写入头部已解密未对齐数据
                decryptData.WriteArray(index, buffer, 0, buffer.Length);
            }
            
            //长度不足进行下一轮解密
            if (unAlignmentLengthResidue == decryptLength)
            {
                return;
            }

            //当前数据索引为头部对齐完毕数据
            index = unAlignmentLengthResidue;
            //key索引置零
            keyIndex = 0;
            //获取循环次数
            long loopCount = (decryptLength - unAlignmentLengthResidue) / xorKeyBytes.Count;

            //下一轮数据不足key长度
            if (loopCount == 0)
            {
                //剩余不足key长度数据长度
                long endCount = decryptLength - unAlignmentLengthResidue;

                //异步解密尾部数据
                if (endCount > 0)
                {
                    //读取头部未对齐数据
                    byte[] buffer = new byte[endCount];
                    data.ReadArray(index, buffer, 0, buffer.Length);

                    //异步解密
                    Parallel.For(0, buffer.Length, bufferIndex =>
                    {
                        buffer[bufferIndex] ^= xorKeyBytes.ElementAt(keyIndex + bufferIndex);
                    });

                    //写入头部已解密未对齐数据
                    decryptData.WriteArray(index, buffer, 0, buffer.Length);
                }
            }
            else
            {
                //4字节打包key
                List<uint> xorKeyList = new List<uint>()
                {
                    this.ConstXorKey1,this.ConstXorKey2,this.ConstXorKey3
                };

                //循环异或打包4字节解密
                Parallel.For(0, loopCount, loopOffset => 
                {
                    //当前数据位置
                    long dataOffset = index + loopOffset * xorKeyBytes.Count;

                    //获取12字节数据 4字节打包
                    uint[] buffer = new uint[xorKeyBytes.Count >> 2];     
                    data.ReadArray(dataOffset, buffer, 0, buffer.Length);

                    //4字节打包异或解密
                    Parallel.For(0, buffer.Length, packed4Index =>
                    {
                        buffer[packed4Index] ^= xorKeyList[packed4Index];
                    });

                    //回写解密后数据
                    decryptData.WriteArray(dataOffset, buffer, 0, buffer.Length);
                });

                //对齐解密完毕  设置解密后数据索引
                index += (loopCount * xorKeyBytes.Count);

                //尾部未对齐数据
                long unAlignmentEnd = decryptLength - index;

                //异步解密尾部数据
                if (unAlignmentEnd > 0)
                {
                    //读取头部未对齐数据
                    byte[] buffer = new byte[unAlignmentEnd];
                    data.ReadArray(index, buffer, 0, buffer.Length);

                    //异步解密
                    Parallel.For(0, buffer.Length, bufferIndex =>
                    {
                        buffer[bufferIndex] ^= xorKeyBytes.ElementAt(keyIndex + bufferIndex);
                    });

                    //写入头部已解密未对齐数据
                    decryptData.WriteArray(index, buffer, 0, buffer.Length);
                }
            }
        }
    }
}
