using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NvlUnity.V1
{
    /// <summary>
    /// Key信息V1
    /// </summary>
    public interface IKeyInformationV1
    {
        /// <summary>
        /// 修复头
        /// </summary>
        public byte[] Header { get; }
        /// <summary>
        /// 加密Key
        /// </summary>
        public byte[] XorKey { get; }
    }

    /// <summary>
    /// V1版加密
    /// </summary>
    internal class NVLFilterV1
    {
        private IKeyInformationV1 mFilterKey;

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">原数据</param>
        /// <param name="offset">偏移</param>
        /// <returns>当前位置</returns>
        public long Decrypt(Span<byte> data, long offset = 0)
        {
            if(this.mFilterKey is null || offset < 0)
            {
                return 0;
            }

            int dataLen = data.Length;
            int dataPos = 0;

            if (dataLen != 0)
            {
                //检查头
                {
                    Span<byte> header = this.mFilterKey.Header;
                    int headerLen = header.Length;

                    //偏移位置在头部范围内
                    if (offset < headerLen)
                    {
                        int copyLen = (int)Math.Min(headerLen - offset, dataLen);
                        //复制头
                        header[(int)offset..copyLen].CopyTo(data[(int)offset..copyLen]);

                        dataPos += copyLen;
                        offset += copyLen;
                    }
                }

                //解密
                {
                    Span<byte> key = this.mFilterKey.XorKey;
                    int keyLen = key.Length;
                    int keyPos = (int)(offset % keyLen);

                    while (dataPos < dataLen)
                    {
                        data[dataPos] ^= key[keyPos];
                        ++keyPos;

                        if (keyPos == keyLen)
                        {
                            keyPos = 0;
                        }

                        ++dataPos;
                        ++offset;
                    }
                }
            }
            return offset;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="keyInformation"></param>
        public NVLFilterV1(IKeyInformationV1 keyInformation)
        {
            this.mFilterKey = keyInformation;
        }
    }
}
