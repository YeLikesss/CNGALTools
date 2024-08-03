using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EngineCoreStatic
{
    /// <summary>
    /// HAC封包
    /// </summary>
    public class HACPackage : IDisposable
    {
        /// <summary>
        /// HAC文件压缩类型
        /// </summary>
        private enum HACCompress : uint
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknow = 0u,
            /// <summary>
            /// 没压缩
            /// </summary>
            NoCompress = 1u,
            /// <summary>
            /// Lzma压缩
            /// </summary>
            Lzma = 2u,
        }

        /// <summary>
        /// HAC文件夹
        /// </summary>
        private class HACDirectory
        {
            /// <summary>
            /// 文件夹名称
            /// </summary>
            public string Name = string.Empty;
            /// <summary>
            /// 子文件夹
            /// </summary>
            public List<HACDirectory> SubDirectories = new();
            /// <summary>
            /// 子文件
            /// </summary>
            public List<HACFileEntry> SubFiles = new();
        }

        /// <summary>
        /// HAC文件
        /// </summary>
        private class HACFileEntry
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string Name = string.Empty;
            /// <summary>
            /// 偏移
            /// </summary>
            public uint Offset;
            /// <summary>
            /// 文件大小
            /// </summary>
            public uint FileSize;
            /// <summary>
            /// 实际大小
            /// </summary>
            public uint ActualSize;
            /// <summary>
            /// 压缩方式
            /// </summary>
            public HACCompress CompressType;
            /// <summary>
            /// MD5 Hash
            /// </summary>
            public byte[] MD5 = Array.Empty<byte>();
        }

        private readonly string mPackageName = string.Empty;         //封包名
        private string mOutputDirectory = string.Empty;              //解包根目录
        private Stream mStream = Stream.Null;                        //资源流
        private readonly HACDirectory mRootDirectory = new();        //文件表根目录
        
        private bool mIsVaild = false;                               //封包合法性

        /// <summary>
        /// 获取封包是否合法
        /// </summary>
        public bool IsVaild => this.mIsVaild;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">文件绝对路径</param>
        public HACPackage(string filename)
        {
            if (File.Exists(filename))
            {
                this.mPackageName = Path.GetFileNameWithoutExtension(filename);
                this.Initialize(File.OpenRead(filename));
            }
        }

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="outputDirectory">目标路径</param>
        public void Extract(string outputDirectory)
        {
            if (!this.mIsVaild)
            {
                return;
            }

            string dir = Path.Combine(outputDirectory, this.mPackageName);
            this.mOutputDirectory = dir;
            this.Extract(dir, this.mRootDirectory);
        }

        /// <summary>
        /// 解包文件夹节点
        /// </summary>
        /// <param name="outputDirectory">目标路径</param>
        /// <param name="directoryNode">封包文件夹对象</param>
        private void Extract(string outputDirectory, HACDirectory directoryNode)
        {
            string dir = Path.Combine(outputDirectory, directoryNode.Name);

            foreach(HACDirectory subDir in directoryNode.SubDirectories)
            {
                this.Extract(dir, subDir);
            }

            foreach(HACFileEntry subFile in directoryNode.SubFiles)
            {
                this.ExtractFile(dir, subFile);
            }
        }

        /// <summary>
        /// 解包文件
        /// </summary>
        /// <param name="outputDirectory">目标路径</param>
        /// <param name="entry">文件信息</param>
        private void ExtractFile(string outputDirectory, HACFileEntry entry)
        {
            string filePath = Path.Combine(outputDirectory, entry.Name);
            string relativePath = filePath[(this.mOutputDirectory.Length + 1)..];

            {
                if (Path.GetDirectoryName(filePath) is string dir && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            using FileStream outFs = File.Create(filePath);

            Stream stream = this.mStream;
            stream.Seek(entry.Offset, SeekOrigin.Begin);             //封包文件偏移

            int actualSize = (int)entry.ActualSize;                  //实际大小
            int fileSize = (int)entry.FileSize;                      //文件大小

            byte[] fileData = new byte[fileSize];
            stream.Read(fileData, 0, fileSize);
            using MemoryStream inMs = new(fileData, 0, fileSize, false);

            switch (entry.CompressType)
            {
                case HACCompress.Unknow:
                {
                    //未知压缩
                    Console.WriteLine("未知压缩格式: {0}", entry.Name);
                    outFs.Write(fileData, 0, actualSize);
                    break;
                }
                case HACCompress.NoCompress:
                {
                    outFs.Write(fileData, 0, actualSize);    //未压缩 直写

                    string ext = Path.GetExtension(entry.Name);
                    switch (ext)
                    {
                        case ".hgp":
                        {
                            HACHgpImageDecoder hgpDecoder = new(inMs, entry.Name);
                            hgpDecoder.ExtractPNG(outputDirectory);

                            break;
                        }
                        case ".tex":
                        {
                            HACTexImageDecoder texDecoder = new(inMs, entry.Name);
                            texDecoder.ExtractToPNG(outputDirectory);

                            break;
                        }
                    }

                    break;
                }
                case HACCompress.Lzma:
                {
                    HACDecompressor.DecompressLzma(inMs, outFs, fileSize, actualSize);
                    break;
                }
            }
            Console.WriteLine("成功: {0}", relativePath);
        }


        public void Dispose()
        {
            if(this.mStream != Stream.Null)
            {
                this.mStream.Dispose();
                this.mStream = Stream.Null;
            }
            this.mIsVaild = false;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stream">资源流</param>
        private void Initialize(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            using BinaryReader br = new(stream, Encoding.Unicode, true);
            // "HAC-3"头
            if(br.ReadUInt32() == 0x2D434148u && br.ReadUInt16() == 0x1A33)
            {
                stream.Seek(-4L, SeekOrigin.End);

                //文件表起始点
                stream.Seek(br.ReadUInt32(), SeekOrigin.Begin);

                //解析文件表
                HACPackage.ParseEntry(stream, this.mRootDirectory);
                    
                this.mStream = stream;
                this.mIsVaild = true;
            }
        }

        /// <summary>
        /// 解析文件表
        /// </summary>
        /// <returns>True解析成功 False解析失败</returns>
        private static void ParseEntry(Stream stream, HACDirectory directoryNode)
        {
            using BinaryReader br = new(stream, Encoding.Unicode, true);

            //解析节点名称
            directoryNode.Name = HACStreamExtend.ReadString(stream).ToLower();

            //解析子文件夹
            {
                List<HACDirectory> subDirs = directoryNode.SubDirectories;

                int subDirectoryCount = br.ReadInt32();

                subDirs.Capacity = subDirectoryCount;
                for (int i = 0; i < subDirectoryCount; ++i)
                {
                    HACDirectory dir = new();

                    HACPackage.ParseEntry(stream, dir);
                    subDirs.Add(dir);
                }
            }

            //解析子文件
            {
                List<HACFileEntry> subFiles = directoryNode.SubFiles;

                int subFileCount = br.ReadInt32();

                subFiles.Capacity = subFileCount;
                for(int i = 0; i < subFileCount; ++i)
                {
                    HACFileEntry fileEntry = new();

                    //文件名
                    fileEntry.Name = HACStreamExtend.ReadString(stream).ToLower();

                    //压缩方式
                    string compressMode = HACStreamExtend.ReadString(stream);
                    if (string.IsNullOrEmpty(compressMode))
                    {
                        fileEntry.CompressType = HACCompress.NoCompress;
                    }
                    else
                    {
                        switch (compressMode)
                        {
                            case "Lzma":
                            {
                                fileEntry.CompressType = HACCompress.Lzma;
                                break;
                            }
                            default:
                            {
                                Debugger.Break();
                                fileEntry.CompressType = HACCompress.Unknow;
                                break;
                            }
                        }
                    }

                    //文件大小
                    fileEntry.ActualSize = br.ReadUInt32();
                    fileEntry.FileSize = br.ReadUInt32();

                    //MD5 Hash
                    fileEntry.MD5 = br.ReadBytes(16);

                    //文件偏移
                    fileEntry.Offset = br.ReadUInt32();

                    subFiles.Add(fileEntry);
                }
            }
        }
    }
}