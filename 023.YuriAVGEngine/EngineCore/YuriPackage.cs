using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace EngineCore
{
    /// <summary>
    /// 封包
    /// </summary>
    public class YuriPackage
    {
        /// <summary>
        /// 文件表
        /// </summary>
        public class FileEntry
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string Name { get; init; }
            /// <summary>
            /// 文件偏移
            /// </summary>
            public long Offset { get; init; }
            /// <summary>
            /// 文件大小
            /// </summary>
            public long Size { get; init; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="name">文件名</param>
            /// <param name="offset">文件偏移</param>
            /// <param name="size">文件大小</param>
            public FileEntry(string name, long offset, long size)
            {
                this.Name = name;
                this.Offset = offset;
                this.Size = size;
            }
        }

        /// <summary>
        /// 文件表文件后缀
        /// </summary>
        private const string cEntryFileExtension = ".pst";

        private string mName = string.Empty;
        private string mKey = string.Empty;
        private string mVersion = string.Empty;
        private byte[] mData = Array.Empty<byte>();
        private readonly List<FileEntry> mEntries = new();

        /// <summary>
        /// 名称
        /// </summary>
        public string Name => this.mName;
        /// <summary>
        /// 密钥
        /// </summary>
        public string Key => this.mKey;
        /// <summary>
        /// 版本
        /// </summary>
        public string Version => this.mVersion;
        /// <summary>
        /// 文件表
        /// </summary>
        public ReadOnlyCollection<FileEntry> Entries => this.mEntries.AsReadOnly();

        /// <summary>
        /// 提取资源
        /// </summary>
        /// <param name="directory">输出目录</param>
        /// <param name="progressCallBack">回调信息</param>
        public void Extract(string directory, IProgress<string>? progressCallBack = null)
        {
            string extractDirectory = Path.Combine(directory, "Static_Extract", this.mName);

            byte[] data = this.mData;
            foreach(FileEntry entry in this.mEntries)
            {
                string path = Path.Combine(extractDirectory, entry.Name);
                {
                    if(Path.GetDirectoryName(path) is string dir && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                //提取资源
                using FileStream outFs = File.Create(path);

                long offset = entry.Offset;
                long size = entry.Size;
                for(long i = 0L; i < size; ++i)
                {
                    outFs.WriteByte(data[offset + i]);
                }
                outFs.Flush();

                progressCallBack?.Report($"提取成功: {this.mName}/{entry.Name}");
            }
        }

        /// <summary>
        /// 打开封包
        /// </summary>
        /// <param name="filepath">封包路径</param>
        /// <param name="gameInfo">游戏信息</param>
        /// <param name="msg">错误信息</param>
        /// <returns>成功:封包对象 失败:null</returns>
        public static YuriPackage? Open(string filepath, YuriGameInformation gameInfo, out string msg)
        {
            string entrypath = filepath + YuriPackage.cEntryFileExtension;
            if(!File.Exists(filepath))
            {
                msg = "封包路径不存在";
                return null;
            }
            if (!File.Exists(entrypath))
            {
                msg = "封包表路径不存在";
                return null;
            }

            using StreamReader entryReader = new(entrypath);

            //获取文件头
            string[] header;
            {
                if (entryReader.ReadLine() is string s)
                {
                    header = s.Split('@');
                }
                else
                {
                    msg = "封包表不存在文件头数据";
                    return null;
                }
            }
            if (header.Length != 4 || header[0] != "___SlyviaLyyneheym")
            {
                msg = "封包表文件头数据错误";
                return null;
            }

            //解析头数据
            int count = Convert.ToInt32(header[1]);
            string name = header[2];
            string key;
            string version;
            {
                string[] ks = header[3].Split('?');
                key = ks[0];
                version = (ks.Length != 2) ? "0" : ks[1];
            }

            if (key != gameInfo.StringKey)
            {
                msg = "封包表Key不匹配";
                return null;
            }

            YuriPackage pkg = new()
            {
                mName = name,
                mKey = key,
                mVersion = version,
            };
            for (int i = 0; i < count; ++i)
            {
                //解析文件表
                if(entryReader.ReadLine() is string s)
                {
                    string[] info = s.Split(':');

                    //终止
                    if (info[0] == "___SlyviaLyyneheymEOF")
                    {
                        break;
                    }

                    if (info.Length == 3)
                    {
                        FileEntry entry = new(info[0], Convert.ToInt64(info[1]), Convert.ToInt64(info[2]));
                        pkg.mEntries.Add(entry);
                    }
                }
            }
            pkg.mData = File.ReadAllBytes(filepath);

            msg = string.Empty;
            return pkg;
        }
    }
}