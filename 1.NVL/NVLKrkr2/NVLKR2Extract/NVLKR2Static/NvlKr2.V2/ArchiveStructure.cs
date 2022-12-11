
using System.Runtime.InteropServices;

namespace NvlKr2Extract.V2
{
    public class ArchiveStructure
    {
        /// <summary>
        /// 资源类型
        /// </summary>
        public enum ArchiveType:uint
        {
            /// <summary>
            /// 一般资源
            /// </summary>
            NormalArchive=0,
            /// <summary>
            /// 压缩资源
            /// </summary>
            CompressedArchive=1
        }
        /// <summary>
        /// 文件表结构
        /// </summary>
        [StructLayout(LayoutKind.Explicit,Pack =1)]
        public struct FileTable
        {
            /// <summary>
            /// Filed标记
            /// </summary>
            [FieldOffset(0x00)]
            public ulong FileSignature;

            [FieldOffset(0x08)]
            public uint Reserve1;

            /// <summary>
            /// Info标记
            /// </summary>
            [FieldOffset(0x0C)]
            public ulong InfoSignature;

            [FieldOffset(0x14)]
            public uint Reserve2;

            [FieldOffset(0x18)]
            public uint Reserve3;

            [FieldOffset(0x1C)]
            public uint DeCompressedSizeLow1;

            [FieldOffset(0x20)]
            public uint DeCompressedSizeHigh1;

            [FieldOffset(0x24)]
            public uint FileSizeLow1;

            [FieldOffset(0x28)]
            public uint FileSizeHigh1;

            /// <summary>
            /// 文件Hash？？？
            /// </summary>
            [FieldOffset(0x2C)]
            public uint Hash1;
            /// <summary>
            /// 文件Hash？？？
            /// </summary>
            [FieldOffset(0x30)]
            public uint Hash2;
            /// <summary>
            /// 文件Hash？？？
            /// </summary>
            [FieldOffset(0x34)]
            public uint Hash3;

            /// <summary>
            /// Segm标记
            /// </summary>
            [FieldOffset(0x38)]
            public ulong SegmSignature;

            [FieldOffset(0x40)]
            public uint Reserve4;

            /// <summary>
            /// 资源类型
            /// </summary>
            [FieldOffset(0x44)]
            public ArchiveType ArchiveType;

            /// <summary>
            /// 文件偏移低32位
            /// </summary>
            [FieldOffset(0x48)]
            public uint FileOffsetLow;
            /// <summary>
            /// 文件偏移高32位
            /// </summary>
            [FieldOffset(0x4C)]
            public uint FileOffsetHigh;

            /// <summary>
            /// 解压缩后大小低32位
            /// </summary>
            [FieldOffset(0x50)]
            public uint DeCompressedSizeLow;
            /// <summary>
            /// 解压缩后大小高32位
            /// </summary>
            [FieldOffset(0x54)]
            public uint DeCompressedSizeHigh;

            /// <summary>
            /// 文件大小低32位
            /// </summary>
            [FieldOffset(0x58)]
            public uint FileSizeLow;
            /// <summary>
            /// 文件大小高32位
            /// </summary>
            [FieldOffset(0x5C)]
            public uint FileSizeHigh;

            /// <summary>
            /// Adlr标记
            /// </summary>
            [FieldOffset(0x60)]
            public ulong AdlrSignature;


            [FieldOffset(0x68)]
            public uint Reserve6;

            /// <summary>
            /// 资源文件Key
            /// </summary>
            [FieldOffset(0x6C)]
            public uint FileKey;

            /// <summary>
            /// 获取资源文件偏移
            /// </summary>
            public ulong FileOffset => (((ulong)this.FileOffsetHigh) << 32) + this.FileOffsetLow;

            /// <summary>
            /// 获取资源文件大小
            /// </summary>
            public ulong FileSize => (((ulong)this.FileSizeHigh) << 32) + this.FileSizeLow;
        }
        /// <summary>
        /// 文件头
        /// </summary>
        [StructLayout(LayoutKind.Explicit,Pack =1)]
        public struct Header
        {
            [FieldOffset(0x00)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0B)]
            public byte[] Signature;

            [FieldOffset(0x0B)]
            public uint Unknow1;

            [FieldOffset(0x0F)]
            public uint Unknow2;

            [FieldOffset(0x13)]
            public uint Unknow3;

            [FieldOffset(0x17)]
            public byte Unknow4;

            [FieldOffset(0x18)]
            public uint Reserve1;
            [FieldOffset(0x1C)]
            public uint Reserve2;

            /// <summary>
            /// 文件表偏移低32位
            /// </summary>
            [FieldOffset(0x20)]
            public uint TableInfoOffsetLow;
            /// <summary>
            /// 文件表偏移高32位
            /// </summary>
            [FieldOffset(0x24)]
            public uint TableInfoOffsetHigh;
            /// <summary>
            /// 获取表信息偏移
            /// </summary>
            public ulong TableInfoOffset => (((ulong)this.TableInfoOffsetHigh) << 32) + this.TableInfoOffsetLow;
        }
        /// <summary>
        /// 文件表信息结构
        /// </summary>
        [StructLayout(LayoutKind.Explicit,Pack =1)]
        public struct TableInfo
        {
            /// <summary>
            /// 表类型
            /// </summary>
            [FieldOffset(0x00)]
            public byte FileType;
            /// <summary>
            /// 表压缩大小低32位
            /// </summary>
            [FieldOffset(0x01)]
            public uint CompressedSizeLow;
            /// <summary>
            /// 表压缩大小高32位
            /// </summary>
            [FieldOffset(0x05)]
            public uint CompressedSizeHigh;
            /// <summary>
            /// 表解压后大小低32位
            /// </summary>
            [FieldOffset(0x09)]
            public uint DecompressedSizeLow;
            /// <summary>
            /// 表解压后大小高32位
            /// </summary>
            [FieldOffset(0x0D)]
            public uint DecompressedSizeHigh;

            /// <summary>
            /// 压缩类型
            /// </summary>
            public bool IsCompressed => this.FileType == 0x01;
            /// <summary>
            /// 获取表压缩大小
            /// </summary>
            public ulong CompressedSize=> (((ulong)this.CompressedSizeHigh) << 32) + this.CompressedSizeLow;

            /// <summary>
            /// 获取表解压缩大小
            /// </summary>
            public ulong DecompressedSize => (((ulong)this.DecompressedSizeHigh) << 32) + this.DecompressedSizeLow;
        }
    }
}
