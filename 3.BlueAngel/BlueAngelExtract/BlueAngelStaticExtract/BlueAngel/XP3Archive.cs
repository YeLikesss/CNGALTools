using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueAngel
{
    /// <summary>
    /// XP3封包
    /// </summary>
    public class XP3Archive
    {
        /// <summary>
        /// 封包文件列表
        /// </summary>
        public struct XP3File
        {
            /// <summary>
            /// File标记
            /// </summary>
            public uint FileSign;
            /// <summary>
            /// 文件信息大小
            /// </summary>
            public ulong FileInfoSize;
            /// <summary>
            /// info标记 I
            /// </summary>
            public uint InfoSign;
            /// <summary>
            /// 基本信息大小
            /// </summary>
            public ulong BaseInfoSize;
            /// <summary>
            /// 加密标记
            /// </summary>
            public uint Protect;
            /// <summary>
            /// 文件原始大小(解压后)
            /// </summary>
            public ulong FileOriginalSize;
            /// <summary>
            /// 文件实际大小(解压前)
            /// </summary>
            public ulong FileActuallySize;
            /// <summary>
            /// 文件名长度
            /// </summary>
            public ushort FileNameLength;
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileNameUTF16LE;
            /// <summary>
            /// segm标记
            /// </summary>
            public uint SegmSign;
            /// <summary>
            /// 文件段大小
            /// </summary>
            public ulong FileSegmSize;
            /// <summary>
            /// 压缩标记
            /// </summary>
            public uint Compress;
            /// <summary>
            /// 文件在封包内偏移
            /// </summary>
            public ulong FileOffset;
            /// <summary>
            /// 文件原始大小(解压后)
            /// </summary>
            public ulong DecompressedSize;
            /// <summary>
            /// 文件实际大小(解压前)
            /// </summary>
            public ulong CompressedSize;
            /// <summary>
            /// adlr标记
            /// </summary>
            public uint AdlrSign;
            /// <summary>
            /// 文件附加数据大小
            /// </summary>
            public ulong FileAdlrSize;
            /// <summary>
            /// 解密Key
            /// </summary>
            public uint Key;

            /// <summary>
            /// 文件数据
            /// </summary>
            public byte[] FileData { get; set; }
            /// <summary>
            /// 获取文件是否已压缩
            /// </summary>
            public bool IsCompressed => this.Compress == 0x00000001;
        }

        /// <summary>
        /// 文件信息表
        /// </summary>
        public struct XP3Info
        {
            /// <summary>
            /// 表压缩标记
            /// </summary>
            public byte Compress;
            /// <summary>
            /// 表在封包大小(解压前)
            /// </summary>
            public ulong CompressedSize;
            /// <summary>
            /// 表原始大小(解压后)
            /// </summary>
            public ulong DecompressedSize;

            /// <summary>
            /// 文件信息表
            /// </summary>
            public byte[] InfoData { get; set; }
            /// <summary>
            /// 获取表是否已压缩
            /// </summary>
            public bool IsCompressed => this.Compress == 0x01;
        }
    }
}
