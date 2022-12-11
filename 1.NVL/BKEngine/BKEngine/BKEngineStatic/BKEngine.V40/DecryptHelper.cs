using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Text;

namespace BKEngine.V40
{
    public class DecryptHelper 
    { 

        
        public static uint ConstKey1 => 0x00FFFF00;
        public static uint ConstKey2 => 0x6C078965;
        public static uint ConstKey3 => 0x5E89F12A;
        public static uint ConstKey4 => 0x5D588B65;
        public static uint ConstKey5 => 0x01010101;

        /// <summary>
        /// 计算文件表Key组的偏移
        /// </summary>
        /// <param name="fileHeader">文件头</param>
        /// <returns>文件目录表解密Key偏移</returns>
        public static uint DecryptTableKeyGroupFileOffset(FileHeader fileHeader)
        {
            uint offsetKey = fileHeader.OffsetKey1;         
            offsetKey &= DecryptHelper.ConstKey1;           
            offsetKey ^= fileHeader.OffsetKey3;            
            offsetKey -= DecryptHelper.ConstKey2;          
            offsetKey ^= fileHeader.OffsetKey2;            
            offsetKey -= DecryptHelper.ConstKey3;         
            offsetKey ^= fileHeader.OffsetKey1;
            offsetKey += 0x10;
            return offsetKey;
        }

        /// <summary>
        /// 获取文件表压缩包的大小
        /// </summary>
        /// <param name="fileHeader">文件头</param>
        /// <param name="tableKeyGroup">文件表key组</param>
        /// <param name="uncompressedLength">解压后长度</param>
        /// <returns>文件表压缩包大小</returns>
        public static uint DecryptTableSize(FileHeader fileHeader,TableKeyGroup tableKeyGroup,out uint uncompressedLength)
        {
            uncompressedLength = fileHeader.OffsetKey1;
            uint compressedLength = fileHeader.OffsetKey1;

            uncompressedLength &= DecryptHelper.ConstKey1;
            compressedLength &= DecryptHelper.ConstKey1;

            uncompressedLength ^= tableKeyGroup.UncompressedSizeKey;
            compressedLength ^= tableKeyGroup.CompressedSizeKey;

            uncompressedLength -= DecryptHelper.ConstKey2;
            uncompressedLength ^= fileHeader.OffsetKey2;
            compressedLength -= DecryptHelper.ConstKey2;
            compressedLength ^= fileHeader.OffsetKey2;

            uncompressedLength -= DecryptHelper.ConstKey3;
            compressedLength -= DecryptHelper.ConstKey3;
            uncompressedLength ^= fileHeader.OffsetKey1;
            compressedLength ^= fileHeader.OffsetKey1;

            return compressedLength;
        }

        /// <summary>
        /// 解密文件表压缩包
        /// </summary>
        /// <param name="buffer">文件表数据</param>
        /// <param name="length">解密长度</param>
        /// <param name="tableKey">文件表key</param>
        /// <returns>文件key</returns>
        public static uint DecryptCompressedTable(byte[] buffer,int length,uint tableKey)
        {
            //长度为0返回原key
            if (length == 0)
            {
                return tableKey;
            }
            //数据索引
            int index = 0;

            do
            {
                uint u32_data = buffer[index];
                tableKey ^= u32_data;
                u32_data = (uint)((int)u32_data * length);
                
                u32_data += DecryptHelper.ConstKey4;
                tableKey += u32_data;
                buffer[index] ^= BitConverter.GetBytes(tableKey).ElementAt(0);

                length--;
                index++;

            } 
            while (length!=0);

            return tableKey;       
        }
        /// <summary>
        /// 计算文件解密的XorKey和Xor长度
        /// </summary>
        /// <param name="fileKey">文件key</param>
        /// <param name="xorKey">解密异或key</param>
        /// <param name="xorLength">解密异或长度</param>
        public static void DecryptFileKey(uint fileKey,out uint xorKey,out uint xorLength)
        {
            xorKey = fileKey & 0x000000FF;
            xorKey ^= 0xDF;
            xorKey += 0x17;
            xorKey = (fileKey & 0xFFFFFF00) + (xorKey & 0x000000FF);

            xorLength = fileKey;
            xorLength ^= 0x000000EA;
            xorLength &= 0x000001FF;
            xorLength += 0x00000200;
            xorLength &= 0xFFFFFFF8;            //向下8字节对齐
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="data">文件数据</param>
        /// <param name="xorKey">文件异或key</param>
        /// <param name="xorLength">文件异或长度</param>
        public static void DecryptFile(byte[] data,uint xorKey,uint xorLength)
        {
            xorKey &= 0x000000FF;
            xorKey = (uint)((int)xorKey * (int)DecryptHelper.ConstKey5);
            byte[] xorKeyBytes = BitConverter.GetBytes(xorKey);

            
            if (data.Length < xorLength)
            {
                xorLength = (uint)data.Length;
            }

            int xorKeyIndex = 0;
            for (uint dataIndex=0; dataIndex < xorLength; dataIndex++)
            {
                data[dataIndex] ^= xorKeyBytes.ElementAt(xorKeyIndex);
                xorKeyIndex++;
                xorKeyIndex &= 0x00000003;
            }

        }
    }
}
