using System;
using System.Buffers;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Extractor.Untils;
using Extractor.ZixRenpy7V1.Crypto;

namespace Extractor.ZixRenpy7V1.Renpy
{
    /// <summary>
    /// 文件表
    /// </summary>
    public class FileEntry
    {
        /// <summary>
        /// 资源偏移
        /// </summary>
        public long Offset { get; set; }
        /// <summary>
        /// 资源大小
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// 资源头
        /// </summary>
        public byte[] Header { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileInfo">文件表信息</param>
        public FileEntry(object[] fileInfo)
        {
            {
                if (fileInfo[0] is long i64)
                {
                    this.Offset = i64;
                }
                else if (fileInfo[0] is int i32)
                {
                    this.Offset = i32;
                }
            }

            {
                if (fileInfo[1] is long i64)
                {
                    this.Size = i64;
                }
                else if (fileInfo[1] is int i32)
                {
                    this.Size = i32;
                }
            }

            if(fileInfo[2] is string strHeader)
            {
                ReadOnlySpan<char> header = strHeader.AsSpan();
                if (header.Length == 16)
                {
                    this.Header = new byte[16];
                    for (int i = 0; i < 16; ++i)
                    {
                        this.Header[i] = (byte)(header[i] & 0xFF);
                    }
                }
                else
                {
                    this.Header = Array.Empty<byte>();
                }
            }
            else
            {
                this.Header = Array.Empty<byte>();
            }
        }
    }

    /// <summary>
    /// Renpy各种路径
    /// </summary>
    public class RenpyPath
    {
        private readonly string mPath;

        /// <summary>
        /// 获取资源路径
        /// </summary>
        /// <returns></returns>
        public string GetArchivePath()
        {
            return Path.Combine(this.mPath, "game");
        }

        /// <summary>
        /// 获取py模块路径
        /// </summary>
        /// <returns></returns>
        public string GetModulePath()
        {
            return Path.Combine(this.mPath, "renpy");
        }


