using System;
using System.Collections.Generic;
using System.Linq;

namespace NvlKr2Extract.V2
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
        /// 资源解密
        /// </summary>
        /// <param name="data">数据流</param>
        /// <param name="fileKey">文件资源key</param>
        /// <param name="decryptLength">解密长度</param>
        /// <param name="archiveMemoryOffsetLow">资源偏移低32位</param>
        /// <param name="archiveMemoryOffsetHigh">资源偏移高32位</param>
        public void Decrypt(byte[] data,uint fileKey,int decryptLength,uint archiveMemoryOffsetLow = 0,uint archiveMemoryOffsetHigh = 0)
        {
            /*xorKey组   0-3文件key 4-7常量key1 8-11常量key2  总长12字节*/
            List<byte> xorKeyBytes = new List<byte>();
            xorKeyBytes.AddRange(BitConverter.GetBytes(fileKey));
            xorKeyBytes.AddRange(BitConverter.GetBytes(this.ConstXorKey1));
            xorKeyBytes.AddRange(BitConverter.GetBytes(this.ConstXorKey2));

            ulong archiveMemOffset = (((ulong)archiveMemoryOffsetHigh) << 32) + archiveMemoryOffsetLow;         //资源内存偏移
            int keyIndex = (int)(archiveMemOffset % (ulong)xorKeyBytes.Count);           //计算key起始异或点

            int xorLength = Math.Min(data.Length, decryptLength);       //xor长度取最小

            //循环异或解密
            for(int dataIndex = 0; dataIndex < xorLength; dataIndex++)
            {
                data[dataIndex] ^= xorKeyBytes.ElementAt(keyIndex);     //异或解密
                keyIndex++;                 //key索引+1
                //检查key索引是否在key数组长度内
                if (keyIndex >= xorKeyBytes.Count)
                {
                    keyIndex = 0;
                }
            }

        }
    }
}
