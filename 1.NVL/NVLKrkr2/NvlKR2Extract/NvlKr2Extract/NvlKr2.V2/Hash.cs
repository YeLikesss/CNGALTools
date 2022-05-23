using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NvlKr2Extract.V2
{
    public class Hasher
    {
        /// <summary>
        /// 获取文件Hash
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static uint GetFileNameHash(string name)
        {
            uint hash = 0;
            byte[] nameBytes= Encoding.Unicode.GetBytes(name.ToLower());
            foreach(byte nb in nameBytes)
            {
                hash = 0x1000193 * hash ^ nb;
            }
            return hash;
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="hash1"></param>
        /// <param name="hash2"></param>
        /// <param name="hash3"></param>
        /// <returns></returns>
        public static string GetFileName(uint hash1,uint hash2,uint hash3)
        {
            hash2 ^= hash1;
            hash3 ^= hash1;

            string name = hash2.ToString("X8");

            switch (hash3)
            {
                case 0x01854675: name += ".png"; break;
                case 0x03D435DE: name += ".map"; break;
                case 0x2D1F13E0: name += ".asd"; break;
                case 0x482F4319: name += ".tjs"; break;
                case 0x58924012: name += ".txt"; break;
                case 0xB01C48CA: name += ".ks"; break; 
                case 0xC0F7DFB2: name += ".wav"; break; 
                case 0xE3A31D19: name += ".jpg"; break; 
                case 0xE7F3FEEB: name += ".ogg"; break;
                case 0xEB6BBBA0: name += ".avi"; break;
                case 0xEBC89F5E: name += ".wmv"; break;
                default: name += hash3.ToString("X8"); break;
            }

            return name;
        } 
    }
}
