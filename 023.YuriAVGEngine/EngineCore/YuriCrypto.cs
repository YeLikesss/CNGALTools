using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EngineCore
{
    /// <summary>
    /// 加密类
    /// </summary>
    public class YuriCrypto
    {
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="s">加密串</param>
        /// <param name="gameInfo">游戏信息</param>
        public static string DecryptString(string s, YuriGameInformation gameInfo)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            byte[] orgData = Convert.FromBase64String(s);
            DES des = new DESCryptoServiceProvider();

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, des.CreateDecryptor(gameInfo.Key, gameInfo.IV), CryptoStreamMode.Write);
            cs.Write(orgData);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
