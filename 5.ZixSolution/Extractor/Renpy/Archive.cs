using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Text;
using Extractor;
using System.Buffers;

namespace Extractor.Renpy
{
    /// <summary>
    /// 资源文件
    /// </summary>
    public class Archive
    {
        /// <summary>
        /// 解密文件表函数指针
        /// </summary>
        /// <param name="key">资源表key</param>
        /// <param name="skey">资源key</param>
        /// <param name="offset">资源表文件偏移</param>
        public delegate void DecryptTableInfoFunc(ref uint key, ref uint skey, ref uint offset);
        /// <summary>
        /// 解密资源信息函数指针
        /// </summary>
        /// <param name="fileOffset"></param>
        /// <param name="fileSize"></param>
        public delegate void DecryptArchiveInfoFunc(ref long fileOffset, ref long fileSize, uint key);
        /// <summary>
        /// 解密资源头函数指针
        /// </summary>
        /// <param name="header">文件头数据</param>
        /// <param name="skey">解密key</param>
        public delegate void DecryptArchiveHeaderFunc(byte[] header, uint skey);

        /// <summary>
        /// 获取或设置导出主路径文件夹
        /// </summary>
        public string ExtractOutputDir { get; set; }

        private Crypto128 mCrypto;  //加密插件相关

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">游戏key</param>
        /// <param name="xorVector">异或向量</param>
        /// <param name="substitutionBox1">S盒1</param>
        /// <param name="substitutionBox2">S盒2</param>
        /// <param name="substitutionBox3">S盒3</param>
        /// <param name="substitutionBox4">S盒4</param>
        /// <param name="substitutionBox5">S盒5</param>
        /// <param name="substitutionBox6">S盒6</param>
        /// <param name="substitutionBox7">S盒7</param>
        /// <param name="substitutionBox8">S盒8</param>
        /// <param name="outDir">导出路径</param>
        public Archive(byte[] key, byte[] xorVector, 
                       byte[] substitutionBox1, byte[] substitutionBox2, byte[] substitutionBox3,
                       byte[] substitutionBox4, byte[] substitutionBox5, byte[] substitutionBox6,
                       byte[] substitutionBox7, byte[] substitutionBox8, string outDir)
        {
            this.mCrypto = new(key, xorVector);
            this.mCrypto.Initialize(substitutionBox1, substitutionBox2, substitutionBox3, substitutionBox4, substitutionBox5, substitutionBox6, substitutionBox7, substitutionBox8);

            //设置导出文件夹
            outDir = outDir.Trim();
            if (outDir.Last() != '\\' || outDir.Last() != '/')
            {
                outDir += "\\";
            }
            this.ExtractOutputDir = outDir;
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="upperDirName">上级文件夹名字</param>
        /// <param name="directory">文件夹信息</param>
        public void DecryptFile(string upperDirName, DirectoryInfo directory)
        {
            //设置导出路径
            string extractDir = string.Concat(this.ExtractOutputDir, upperDirName, directory.Name, "/");
            //如果不存在则创建文件
            if (Directory.Exists(extractDir) == false)
            {
                Directory.CreateDirectory(extractDir);
            }

            //获取目录子文件
            List<FileInfo> archiveFiles = directory.EnumerateFiles().ToList();

            //遍历文件
            foreach (FileInfo archiveFile in archiveFiles)
            {
                using FileStream fs = File.OpenRead(archiveFile.FullName);
                
                //检查16字节对齐
                if ((fs.Length & 0xF) != 0)
                {
                    continue;
                }
                //申请内存
                byte[] buffer = ArrayPool<byte>.Shared.Rent((int)fs.Length);
                //读取文件
                fs.Read(buffer, 0, (int)fs.Length);

                //设置循环次数
                int loopCount = (int)(fs.Length / 16);

                //循环解密
                for(int loop = 0; loop < loopCount; loop++)
                {
                    //指向待解密数据指针
                    Span<byte> temp = new Span<byte>(buffer, loop * 16, 16);
                    //一次解密16字节
                    this.mCrypto.Decrypt16BytesData_1(temp);
                }

                
                //获取导出长度
                int outLength = (int)(fs.Length - buffer[fs.Length - 1]);

                //获取导出路径
                string outPath = string.Concat(extractDir, archiveFile.Name);

                //写出文件
                using FileStream fsw = new(outPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fsw.Write(buffer, 0, outLength);

                //释放
                ArrayPool<byte>.Shared.Return(buffer);

                //打印log
                Console.WriteLine(string.Concat(archiveFile.Name, "    解密成功"));

            }

            //获取子文件夹
            List<DirectoryInfo> subDirs = directory.EnumerateDirectories().ToList();

            //循环递归
            subDirs.ForEach(subdir =>
            {
                this.DecryptFile(string.Concat(upperDirName, directory.Name, "/"), subdir);
            });
        }

        /// <summary>
        /// 提取RPA封包
        /// </summary>
        /// <param name="archiveDirectory">封包文件夹</param>
        /// <param name="decryptTableInfoFunc">资源表解密函数指针</param>
        /// <param name="decryptArchiveInfoFunc">资源信息解密函数指针</param>
        /// <param name="decryptArchiveHeaderFunc">解密资源头函数指针</param>
        public void RPAExtract(DirectoryInfo archiveDirectory, 
                               DecryptTableInfoFunc decryptTableInfoFunc, 
                               DecryptArchiveInfoFunc decryptArchiveInfoFunc,
                               DecryptArchiveHeaderFunc decryptArchiveHeaderFunc)
        {
            //获取目录子文件
            List<FileInfo> archiveFiles = archiveDirectory.EnumerateFiles().ToList();

            //遍历资源文件
            foreach(FileInfo archiveFile in archiveFiles)
            {
                //过滤掉非资源文件
                if (archiveFile.Extension.ToLower() != ".rpa")
                {
                    continue;
                }

                //开启流读取
                using FileStream archiveFs = File.OpenRead(archiveFile.FullName);
                using BinaryReader archiveBinReader = new(archiveFs);

                archiveFs.Seek(96, SeekOrigin.Begin);   //96字节出开始读取

                //分别读取 文件表key 资源key 文件表offset
                uint key = archiveBinReader.ReadUInt32();
                uint skey = archiveBinReader.ReadUInt32();
                uint tableOffset = archiveBinReader.ReadUInt32();

                //解密
                decryptTableInfoFunc(ref key, ref skey, ref tableOffset);


                //申请内存存放压缩文件表
                int compressTableLength = (int)(archiveFs.Length - tableOffset);
                byte[] compressTable = ArrayPool<byte>.Shared.Rent(compressTableLength);
                //读取文件表
                archiveFs.Seek(tableOffset, SeekOrigin.Begin);
                archiveFs.Read(compressTable, 0, compressTableLength);

                //解压文件表
                byte[] tableData = Zlib.Decompress(compressTable);

                //释放压缩文件表内存
                ArrayPool<byte>.Shared.Return(compressTable);

                //获取文件信息表
                Hashtable tableInfo = (Hashtable)Pickle.Decode(tableData);

                
                //遍历文件表
                foreach(DictionaryEntry archiveInfo in tableInfo)
                {
                    string fileName = (string)archiveInfo.Key;      //获取文件名
                    object[] fileInfo = (object[])((ArrayList)archiveInfo.Value)[0];       //获取文件信息

                    long fileOffset = (long)fileInfo[0];            //获取资源在封包文件偏移
                    long fileSize;
                    //获取资源大小
                    try
                    {
                        fileSize = (long)fileInfo[1];              
                    }
                    catch (InvalidCastException)
                    {
                        fileSize = (int)fileInfo[1];
                    }
                    
                    decryptArchiveInfoFunc(ref fileOffset, ref fileSize, key);   //解密

                    //导出文件路径
                    string extractPath = string.Concat(this.ExtractOutputDir, fileName);
                    //检查文件夹是否存在
                    if (Directory.Exists(Path.GetDirectoryName(extractPath)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(extractPath));
                    }
                    using FileStream archiveExtractFs = new(extractPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);  //创建写入流

                    //申请资源头内存
                    byte[] headerData = ArrayPool<byte>.Shared.Rent(16);

                    //获取资源头
                    ReadOnlySpan<char> headerStr = ((string)fileInfo[2]).AsSpan();
                    if (headerStr.Length == 16)
                    {
                        for (int index = 0; index < 16; index++)
                        {
                            headerData[index] = (byte)(headerStr[index] & 0xFF);
                        }
                        //解密
                        decryptArchiveHeaderFunc(headerData, skey);
                        //写入资源头
                        archiveExtractFs.Write(headerData, 0, 16);
                    }
                   
                    //释放资源
                    ArrayPool<byte>.Shared.Return(headerData);


                    //申请资源数据内存
                    byte[] fileData = ArrayPool<byte>.Shared.Rent((int)fileSize);
                    //读取封包
                    archiveFs.Seek(fileOffset, SeekOrigin.Begin);
                    archiveFs.Read(fileData, 0, (int)fileSize);

                    //回写资源
                    archiveExtractFs.Write(fileData, 0, (int)fileSize);

                    //释放资源
                    ArrayPool<byte>.Shared.Return(fileData);

                    //打印log
                    Console.WriteLine(string.Concat(fileName,"    提取成功"));

                }

            }
        }


    }
}
