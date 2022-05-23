using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Snowing
{
    /// <summary>
    /// 引擎AES解密相关
    /// </summary>
    public class AesHelper
    {
        /// <summary>
        /// Aes128解密
        /// </summary>
        /// <param name="fileData">文件数据</param>
        /// <param name="key">解密Key</param>
        /// <param name="iv">IV初始向量</param>
        /// <returns>解密后数据</returns>
        public static byte[] AesDecrypt128(byte[] fileData,byte[] key,byte[] iv)
        {
            Aes aes = Aes.Create();
            aes.BlockSize = 128;        //快对齐128位
            aes.KeySize = 128;          //key长度128位
            aes.Padding = PaddingMode.None;     //不填充
            aes.Key = key;              //设置key
            aes.IV = iv;                //设置IV
            aes.Mode = CipherMode.CBC;      //设置解密模式

            byte[] decryptData = new byte[fileData.Length];    
            fileData.CopyTo(decryptData,0);         //复制数据

            //解密数据
            aes.CreateDecryptor().TransformBlock(fileData, 0, (int)(fileData.Length&0xFFFFFFF0), decryptData, 0);

            return decryptData;
        }
    }
}
