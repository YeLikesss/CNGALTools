using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace IrregularsStatic
{
    /// <summary>
    /// 默认值
    /// </summary>
    public class GameInformationBase
    {
        protected byte[] Key { get; } = new byte[]
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
        };
        protected byte[] IV { get; } = new byte[]
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
        };

        protected virtual byte[] CustomKey { get; } = Array.Empty<byte>();

        public GameInformationBase()
        {
            byte[] key = this.CustomKey;

            if (key.Length >= 0x20)
            {
                this.Key = key[0..16];
                this.IV = key[16..32];
            }
        }

        /// <summary>
        /// 创建资源流
        /// </summary>
        /// <param name="fileName">文件全路径</param>
        /// <returns></returns>
        public Stream CreateStream(string fileName)
        {
            return GameInformationBase.CreateStream(fileName, this.Key, this.IV);
        }

        /// <summary>
        /// 创建资源流
        /// </summary>
        /// <param name="fileName">文件全路径</param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static Stream CreateStream(string fileName, byte[] key, byte[] iv)
        {
            if (File.Exists(fileName))
            {
                Aes aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor();
                FileStream inFs = File.OpenRead(fileName);
                CryptoStream cryptoStream = new(inFs, decryptor, CryptoStreamMode.Read);

                return cryptoStream;
            }
            else
            {
                return Stream.Null;
            }
        }
    }


    public class MobiusBand : GameInformationBase
    {
        protected override byte[] CustomKey { get; } = Encoding.Unicode.GetBytes("MobiusBand*Steam");
    }

}