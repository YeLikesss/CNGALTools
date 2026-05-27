using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;

namespace LightVNStatic
{
    /// <summary>
    /// Light.VN V1版本封包
    /// </summary>
    public class PackageV1
    {
        private readonly CryptoFilterV1? mFilter = null;

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="pkgPath">封包路径</param>
        /// <returns>True解包成功 False解包失败</returns>
        public bool Extract(string pkgPath)
        {
            if (Path.GetExtension(pkgPath) != ".vndat")
            {
                return false;
            }
            if (!File.Exists(pkgPath))
            {
                return false;
            }

            string extractDir = Path.Combine(Path.GetDirectoryName(pkgPath)!, "Static_Extract");

            //Zip解压
            using ZipArchive zip = ZipFile.OpenRead(pkgPath);
            ReadOnlyCollection<ZipArchiveEntry> entries = zip.Entries;

            //遍历zip文件表并提取
            for (int i = 0; i < entries.Count; ++i)
            {
                ZipArchiveEntry entry = entries[i];

                string path = Path.Combine(extractDir, entry.FullName);
                {
                    if (Path.GetDirectoryName(path) is string dir && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                //读取流
                byte[] fileData = new byte[entry.Length];
                using Stream stream = entry.Open();
                stream.Read(fileData);

                //解密流
                this.mFilter?.Decrypt(fileData);

                //写入到硬盘
                using FileStream outFs = File.Create(path);
                outFs.Write(fileData);

                Console.WriteLine("成功: {0}", path[(extractDir.Length + 1)..]);
            }
            return true;
        }

        /// <summary>
        /// 封包构造函数
        /// </summary>
        /// <param name="filter">解密器</param>
        public PackageV1(CryptoFilterV1? filter)
        {
            this.mFilter = filter;
        }
    }
}