        /// <summary>
        /// 获取所有资源文件全路径
        /// </summary>
        /// <returns></returns>
        public string[] GetAllArchiveFilesFullPath()
        {
            return Directory.GetFiles(this.GetArchivePath(), "*.rpa", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 获取所有模块文件全路径 (加密的)
        /// </summary>
        /// <returns></returns>
        public string[] GetAllModuleFilesFullPath()
        {
            return Directory.GetFiles(this.GetModulePath(), "*.pyc", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 获取提取目录
        /// </summary>
        /// <returns></returns>
        public string GetExtractPath()
        {
            return Path.Combine(this.mPath, "Static_Extract");
        }

        /// <summary>
        /// 获取相对路径
        /// </summary>
        /// <returns></returns>
        public string GetRelativePath(string path)
        {
            if (this.mPath == path.Substring(0, this.mPath.Length))
            {
                ReadOnlySpan<char> str = path.AsSpan().Slice(this.mPath.Length);

                int pos = 0;
                for (int i = 0; i < str.Length; ++i)
                {
                    if (str[i] == '\\' || str[i] == '/')
                    {
                        pos++;
                    }
                    else
                    {
                        break;
                    }
                }

                return str.Slice(pos).ToString();
            }
            else
            {
                return path;
            }
        }

        /// <summary>
        /// 恢复扩展名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string FixExtension(string path)
        {
            return Path.ChangeExtension(path, ".pyc");
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gamePath">游戏路径</param>
        public RenpyPath(string gamePath)
        {
            this.mPath = gamePath;
        }
    }

    /// <summary>
    /// 提取接口
    /// </summary>
    public interface IExtractor
    {
        /// <summary>
        /// 提取资源
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <param name="extractPath">导出全路径</param>
        public void Extract(string filePath, string extractPath);

        /// <summary>
        /// 解压解析脚本
        /// <para>在提取资源完毕后调用</para>
        /// </summary>
        /// <param name="extractPath">导出全路径</param>
        public void ExtractScript(string extractPath);
    }

    /// <summary>
    /// 时间记忆:碎片 Renpy7.3.5
    /// </summary>
    public class AeonOnMosaicAnemone : KeyInformationBase, IExtractor
    {
        public void Extract(string filePath, string extractPath)
        {
            string extractDir = Path.Combine(extractPath, Path.GetFileNameWithoutExtension(filePath));

            //开启流读取
            using FileStream mFs = File.OpenRead(filePath);
            using BinaryReader mBr = new(mFs);

            mFs.Seek(96, SeekOrigin.Begin);

            //分别读取 文件表key 资源key 文件表offset
            uint key = mBr.ReadUInt32() ^ 0x154AEF91;
            uint skey = mBr.ReadUInt32() ^ 0x154AEF91;
            uint entryOffset = mBr.ReadUInt32() ^ 0x154AEF91;

            //读表
            byte[] entry = new byte[mFs.Length - entryOffset];
            mFs.Seek(entryOffset, SeekOrigin.Begin);
            mFs.Read(entry);

            entry = Zlib.Decompress(entry);

            //获取文件信息表
            Hashtable entryInfo = (Hashtable)Pickle.Decode(entry);

            //文件头key
            Span<uint> headerKey = stackalloc uint[4] { 0x641F6916, 0x7EA54007, 0x1E20D401, 0x11A27A20 };

            //遍历文件表
            foreach (DictionaryEntry archiveInfo in entryInfo)
            {
                string fileName = (string)archiveInfo.Key;      //获取文件名

                object[]? fileInfo = (object[])((ArrayList)archiveInfo.Value)[0];       //获取文件信息
                FileEntry fileEntry = new(fileInfo);        //获取文件信息

                fileEntry.Offset ^= key;
                fileEntry.Size ^= key;

                string extractFullPath = Path.Combine(extractDir, fileName);
                {
                    if (Path.GetDirectoryName(extractFullPath) is string dir)
                    {
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }
                }
                using FileStream mFsW = new(extractFullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

                //检测资源头
                if (fileEntry.Header.Length != 0)
                {
                    unsafe
                    {
                        Span<byte> headerPtr = fileEntry.Header.AsSpan();
                        Span<uint> headerPtrPack4 = MemoryMarshal.Cast<byte, uint>(headerPtr);

                        uint hKey = skey;
                        {
                            Span<byte> hKeyPtr = new(&hKey, 4);
                            hKeyPtr.Reverse();      //大端
                        }

                        for (int index = 0; index < 4; index++)
                        {
                            headerPtrPack4[index] ^= headerKey[index] ^ hKey;
                        }
                    }
                    mFsW.Write(fileEntry.Header);         //写入头
                }

                //提取
                {
                    byte[] buffer = new byte[fileEntry.Size];
                    mFs.Seek(fileEntry.Offset, SeekOrigin.Begin);
                    mFs.Read(buffer);
                    mFsW.Write(buffer);         //写入数据
                    mFsW.Flush();
                }

                Console.WriteLine("{0} ---> Extract Success", fileName);
            }
        }

        public void ExtractScript(string extractPath)
        {
            string[] scriptPaths = Directory.GetFiles(extractPath, "*.rpyc", SearchOption.AllDirectories);

            Span<byte> keys = stackalloc byte[4];

            for (int i = 0; i < scriptPaths.Length; ++i)
            {
                string path = scriptPaths[i];

                string extractDir = Path.ChangeExtension(path, string.Empty);
                if (!Directory.Exists(extractDir))
                {
                    Directory.CreateDirectory(extractDir);
                }

                using FileStream rpycFS = File.OpenRead(path);
                using BinaryReader rpycBR = new(rpycFS);

                //读key
                rpycFS.Position = 0x30;
                rpycFS.Read(keys);

                rpycFS.Position = 0xA;
                //读表
                long tablePosition;
                int slot, start, length;
                while (true)
                {
                    slot = rpycBR.ReadInt32();
                    start = rpycBR.ReadInt32();
                    length = rpycBR.ReadInt32();

                    tablePosition = rpycFS.Position;    //保存当前表位置

                    if (slot == 0)
                    {
                        break;
                    }

                    //解密信息
                    start = start ^ keys[0] ^ keys[3];
                    length = length ^ keys[1] ^ keys[2];
                    //读取封包
                    byte[] compressedData = ArrayPool<byte>.Shared.Rent(length);
                    rpycFS.Seek(start, SeekOrigin.Begin);
                    rpycBR.Read(compressedData, 0, length);
                    //解压导出
                    byte[] rawData = Zlib.Decompress(compressedData);

                    File.WriteAllBytes(Path.Combine(extractDir, $"{slot}.bin"), rawData);

                    ArrayPool<byte>.Shared.Return(compressedData);  //释放
                    rpycFS.Seek(tablePosition, SeekOrigin.Begin);   //回到下一个表的起始点
                }
            }
        }
    }


}
