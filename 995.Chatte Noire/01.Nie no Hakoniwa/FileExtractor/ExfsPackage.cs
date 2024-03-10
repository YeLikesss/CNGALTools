using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace FileExtractor
{
    /// <summary>
    /// 封包
    /// </summary>
    internal class ExfsPackage
    {
        /// <summary>
        /// 文件头
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 0x50)]
        private struct FileHeader
        {
            /// <summary>
            /// 标记
            /// </summary>
            [FieldOffset(0x00)]
            public uint Signature;

            /// <summary>
            /// Reader版本
            /// </summary>
            [FieldOffset(0x04)]
            public uint ReaderVersion;

            /// <summary>
            /// Writer版本
            /// </summary>
            [FieldOffset(0x08)]
            public uint WriterVersion;

            /// <summary>
            /// 文件个数
            /// </summary>
            [FieldOffset(0x0C)]
            public uint FileCount;

            /// <summary>
            /// 文件头大小
            /// </summary>
            [FieldOffset(0x10)]
            public long HeaderSize;

            /// <summary>
            /// 文件表大小
            /// </summary>
            [FieldOffset(0x18)]
            public long EntryTableSize;

            /// <summary>
            /// 路径表大小
            /// </summary>
            [FieldOffset(0x20)]
            public long PathTableSize;

            /// <summary>
            /// 资源表便宜
            /// </summary>
            [FieldOffset(0x28)]
            public long ResourceTableOffset;

            [FieldOffset(0x30)]
            public long Reserve1;
            [FieldOffset(0x38)]
            public long Reserve2;
            [FieldOffset(0x40)]
            public long Reserve3;
            [FieldOffset(0x48)]
            public long Reserve4;

            /// <summary>
            /// 获取封包合法
            /// </summary>
            public bool IsVaild => this.Signature == 0x53465845u && this.ReaderVersion != 0u;
        }

        /// <summary>
        /// 文件表
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 0x20)]
        private struct FileEntry
        {
            /// <summary>
            /// 文件路径偏移
            /// </summary>
            [FieldOffset(0x00)]
            public long FilePathOffset;

            /// <summary>
            /// 文件路径长度
            /// </summary>
            [FieldOffset(0x08)]
            public long FilePathSize;

            /// <summary>
            /// 文件偏移
            /// </summary>
            [FieldOffset(0x10)]
            public long FileOffset;

            /// <summary>
            /// 文件大小
            /// </summary>
            [FieldOffset(0x18)]
            public long FileSize;
        }
        

        private readonly string mPackagePath;       //封包路径

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="packagePath">封包路径</param>
        public ExfsPackage(string packagePath) 
        {
            this.mPackagePath = packagePath;
        }

        /// <summary>
        /// 解包
        /// </summary>
        /// <returns>True解包成功 False解包失败</returns>
        public bool Extract()
        {
            string packagePath = this.mPackagePath;
            if (string.IsNullOrEmpty(packagePath))
            {
                Console.WriteLine("路径为空");
                return false;
            }

            if (!File.Exists(packagePath))
            {
                Console.WriteLine("{0} 封包不存在", packagePath);
                return false;
            }

            if(Path.GetExtension(packagePath) != ".pack")
            {
                Console.WriteLine("请选择.pack后缀的封包文件");
                return false;
            }

            using FileStream inFs = File.OpenRead(packagePath);

            //读头
            if(inFs.Length <= Unsafe.SizeOf<FileHeader>())
            {
                Console.WriteLine("错误的封包文件, 文件头长度不匹配");
                return false;
            }

            FileHeader fileHeader = ExfsPackage.Read<FileHeader>(inFs);
            if (!fileHeader.IsVaild)
            {
                Console.WriteLine("错误的封包文件, 文件头不匹配");
                return false;
            }

            inFs.Position = fileHeader.HeaderSize;

            //读表
            byte[] entryTableBytes = new byte[fileHeader.EntryTableSize];
            byte[] pathTableBytes = new byte[fileHeader.PathTableSize];

            if(inFs.Read(entryTableBytes) != entryTableBytes.LongLength)
            {
                Console.WriteLine("错误的封包文件, 文件信息表长度不匹配");
                return false;
            }

            if(inFs.Read(pathTableBytes) != pathTableBytes.LongLength)
            {
                Console.WriteLine("错误的封包文件, 文件名表长度不匹配");
                return false;
            }

            ReadOnlySpan<FileEntry> fileEntries = MemoryMarshal.Cast<byte, FileEntry>(entryTableBytes);

            //读资源
            string packageName = Path.GetFileNameWithoutExtension(packagePath);
            string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Static_Extract", packageName);

            for(uint idx = 0u; idx < fileHeader.FileCount; ++idx)
            {
                FileEntry entry = fileEntries[(int)idx];

                string filePath = Encoding.UTF8.GetString(pathTableBytes, (int)entry.FilePathOffset, (int)entry.FilePathSize);
                string extractPath = Path.Combine(outputDir, filePath);

                {
                    if (Path.GetDirectoryName(extractPath) is string dir && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                inFs.Position = fileHeader.ResourceTableOffset + entry.FileOffset;

                byte[] data = new byte[entry.FileSize];
                if(inFs.Read(data, 0, (int)entry.FileSize) == data.LongLength)
                {
                    File.WriteAllBytes(extractPath, data);
                    Console.WriteLine("{0} 提取成功", filePath);
                }
                else
                {
                    Console.WriteLine("{0} 提取失败", filePath);
                }
            }
            Console.WriteLine("{0}封包提取成功", packageName);
            return true;
        }

        /// <summary>
        /// 读取结构体
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="stream">流</param>
        private static T Read<T>(Stream stream) where T : struct
        {
            Span<byte> buf = stackalloc byte[Unsafe.SizeOf<T>()];
            if(stream.Read(buf) == buf.Length)
            {
                return MemoryMarshal.Read<T>(buf);
            }
            else
            {
                return default;
            }
        }
    }
}
