using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Text;
using System.Buffers;
using Extractor.ZixRenpy7V1.Crypto;
using Extractor.Untils;

namespace Extractor.ZixRenpy7V1.Renpy
{
    /// <summary>
    /// 资源文件
    /// </summary>
    public class Archive
    {
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
            mCrypto = new(key, xorVector);
            mCrypto.Initialize(substitutionBox1, substitutionBox2, substitutionBox3, substitutionBox4, substitutionBox5, substitutionBox6, substitutionBox7, substitutionBox8);

            //设置导出文件夹
            outDir = outDir.Trim();
            if (outDir.Last() != '\\' || outDir.Last() != '/')
            {
                outDir += "\\";
            }
            ExtractOutputDir = outDir;
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="upperDirName">上级文件夹名字</param>
        /// <param name="directory">文件夹信息</param>
        public void DecryptFile(string upperDirName, DirectoryInfo directory)
        {
            //设置导出路径
            string extractDir = string.Concat(ExtractOutputDir, upperDirName, directory.Name, "/");
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
                for (int loop = 0; loop < loopCount; loop++)
                {
                    //指向待解密数据指针
                    Span<byte> temp = new Span<byte>(buffer, loop * 16, 16);
                    //一次解密16字节
                    mCrypto.Decrypt16BytesData(temp);
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
                DecryptFile(string.Concat(upperDirName, directory.Name, "/"), subdir);
            });
        }

    }
}
