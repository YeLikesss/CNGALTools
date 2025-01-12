using System;
using System.Collections.Generic;

namespace XP3
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
            public long FileInfoSize;
            /// <summary>
            /// info标记 I
            /// </summary>
            public uint InfoSign;
            /// <summary>
            /// 基本信息大小
            /// </summary>
            public long BaseInfoSize;
            /// <summary>
            /// 加密标记
            /// </summary>
            public uint Protect;
            /// <summary>
            /// 文件原始大小(解压后)
            /// </summary>
            public long FileOriginalSize;
            /// <summary>
            /// 文件实际大小(解压前)
            /// </summary>
            public long FileActuallySize;
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
            public long FileSegmSize;

            /// <summary>
            /// 段结构
            /// </summary>
            public List<XP3FileSegment> Segments;

            /// <summary>
            /// adlr标记
            /// </summary>
            public uint AdlrSign;
            /// <summary>
            /// 文件附加数据大小
            /// </summary>
            public long FileAdlrSize;
            /// <summary>
            /// Hash
            /// </summary>
            public uint Hash;
        }

        /// <summary>
        /// 封包数据块
        /// </summary>
        public struct XP3FileSegment
        {
            /// <summary>
            /// 压缩标记
            /// </summary>
            public uint Compress;
            /// <summary>
            /// 文件在封包内偏移
            /// </summary>
            public long FileOffset;
            /// <summary>
            /// 文件原始大小(解压后)
            /// </summary>
            public long DecompressedSize;
            /// <summary>
            /// 文件实际大小(解压前)
            /// </summary>
            public long CompressedSize;
            /// <summary>
            /// 获取文件是否已压缩
            /// </summary>
            public readonly bool IsCompressed => this.Compress == 0x00000001;
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
            public long CompressedSize;
            /// <summary>
            /// 表原始大小(解压后)
            /// </summary>
            public long DecompressedSize;

            /// <summary>
            /// 文件信息表
            /// </summary>
            public byte[] InfoData { get; set; }
            /// <summary>
            /// 获取表是否已压缩
            /// </summary>
            public readonly bool IsCompressed => this.Compress == 0x01;
        }
    }
}
