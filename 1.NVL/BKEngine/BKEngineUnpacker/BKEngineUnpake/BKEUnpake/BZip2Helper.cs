using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;

namespace BKEUnpake
{
    /// <summary>
    /// bz2压缩解压
    /// </summary>
    public class BZip2Helper
    {
        /// <summary>
        /// 解压数据
        /// </summary>
        /// <param name="data">压缩包数据</param>
        /// <returns>解压后的数据  返回null则为解压失败</returns>
        public static byte[] DecompressData(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            try
            {
                List<byte> metadata = new List<byte>();
                //输入到bzip2库进行解压
                BZip2InputStream unbZip2 = new BZip2InputStream(ms);
                int temp;
                while (true)
                {   //循环读取数据
                    temp = unbZip2.ReadByte();
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
        /// <summary>
        /// 解压一组压缩数据
        /// </summary>
        /// <param name="data">一组压缩数据</param>
        /// <returns>解压后的数据数组 null为解压失败</returns>
        public static List<List<byte>> DecompressData(List<List<byte>> data)
        {
            List<List<byte>> unzipdata = new List<List<byte>>();     
            foreach(List<byte> databuffer in data)
            {
                //解压数据
                byte[] unzipbuffer = DecompressData(databuffer.ToArray());
                //解压失败
                if (unzipbuffer == null)
                {
                    return null;           
                }
                unzipdata.Add(unzipbuffer.ToList());     //添加到已解压数组
            }
            return unzipdata;
        }

    }
}
