using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BKEngine.V40
{
    /// <summary>
    /// 文件头结构
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct FileHeader
    {
        /// <summary>
        /// 文件头标记
        /// </summary>
        [FieldOffset(0)]
        public ulong Signature;
        /// <summary>
        /// 解密Key1
        /// </summary>
        [FieldOffset(8)]
        public uint OffsetKey1;
        /// <summary>
        /// 解密Key2
        /// </summary>
        [FieldOffset(12)]
        public uint OffsetKey2;
        /// <summary>
        /// 解密Key3
        /// </summary>
        [FieldOffset(16)]
        public uint OffsetKey3;

        /// <summary>
        /// 文件头结束全0
        /// </summary>
        [FieldOffset(20)]
        public uint Ender;

        /// <summary>
        /// 获取文件头标记是否合法
        /// </summary>
        public bool IsVaild => (this.Signature & 0x0000FFFFFFFFFFFF) == 0x0000044352414B42;
    }

    /// <summary>
    /// 文件表Key结构体
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct TableKeyGroup
    {
        /// <summary>
        /// 用于计算压缩文件长度
        /// </summary>
        [FieldOffset(0)]
        public uint CompressedSizeKey;
        /// <summary>
        /// 用于计算解压缩文件长度
        /// </summary>
        [FieldOffset(4)]
        public uint UncompressedSizeKey;
        /// <summary>
        /// 解密文件表压缩包Key
        /// </summary>
        [FieldOffset(8)]
        public uint TableKey;
        /// <summary>
        /// key组结束全0
        /// </summary>
        [FieldOffset(12)]
        public uint Ender;
    }

    /// <summary>
    /// 文件数据类型
    /// </summary>
    public enum FileType:uint
    {
        /// <summary>
        /// 一般资源
        /// </summary>
        NormalArchive=0,
        /// <summary>
        /// 文件夹
        /// </summary>
        Directory=1,
        /// <summary>
        /// 压缩资源
        /// </summary>
        CompressedArchive=2
    }

    /// <summary>
    /// 文件表结构
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct FileListTable
    {
        /// <summary>
        /// 文件类型
        /// </summary>
        [FieldOffset(0)]
        public FileType FileType;
        /// <summary>
        /// 文件大小
        /// </summary>
        [FieldOffset(4)]
        public uint FileSize;
        /// <summary>
        /// 文件偏移
        /// </summary>
        [FieldOffset(8)]
        public uint FileOffset;
        /// <summary>
        /// 文件Key(普通资源可用)
        /// </summary>
        [FieldOffset(12)]
        public uint Key;
        /// <summary>
        /// 解压后长度(压缩资源可用)
        /// </summary>
        [FieldOffset(12)]
        public uint DecompressLength;
        /// <summary>
        /// 文件名
        /// </summary>
        [FieldOffset(16)]
        public string FileName;
    }
    /// <summary>
    /// 文件列表数据头
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct ListHeader
    {
        [FieldOffset(0)]
        public uint Unknow1;
        [FieldOffset(4)]
        public uint Unknow2;
        [FieldOffset(8)]
        public uint Unknow3;
        [FieldOffset(12)]
        public uint Ender;
    }

}
