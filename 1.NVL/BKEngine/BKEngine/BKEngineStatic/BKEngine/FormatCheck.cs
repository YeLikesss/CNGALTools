using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BKEngine
{
    public class FormatCheck
    {
        /// <summary>
        /// 文件头与格式
        /// </summary>
        private static Dictionary<string, string> fileFormat = new Dictionary<string, string>()
        {
            {"ID3",".mp3" },{"RIFF",".wav"},{"JFIF",".jpg"},{"Exif",".jpg"},{"PNG",".png"},{"OggS",".ogg"},{"bkc",".bkbin"},{"OTTO",".otf"}
        };

        /// <summary>
        /// 获取已知文件格式
        /// </summary>
        public static Dictionary<string, string> GetFileFormat => FormatCheck.fileFormat;

        /// <summary>
        /// 寻找文件头的特征码
        /// </summary>
        /// <param name="fileheader">文件头数据流</param>
        /// <param name="signature">特征码</param>
        /// <returns>True为检查成功 False为检查失败</returns>
        public static bool FileCheck(byte[] fileheader,byte[] signature)
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
