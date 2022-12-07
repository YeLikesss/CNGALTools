using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snowing
{
    /// <summary>
    /// 文本资源相关
    /// </summary>
    public class ScenarioArchive
    {
        /// <summary>
        /// 文本解密
        /// </summary>
        /// <param name="dataInfo">资源信息</param>
        /// <param name="key">AES128Key</param>
        /// <param name="iv">AES128IV</param>
        /// <returns></returns>
        public static Archive.DataInfo Decrypt(Archive.DataInfo dataInfo,byte[] key,byte[] iv)
        {
            dataInfo.Data = AesHelper.AesDecrypt128(dataInfo.Data, key, iv);
            return dataInfo;
        }
    }
}
