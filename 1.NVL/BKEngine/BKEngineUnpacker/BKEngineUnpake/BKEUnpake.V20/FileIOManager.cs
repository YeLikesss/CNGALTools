using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BKEUnpake.V20
{
    public class FileIOManager
    {
        /// <summary>
        /// 读取文件中的压缩资源
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="compressedresinfo">压缩资源列表项</param>
        /// <param name="errormessage">错误信息</param>
        /// <returns>读取成功返回数据流数组 读取失败为Null</returns>
        public static List<List<byte>> ReadCompressedResources(FileStream fileStream,List<BZip2CompressedResources> compressedresinfo,out string errormessage)
        {
            List<List<byte>> compresseddatalist = new List<List<byte>>();
            byte[] buffer;
            foreach(BZip2CompressedResources compressedinfo in compressedresinfo)
            {
                try
                {
                    fileStream.Seek(compressedinfo.FileOffset, SeekOrigin.Begin);       //设置读取偏移
                    buffer = new byte[compressedinfo.FileSize];             //设置要读取的字节数
                    fileStream.Read(buffer, 0, buffer.Length);                 //读取文件
                    compresseddatalist.Add(buffer.ToList());                 //添加到数组
                }
                catch(Exception ex)
                {
                    errormessage = "读取文件失败\n" + ex.Message;     //设置错误信息
                    return null;
                }
            }
            errormessage = null;
            return compresseddatalist;
        }
        /// <summary>
        /// 读取普通资源文件
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="normalresinfo">普通资源表</param>
        /// <param name="errormessage">错误信息</param>
        /// <returns>读取成功返回数据流数据数组 读取失败为Null</returns>
        public static List<List<byte>> ReadNormalResources(FileStream fileStream,List<NormalResources> normalresinfo,out string errormessage)
        {
            List<List<byte>> normaldatalist = new List<List<byte>>();
            foreach(NormalResources normalinfo in normalresinfo)
            {
                try
                {
                    fileStream.Seek(normalinfo.FileOffset, SeekOrigin.Begin);   //设置读取偏移
                    byte[] buffer = new byte[normalinfo.FileSize];              //设置要读取的字节数
                    fileStream.Read(buffer, 0, buffer.Length);          //读取文件
                    normaldatalist.Add(buffer.ToList());                 //添加到数组
                }
                catch(Exception ex)
                {
                    errormessage = "读取文件失败\n" + ex.Message;     //设置错误信息
                    return null;
                }
            }
            errormessage = null;
            return normaldatalist;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="data">数据流</param>
        /// <param name="filepath">文件路径</param>
        /// <param name="errormessage">错误信息</param>
        /// <returns>True为写入成功 False为写入失败</returns>
        public static bool WriteFile(byte[] data,string filepath,out string errormessage)
        {
            FileInfo fileInfo = new FileInfo(filepath);

            //文件夹不存在则创建
            if (Directory.Exists(fileInfo.DirectoryName)==false)
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }

            FileStream fs=null;
            try
            {
                fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);           //实例化写入文件流
                fs.Write(data, 0, data.Length);                         //写入数据
                errormessage = null;
                return true;
            }
            catch(Exception ex)
            {
                errormessage = "写入文件失败\n" + ex.Message;
                return false;
            }
            finally
            {
                fs?.Close();
                fs?.Dispose();
            }
        }

    }
}
