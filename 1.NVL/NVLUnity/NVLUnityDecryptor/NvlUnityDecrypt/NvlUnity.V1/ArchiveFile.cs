using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace NvlUnity.V1
{
    public class ArchiveFile
    {
        private FileInfo mFileInfo;
        private MemoryMappedFile mMappedFile;
        private MemoryMappedViewAccessor mFileData;
        private MemoryMappedViewAccessor mDecryptData;
        /// <summary>
        /// 分析封包
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void Analysis(string filePath)
        {
            try
            {
                this.mFileInfo = new FileInfo(filePath);
                this.mMappedFile = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 解密封包
        /// </summary>
        /// <param name="constKey1">游戏主程序Key1</param>
        /// <param name="constKey2">游戏主程序Key2</param>
        /// <param name="constKey3">游戏主程序Key3</param>
        /// <param name="version">Unity版本</param>
        private void Decrypt(uint constKey1,uint constKey2,uint constKey3,ArchiveHeader.UnityVersion version)
        {
            ArchiveCrypto archiveCrypto = new()
            {
                ConstXorKey1 = constKey1,
                ConstXorKey2 = constKey2,
                ConstXorKey3 = constKey3
            };

            //获取数据
            this.mFileData = this.mMappedFile.CreateViewAccessor(0, this.mFileInfo.Length, MemoryMappedFileAccess.Read);

            //封包解密
            archiveCrypto.Decrypt(this.mFileData,this.mDecryptData, this.mFileInfo.Length);

            //修复UnityFs头
            Fix.UnityFSHeader(this.mDecryptData, version);          
        }
        /// <summary>
        /// 提取封包
        /// </summary>
        /// <param name="constKey1">游戏主程序Key1</param>
        /// <param name="constKey2">游戏主程序Key2</param>
        /// <param name="constKey3">游戏主程序Key3</param>
        /// <param name="version">Unity版本</param>
        public void Extract(uint constKey1, uint constKey2, uint constKey3, ArchiveHeader.UnityVersion version)
        {
            string subdir = string.Concat(this.mFileInfo.DirectoryName, "/Extract/");       //设置导出文件夹
            //检查文件夹是否存在 不存在则创建
            if (Directory.Exists(subdir) == false)
            {
                Directory.CreateDirectory(subdir);
            }

            //设置导出文件路径
            string filePath = string.Concat(subdir, this.mFileInfo.Name.Split(".").ElementAt(0), ".asset"); 
            try
            {
                //创建解密后资源文件
                MemoryMappedFile mappedFile = MemoryMappedFile.CreateFromFile(filePath,FileMode.Create,null,this.mFileInfo.Length,MemoryMappedFileAccess.ReadWrite);
                //获取解密后资源文件数据流
                this.mDecryptData = mappedFile.CreateViewAccessor(0,this.mFileInfo.Length,MemoryMappedFileAccess.ReadWrite);

                //解密数据
                this.Decrypt(constKey1, constKey2, constKey3, version);

                //Log打印
                if (SystemConfig.ConsoleLogEnable)
                {
                    Console.WriteLine(string.Concat(Path.GetFileName(this.mFileInfo.FullName),"    解密成功"));
                }

                //清空缓存写入磁盘
                this.mDecryptData.Flush();
                this.mDecryptData.Dispose();
                this.mFileData.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
