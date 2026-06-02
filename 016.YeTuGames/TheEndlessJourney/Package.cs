using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using Utils.Binary;

namespace TheEndlessJourney
{
    internal class Package
    {
        static Package()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// 提取封包
        /// </summary>
        /// <param name="pkgPath">封包全路径</param>
        /// <returns>True提取成功 False提取失败</returns>
        public static bool Extract(string pkgPath)
        {
            if (!File.Exists(pkgPath))
            {
                return false;
            }

            string pkgName = Path.GetFileNameWithoutExtension(pkgPath);
            string extractDir = Path.Combine(Path.GetDirectoryName(pkgPath)!, "Static_Extract", pkgName);

            using ZipArchive zip = ZipFile.Open(pkgPath, ZipArchiveMode.Read, Encoding.GetEncoding(936));
            ReadOnlyCollection<ZipArchiveEntry> entries = zip.Entries;
            for (int i = 0; i < entries.Count; ++i)
            {
                ZipArchiveEntry entry = entries[i];

                //过滤文件夹
                if(entry.FullName.Last() == '/')
                {
                    continue;
                }

                string filename = entry.FullName;
                string path = Path.Combine(extractDir, filename);
                {
                    if (Path.GetDirectoryName(path) is string dir && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                using Stream zipStream = entry.Open();
                using FileStream outFs = File.Create(path);

                string ext = Path.GetExtension(filename);

                //过滤不加密文件
                if (ext == ".csb" || 
                    ext == ".ttf")
                {
                    zipStream.CopyTo(outFs);
                }
                else
                {
                    using Stream decStream = Package.CreateDecryptStream(filename, zipStream);
                    decStream.CopyTo(outFs);
                }
                outFs.Flush();

                Console.WriteLine($"提取成功: {pkgName}/{filename}");
            }
            return true;
        }

        /// <summary>
        /// 解密资源
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <param name="stream">原始流</param>
        /// <returns>解密流</returns>
        private static Stream CreateDecryptStream(string name, Stream stream)
        {
            byte[] md5 = MD5.HashData(Encoding.UTF8.GetBytes(name));
            string md5Str = BinaryDataConvert.HexToString(md5).ToLower();
            string keyStr = md5Str.Substring((byte)md5Str[10] % 24, 8);

            using DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Encoding.UTF8.GetBytes(keyStr);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = des.CreateDecryptor();
            return new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
        }
    }
}
