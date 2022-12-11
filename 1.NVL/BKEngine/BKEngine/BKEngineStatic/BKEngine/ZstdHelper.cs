using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ZstdNet;

namespace BKEngine
{
    /// <summary>
    /// zstd压缩解压
    /// </summary>
    public class ZstdHelper
    {
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="data">压缩包数据</param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            try
            {
                List<byte> metadata = new List<byte>();
                //输入到zstd库进行解压
                DecompressionStream zstd = new DecompressionStream(ms);
                int temp;
                while (true)
                {   //循环读取数据
                    temp = zstd.ReadByte();
                    if (temp == -1)
                    {
                        //读取到-1则为读取完毕
                        break;
                    }
                    metadata.Add((byte)(uint)temp);         //添加数据到数组
                }

                return metadata.ToArray();                   //解压得到数据
            }
            catch
            {
                return null;
            }
            finally
            {
                ms?.Close();                
                ms?.Dispose();
            }
        }
    }
}
