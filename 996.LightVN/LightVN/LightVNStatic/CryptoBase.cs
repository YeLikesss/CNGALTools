using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightVNStatic
{
    /// <summary>
    /// 加密接口
    /// </summary>
    public interface ICryptoFilter
    {
        /// <summary>
        /// 解密资源
        /// </summary>
        /// <param name="data">资源数据  长度必须为文件长度</param>
        public void Decrypt(Span<byte> data);
    } 
}
