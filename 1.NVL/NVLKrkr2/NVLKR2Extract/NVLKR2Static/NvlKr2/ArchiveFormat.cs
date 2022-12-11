using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NvlKr2Extract
{
    public class ArchiveFormat
    {
        /// <summary>
        /// 文件头与格式
        /// </summary>
        private static Dictionary<byte[], string> fileFormat = new Dictionary<byte[], string>()
        {

            {new byte[]{ 0x49,0x44,0x33 },".mp3" },
            {new byte[]{ 0x52,0x49,0x46,0x46 },".wav"},
            {new byte[]{ 0x4A,0x46,0x49,0x46 },".jpg"},
            {new byte[]{ 0x45,0x78,0x69,0x66 },".jpg"},
            {new byte[]{ 0x50,0x4E,0x47 },".png"},
            {new byte[]{ 0x4F,0x67,0x67,0x53 },".ogg"},
            {new byte[]{ 0x4F,0x54,0x54,0x4F },".otf"},
            {new byte[]{ 0x30,0x26,0xB2,0x75 },".wmv"}
        };

        /// <summary>
        /// 获取已知文件格式
        /// </summary>
        public static Dictionary<byte[], string> GetFileFormat => ArchiveFormat.fileFormat;

        /// <summary>
        /// 寻找文件头的特征码
        /// </summary>
        /// <param name="fileheader">文件头数据流</param>
        /// <param name="signature">特征码</param>
        /// <returns>True为检查成功 False为检查失败</returns>
        public static bool FileCheck(byte[] fileheader, byte[] signature)
        {
            for (int i = 0; i < fileheader.Length; i++)
            {
                if (fileheader.Skip(i).Take(signature.Length).SequenceEqual(signature))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
