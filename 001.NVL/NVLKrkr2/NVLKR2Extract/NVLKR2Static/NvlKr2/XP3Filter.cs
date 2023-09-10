using System;
using System.Collections.Generic;
using System.Text;

namespace NVLKR2Static
{
    /// <summary>
    /// 资源解密
    /// </summary>
    public class XP3Filter
    {
        private byte[] mKey;
        /// <summary>
        /// 解密构造
        /// </summary>
        /// <param name="entry">文件表</param>
        /// <param name="keyInformation">游戏key信息</param>
        public XP3Filter(XP3Archive.XP3File entry ,IKeyInformation keyInformation)
        {
            this.mKey = new byte[12];
            BitConverter.TryWriteBytes(this.mKey, entry.Adlr32);
            Array.Copy(keyInformation.Key, 0, this.mKey, 4, 8);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移</param>
        public void Decrypt(Span<byte> data, long offset = 0)
        {
            byte[] key = this.mKey;
            int keyLen = this.mKey.Length;

            int keyIndex = (int)(offset % keyLen);

            for(int i = 0; i < data.Length; ++i)
            {
                data[i] ^= key[keyIndex];
                ++keyIndex;

                if (keyIndex == keyLen)
                {
                    keyIndex = 0;
                }
            }
        }
    }
}
