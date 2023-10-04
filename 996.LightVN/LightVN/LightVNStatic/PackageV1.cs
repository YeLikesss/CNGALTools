using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace LightVNStatic
{
    public class PackageV1 : IPackage
    {
        private readonly string mPackagePath = string.Empty;
        private readonly string mOutDirectory = string.Empty;
        private readonly bool mIsVaild = false;
        private readonly ICryptoFilter? mFilter = null;

        public bool Extract()
        {
            if (this.mIsVaild)
            {
                try
                {
                    //Zip解压
                    using ZipArchive zip = ZipFile.OpenRead(this.mPackagePath);
                    ReadOnlyCollection<ZipArchiveEntry> entries = zip.Entries;

                    int bufLen = 1024 * 1024 * 4;     //4M缓存
                    byte[] buffer = ArrayPool<byte>.Shared.Rent(bufLen);

                    //遍历zip文件表并提取
                    for(int i = 0; i < entries.Count; ++i)
                    {
                        ZipArchiveEntry entry = entries[i];
                        string path = Path.Combine(this.mOutDirectory, entry.FullName);
                        //导出文件夹
                        if(Path.GetDirectoryName(path) is string dir)
                        {
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                        }

                        //文件大小
                        long fileLength = entry.Length;

                        //刷新缓存大小
                        if (fileLength > bufLen)
                        {
                            bufLen = (int)fileLength;

                            ArrayPool<byte>.Shared.Return(buffer);
                            buffer = ArrayPool<byte>.Shared.Rent(bufLen);
                        }

                        Span<byte> fileData = buffer.AsSpan().Slice(0, (int)fileLength);

                        //读取流
                        {
                            using Stream stream = entry.Open();
                            stream.Read(fileData);
                        }

                        //解密流
                        this.mFilter?.Decrypt(fileData);

                        //写入到硬盘
                        using FileStream outFs = new(path, FileMode.Create, FileAccess.ReadWrite);
                        outFs.Write(fileData);
                    }
                    //释放缓存
                    ArrayPool<byte>.Shared.Return(buffer);
                    return true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("{0}::{1} 异常:{2}", nameof(PackageV1), nameof(this.Extract), ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 封包构造函数
        /// </summary>
        /// <param name="pkgPath">封包全路径</param>
        /// <param name="filter">解密器</param>
        public PackageV1(string pkgPath, ICryptoFilter? filter)
        {
            if (!string.IsNullOrEmpty(pkgPath) && Path.GetExtension(pkgPath) == ".vndat" && File.Exists(pkgPath))
            {
                this.mPackagePath = pkgPath;
                this.mFilter = filter;
                this.mIsVaild = true;

                if(Path.GetDirectoryName(pkgPath) is string dir)
                {
                    this.mOutDirectory = Path.Combine(dir, "Static_Extract", Path.GetFileNameWithoutExtension(pkgPath));
                }
            }
        }
    }
}
