using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using NvlKr2Extract;

namespace NvlKr2Extract.V2
{
    public class ArchiveFile
    {
        private FileInfo arcFileInfo;
        private byte[] mFileData;
        private Dictionary<ArchiveStructure.FileTable, byte[]> mArchiveInfo;
        /// <summary>
        /// 读取分析资源文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void Analysis(string filePath)
        {
            try
            {
                this.arcFileInfo = new FileInfo(filePath);  //设置文件信息
                this.mFileData = File.ReadAllBytes(this.arcFileInfo.FullName);  //读取文件

                //读取头
                ArchiveStructure.Header arcHeader = StructureConvert.GetStructure<ArchiveStructure.Header>(this.mFileData);

                //读取表信息
                ArchiveStructure.TableInfo arcTableInfo = StructureConvert.GetStructure<ArchiveStructure.TableInfo>(this.mFileData,(int)arcHeader.TableInfoOffset);

                //获取表偏移
                int tableOffset = (int)arcHeader.TableInfoOffset + Marshal.SizeOf(typeof(ArchiveStructure.TableInfo));

                //读取表
                byte[] tableData = new byte[arcTableInfo.CompressedSize];
                Array.Copy(this.mFileData, tableOffset, tableData, 0, tableData.Length);

                //分析获取表项
                List<ArchiveStructure.FileTable> fileTables;
                if (arcTableInfo.IsCompressed)
                {
                    byte[] decompressedTableData = Zlib.Decompress(tableData);  //解压数据
                    fileTables = ArchiveTable.Analysis(decompressedTableData);  //分析表
                }
                else
                {
                    fileTables = ArchiveTable.Analysis(tableData);  //分析表
                }

                if (SystemConfig.ConsoleLogEnable)
                {
                    Console.WriteLine(string.Concat(this.arcFileInfo.Name, "  表项分析完毕"));
                }

                //获取资源数据
                this.mArchiveInfo = new Dictionary<ArchiveStructure.FileTable, byte[]>();
                fileTables.ForEach(fileTable => 
                {
                    //读取资源
                    byte[] arcData = new byte[fileTable.FileSize];
                    Array.Copy(this.mFileData, (long)fileTable.FileOffset, arcData, 0, arcData.Length);

                    //判断资源类型
                    switch (fileTable.ArchiveType)
                    {
                        case ArchiveStructure.ArchiveType.NormalArchive:   //普通数据
                            this.mArchiveInfo.Add(fileTable, arcData);      //添加资源信息
                            break;
                        case ArchiveStructure.ArchiveType.CompressedArchive:    //压缩数据
                            byte[] buffer = Zlib.Decompress(arcData);       //解压数据
                            this.mArchiveInfo.Add(fileTable, buffer);      //添加资源信息
                            break;
                    }
                });

                if (SystemConfig.ConsoleLogEnable)
                {
                    Console.WriteLine(string.Concat(this.arcFileInfo.Name, "  数据读取完毕"));
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 解密资源
        /// </summary>
        /// <param name="constKey1">常量Key1</param>
        /// <param name="constKey2">常量Key2</param>
        public void Decrypt(uint constKey1,uint constKey2)
        {
            if (SystemConfig.ConsoleLogEnable)
            {
                Console.WriteLine(string.Concat("Key1 = ",constKey1.ToString("X8"),"    Key2 = ",constKey2.ToString("X8")));
            }

            //循环解密
            foreach (KeyValuePair<ArchiveStructure.FileTable, byte[]> arc in this.mArchiveInfo)
            {
                //初始化key
                ArchiveCrypto arcCrypt = new ArchiveCrypto
                {
                    ConstXorKey1 = constKey1,
                    ConstXorKey2 = constKey2
                };
                arcCrypt.Decrypt(arc.Value, arc.Key.FileKey, arc.Value.Length);     //解密资源


            }
        }
        /// <summary>
        /// 提取导出资源
        /// </summary>
        public void Extract()
        {
            //导出文件夹
            string subDir = string.Concat(this.arcFileInfo.DirectoryName, "/Extract/", this.arcFileInfo.Name.Split('.').ElementAt(0),"/");
            //检查文件夹
            if (Directory.Exists(subDir) == false)
            {
                Directory.CreateDirectory(subDir);  //创建文件夹
            }

            //循环导出
            foreach (KeyValuePair<ArchiveStructure.FileTable, byte[]> arc in this.mArchiveInfo)
            {
                //设置文件名
                string fileName;

                fileName = Hasher.GetFileName(arc.Key.Hash1,arc.Key.Hash2,arc.Key.Hash3);
                try
                {
                    File.WriteAllBytes(string.Concat(subDir, fileName), arc.Value); //写入文件
                }
                catch
                {
                }

                if (SystemConfig.ConsoleLogEnable)
                {
                    Console.WriteLine(string.Concat(this.arcFileInfo.Name, "/", fileName, "  已导出"));
                }
            }
        }
    }
}
