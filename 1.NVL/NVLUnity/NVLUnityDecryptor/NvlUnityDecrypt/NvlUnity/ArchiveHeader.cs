using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NvlUnity
{
    public class ArchiveHeader
    {
        /// <summary>
        /// Unity版本
        /// </summary>
        public enum UnityVersion
        {
            V2018_4_0_65448,
            V2018_4_26_44060
        }
        /// <summary>
        /// Unity版本与对应头数据
        /// </summary>
        public static readonly Dictionary<UnityVersion, byte[]> UnityHeaderList = new Dictionary<UnityVersion, byte[]>()
        {
            {
                //UnityFS  5.x.x 2018.4.0f1
                UnityVersion.V2018_4_0_65448,
                new byte[]
                {
                    0x55, 0x6E, 0x69, 0x74, 0x79, 0x46, 0x53, 0x00, 0x00, 0x00, 0x00, 0x06, 0x35, 0x2E, 0x78, 0x2E,
                    0x78, 0x00, 0x32, 0x30, 0x31, 0x38, 0x2E, 0x34, 0x2E, 0x30, 0x66, 0x31, 0x00, 0x00, 0x00, 0x00
                } 
            },
            {
                //UnityFS  5.x.x 2018.4.26f1
                UnityVersion.V2018_4_26_44060,
                new byte[]
                {
                    0x55, 0x6E, 0x69, 0x74, 0x79, 0x46, 0x53, 0x00, 0x00, 0x00, 0x00, 0x06, 0x35, 0x2E, 0x78, 0x2E,
                    0x78, 0x00, 0x32, 0x30, 0x31, 0x38, 0x2E, 0x34, 0x2E, 0x32, 0x36, 0x66, 0x31, 0x00, 0x00, 0x00
                }
            }
        };

    }
}
