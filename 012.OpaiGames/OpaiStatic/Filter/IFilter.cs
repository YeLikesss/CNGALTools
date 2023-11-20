using System;

namespace OpaiStatic.Filter
{
    /// <summary>
    /// 加密接口
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据指针</param>
        /// <param name="blockOffset">数据块偏移</param>
        public void Decrypt(Span<byte> data, long blockOffset);
    }
}