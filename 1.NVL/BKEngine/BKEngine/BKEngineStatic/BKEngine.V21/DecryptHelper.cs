using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BKEngine.V21
{
    /// <summary>
    /// 数据解密
    /// </summary>
    public class DecryptHelper
    {
        /// <summary>
        /// 常量key1
        /// </summary>
        public static uint constKey1 = 0x811C9DC5;
        /// <summary>
        /// 常量key2
        /// </summary>
        public static uint constKey2 = 0x01000193;
        /// <summary>
        /// 常量key3
        /// </summary>
        public static uint constKey3 = 0x5D588B65;

        /// <summary>
        /// 列表解密
        /// </summary>
        /// <param name="databuffer">数据字节数组</param>
        /// <param name="datalength">数据长度</param>
        /// <param name="listkey">解密listkey</param>
        /// <returns>文件key</returns>
        public static uint DecryptList(byte[] databuffer,uint datalength,uint listkey)
        {
            uint index = 0;                        //数据索引
            uint nowlength = datalength;           //当前剩余长度
            uint temp=listkey;                   //临时变量
            while(index < datalength)
            {
                byte singledata = databuffer[index];    //获取单字节数据
                uint uint_data = Convert.ToUInt32(singledata);      //无符号扩展
                //计算解密listkey
                temp = temp ^ uint_data;
                temp = temp + constKey3;
                temp = temp + (uint)((int)uint_data * (int)nowlength);

                byte xorkey = (byte)(temp & 0x000000FF);         //取最后一个字节
                databuffer[index] = (byte)(singledata ^ xorkey);   //解密完成
                index++;                    //索引自增
                nowlength--;              //当前剩余长度自减
            }
            return temp;        //返回文件key
        }
        /// <summary>
        /// 解密文件listkey索引
        /// </summary>
        /// <param name="rangekey">索引范围listkey</param>
        /// <param name="indexrange">索引范围</param>
        /// <param name="unknowkey"></param>
        /// <returns></returns>
        public static uint DecryptIndex(uint rangekey,uint indexrange,uint unknowkey)
        {
            uint key1 = unknowkey & 0x000000FF;                      //取4个单字节listkey
            uint key2 = (unknowkey & 0x0000FF00)>>8;
            uint key3 = (unknowkey & 0x00FF0000) >> 16;
            uint key4 = (unknowkey & 0xFF000000) >> 24;
            //计算解密索引
            uint result = 0;
            result = (uint)((int)(key1 ^ constKey1) * (int)constKey2);
            result = (uint)((int)(key2 ^ result) * (int)constKey2);
            result = (uint)((int)(key3 ^ result) * (int)constKey2);
            result = (uint)((int)(key4 ^ result) * (int)constKey2);
            result = result & rangekey;

            if (indexrange > result)
            {   //解密的索引在范围内
                return result;
            }
            result = result - 1 - (rangekey >> 1);
            return result;
        }
        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="filebuffer">文件数据流</param>
        /// <param name="filekey">解密listkey</param>
        /// <param name="filesize">文件大小</param>
        /// <param name="fileoffset">文件在封包内偏移</param>
        /// <param name="memoryoffset">文件读取偏移</param>
        /// <returns>True为文件进行解密操作 False为未进行解密操作</returns>
        public static byte[] DecryptFile(byte[] filebuffer,int filekey,uint filesize,uint fileoffset,uint memoryoffset=0)
        {
            uint decryptfilesize;                              //需要解密的数据长度
            uint thisdecryptlength=filesize;            //实际解密的长度
            uint actuallength;                                //实际长度
            decryptfilesize = ((uint)(filekey % 0x00000023) + 2 * fileoffset + 3 * filesize) % 0x000001E8 + 0x000000EC;  //解密需要解密的文件长度
            if (memoryoffset < decryptfilesize)
            {
                actuallength = decryptfilesize - memoryoffset;       //计算实际需要解密长度
                if (actuallength < filesize)
                {   //实际解密长度和硬盘文件长度取最小
                    thisdecryptlength = actuallength;
                }
                if (thisdecryptlength > 0)
                {   //开始解密
                     for(uint index = 0; index < thisdecryptlength; index++)
                     {
                        //计算异或listkey
                        uint uint_xorkey = (7 * fileoffset + (uint)(filekey % 0x0000001B) + 5 * filesize) % 0x000000F1 + 0x0000000B;

                        byte b_xorkey = (byte)(uint_xorkey & 0x000000FF);           //取低8位作为异或listkey
                        filebuffer[index] = (byte)(filebuffer[index] ^ b_xorkey);           //异或解密文件
                     }
                }
                return filebuffer;
            }
            return null;
        }
    }
}
