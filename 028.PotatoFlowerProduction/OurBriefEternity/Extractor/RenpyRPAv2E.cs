using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Extractor.Utils;
using System.Collections;

namespace Extractor
{
    internal class RenpyRPAv2E
    {
        /// <summary>
        /// 文件表
        /// </summary>
        public class FileEntry
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName { get; init; }
            /// <summary>
            /// 偏移
            /// </summary>
            public long Offset { get; init; }
            /// <summary>
            /// 长度
            /// </summary>
            public long Length { get; init; }
            /// <summary>
            /// 加密key
            /// </summary>
            public byte[] Nonce { get; init; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="fileName">文件名</param>
            /// <param name="offset">文件偏移</param>
            /// <param name="length">文件长度</param>
            /// <param name="nonce">文件key</param>
            public FileEntry(string fileName, long offset, long length, byte[] nonce)
            {
                this.FileName = fileName;
                this.Offset = offset;
                this.Length = length;
                this.Nonce = nonce;
            }
        }
        private readonly string mFilePath;
        private readonly string mName;
        private readonly byte[] mKey = Array.Empty<byte>();
        private readonly List<FileEntry> mFileEntries = new();
        private readonly bool mInitialized;
        private readonly string mLastError;

        /// <summary>
        /// 初始化成功标志
        /// </summary>
        public bool Initialized => this.mInitialized;
        /// <summary>
        /// 封包名字
        /// </summary>
        public string Name => this.mName;
        /// <summary>
        /// 最后错误
        /// </summary>
        public string LastError => this.mLastError;

        /// <summary>
        /// 解包
        /// </summary>
        public void Extract()
        {
            if (!this.mInitialized)
            {
                return;
            }

            string pkgPath = this.mFilePath;
            string pkgName = this.mName;
            string outputDirectory = Path.GetDirectoryName(pkgPath)!;
            using FileStream inFs = File.OpenRead(pkgPath);

            foreach (FileEntry entry in this.mFileEntries)
            {
                string outPath = Path.Combine(outputDirectory, "Static_Extract", pkgName, entry.FileName);
                {
                    string dir = Path.GetDirectoryName(outPath)!;
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    byte[] buf = new byte[entry.Length];
                    inFs.Position = entry.Offset;
                    inFs.Read(buf);

                    AesCtr.Decrypt(buf, this.mKey, entry.Nonce);

                    using FileStream outFs = File.Create(outPath);
                    outFs.Write(buf);
                    outFs.Flush();

                    Console.WriteLine($"提取成功: {pkgName}/{entry.FileName}");
                }
            }
        }

        /// <summary>
        /// 定制Renpy 2E封包
        /// </summary>
        /// <param name="filepath">封包路径</param>
        /// <param name="key">封包key</param>
        public RenpyRPAv2E(string filepath, byte[] key)
        {
            this.mFilePath = filepath;
            this.mName = Path.GetFileNameWithoutExtension(filepath);

            bool successed = false;
            string errmsg = string.Empty;
            if(!File.Exists(filepath))
            {
                errmsg = "文件不存在";
                goto proc_end;
            }

            //解密Key
            if (key.Length != 32)
            {
                errmsg = "Key长度错误";
                goto proc_end;
            }
            this.mKey = key;

            {
                using FileStream inFs = File.OpenRead(filepath);

                //文件头
                byte[] hdr = new byte[40];
                if (inFs.Read(hdr) != hdr.Length)
                {
                    errmsg = "错误的文件格式";
                    goto proc_end;
                }
                if (Encoding.UTF8.GetString(hdr, 0, 8)!= "RPAE-2.0")
                {
                    errmsg = "错误的文件标识";
                    goto proc_end;
                }

                //文件表信息
                string sofs = Encoding.UTF8.GetString(hdr, 9, 16);
                string skey = Encoding.UTF8.GetString(hdr, 26, 8);
                if(!long.TryParse(sofs, NumberStyles.HexNumber, null, out long idxofs) ||
                   !long.TryParse(skey, NumberStyles.HexNumber, null, out long idxkey))
                {
                    errmsg = "错误的文件索引信息";
                    goto proc_end;
                }

                byte[] idxnoc = new byte[12];
                if (inFs.Read(idxnoc) != idxnoc.Length)
                {
                    errmsg = "错误的文件索引信息";
                    goto proc_end;
                }

                //读取解密文件表
                inFs.Position = idxofs;
                byte[] idxBytes = new byte[inFs.Length - inFs.Position];
                inFs.Read(idxBytes);

                try
                {
                    AesCtr.Decrypt(idxBytes, key, idxnoc);
                    idxBytes = Zlib.Decompress(idxBytes);
                }
                catch
                {
                    errmsg = "索引解密解压失败";
                    goto proc_end;
                }

                object idxObj = Pickle.Decode(idxBytes);
                if (idxObj is not Hashtable entries)
                {
                    errmsg = "索引反序列化失败";
                    goto proc_end;
                }

                //解析文件表
                List<FileEntry> fileEntries = this.mFileEntries;
                fileEntries.Capacity = entries.Count;
                foreach (DictionaryEntry entry in entries)
                {
                    string fileName = (entry.Key as string)!;

                    if ((entry.Value as ArrayList)?[0] is not object[] infos)
                    {
                        errmsg = "文件表解析失败";
                        goto proc_end;
                    }

                    long offset = Convert.ToInt64(infos[0]);
                    long length = Convert.ToInt64(infos[1]);

                    offset ^= idxkey;
                    length ^= idxkey;

                    byte[] nonce = infos[2] switch
                    {
                        null => Array.Empty<byte>(),
                        string s => Encoding.UTF8.GetBytes(s),
                        byte[] bytes => bytes,
                        _ => throw new InvalidDataException($"不支持的数据前缀 {infos[2].GetType().Name}"),
                    };

                    fileEntries.Add(new(fileName, offset, length, nonce));
                }

                successed = true;
            }

            //结束
            proc_end:
            {
                this.mInitialized = successed;
                this.mLastError = errmsg;
            }
        }
    }
}
