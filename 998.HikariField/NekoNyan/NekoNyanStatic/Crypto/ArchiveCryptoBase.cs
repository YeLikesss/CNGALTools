using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NekoNyanStatic.Crypto.V1;

namespace NekoNyanStatic.Crypto
{
    /// <summary>
    /// 加密版本
    /// </summary>
    public enum CryptoVersion : int
    {
        V10 = 0,
        V11 = 1,
        V12 = 2,
    }


    public abstract class ArchiveCryptoBase : IDisposable
    {
        /// <summary>
        /// 文件表
        /// </summary>
        public struct FileEntry
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName;
            /// <summary>
            /// 文件偏移
            /// </summary>
            public uint Offset;
            /// <summary>
            /// 文件大小
            /// </summary>
            public uint Size;
            /// <summary>
            /// Key
            /// </summary>
            public uint Key;
        }
        protected FileStream mFileStream;
        protected List<FileEntry> mFileEntries;         //文件表数组

        protected string mPackageName;        //封包名字
        protected string mExtractDir;          //提取路径

        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Initialize();
        /// <summary>
        /// 获得文件表
        /// </summary>
        /// <param name="rawEntryData">原文件表信息</param>
        /// <param name="rawFileNamesData">原文件名信息</param>
        /// <param name="fileCount">文件个数</param>
        protected abstract void ParserFileEntry(Span<byte> rawEntryData, Span<byte> rawFileNamesData, int fileCount);
        /// <summary>
        /// 生成key  256字节长度
        /// </summary>
        /// <param name="tablePtr">表指针</param>
        /// <param name="key">key</param>
        protected abstract void KeyGenerator(Span<byte> tablePtr, uint key);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">解密Key</param>
        protected abstract void Decrypt(Span<byte> data, uint key);


        /// <summary>
        /// 提取资源
        /// </summary>
        public void Extract()
        {
            for(int idx = 0; idx < this.mFileEntries.Count; ++idx)
            {
                FileEntry entry = this.mFileEntries[idx];

                string extractPath = Path.Combine(this.mExtractDir, entry.FileName);
                string archiveDir = Path.GetDirectoryName(extractPath);
                if (!Directory.Exists(archiveDir))
                {
                    Directory.CreateDirectory(archiveDir);
                }

                //读取并解密资源
                this.mFileStream.Position = entry.Offset;
                byte[] buffer = ArrayPool<byte>.Shared.Rent((int)entry.Size);

                Span<byte> data = buffer.AsSpan(0, (int)entry.Size);
                this.mFileStream.Read(data);
                this.Decrypt(data, entry.Key);

                //回写解密后资源
                using FileStream outStream = new(extractPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                outStream.Write(data);
                outStream.Flush();

                //释放
                ArrayPool<byte>.Shared.Return(buffer);

                Console.WriteLine("Extract ---> {0}/{1}", this.mPackageName, entry.FileName);
            }
        }

        /// <summary>
        /// 使用封包路径初始化
        /// </summary>
        /// <param name="pkgPath">封包路径</param>

        private void InitializeWithPackagePath(string pkgPath)
        {
            this.mFileStream = File.OpenRead(pkgPath);
            this.mPackageName = Path.GetFileNameWithoutExtension(pkgPath);
            this.mExtractDir = Path.Combine(Path.GetDirectoryName(pkgPath), "Extract_Static", this.mPackageName);
            this.Initialize();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            this.mFileEntries.Clear();

            this.mFileStream.Close();
            this.mFileStream.Dispose();
        }

        /// <summary>
        /// 创建加密对象
        /// </summary>
        /// <param name="pkgPath">封包全路径</param>
        /// <param name="ver">加密版本</param>
        /// <returns></returns>
        public static ArchiveCryptoBase Create(string pkgPath, CryptoVersion ver)
        {
            ArchiveCryptoBase filter;
            filter = ver switch
            {
                CryptoVersion.V10 => new ArchiveCryptoV10(),
                CryptoVersion.V11 => new ArchiveCryptoV11(),
                CryptoVersion.V12 => new ArchiveCryptoV12(),
                _ => null,
            };
            filter?.InitializeWithPackagePath(pkgPath);
            return filter;
        }


        /// <summary>
        /// 枚举封包路径
        /// </summary>
        /// <param name="dirPath">封包文件夹路径</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumeratePackagePaths(string dirPath)
        {
            return Directory.EnumerateFiles(dirPath, "*.dat");
        }
        /// <summary>
        /// 检查是否合法封包
        /// </summary>
        /// <param name="path">封包路径</param>
        /// <returns></returns>
        public static bool IsVaildPackage(string path)
        {
            return Path.GetExtension(path) == ".dat";
        }

    }
}
