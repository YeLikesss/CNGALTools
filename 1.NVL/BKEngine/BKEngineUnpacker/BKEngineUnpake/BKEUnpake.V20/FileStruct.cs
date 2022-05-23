using System;
using System.Runtime.InteropServices;

namespace BKEUnpake.V20
{
    /// <summary>
    /// 文件表封包结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FilePackageListInfo
    {
        /// <summary>
        /// 资源表长度
        /// </summary>
        public uint ListDataSize;
        /// <summary>
        /// 资源表项计数
        /// </summary>
        public uint ListCount;
        /// <summary>
        /// 资源表解密listkey
        /// </summary>
        public uint ListDecryptKey;
    }
    /// <summary>
    /// 压缩资源位于资源表中的结构
    /// </summary>
    public struct BZip2CompressedResources
    {
        /// <summary>
        /// 文件名Hash
        /// </summary>
        public string FileName;
        /// <summary>
        /// 资源在文件中的偏移
        /// </summary>
        public uint FileOffset;
        /// <summary>
        /// 资源解压后的大小
        /// </summary>
        public uint UncompressedSize;
        /// <summary>
        /// 资源类型  压缩包型则为1
        /// </summary>
        public uint ResourcesType;
        /// <summary>
        /// 资源在文件中的大小
        /// </summary>
        public uint FileSize;
    }
    /// <summary>
    /// 普通资源位于资源表中的结构
    /// </summary>
    public struct NormalResources
    {
        /// <summary>
        /// 文件名Hash
        /// </summary>
        public string FileName;
        /// <summary>
        /// 资源在文件中的偏移
        /// </summary>
        public uint FileOffset;
        /// <summary>
        /// 资源在文件中的大小
        /// </summary>
        public uint FileSize;
        /// <summary>
        /// 资源类型  普通型则为0
        /// </summary>
        public uint ResourcesType;
    }
}
