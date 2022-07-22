using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Buffers;

namespace Extractor.Renpy
{
    /// <summary>
    /// RPYC脚本相关
    /// </summary>
    public class Script
    {
        public static string RPYCSignature => "RENPY RPC2";


        /// <summary>
        /// 单个脚本解包
        /// </summary>
        /// <param name="filePath">脚本路径</param>
        public static void RPYCUnpake(string filePath)
        {
            using FileStream rpycFS = File.OpenRead(filePath);
            using BinaryReader rpycBR = new(rpycFS);

            
            Span<byte> keys = stackalloc byte[4];       //用于存放key

            //读取key
            rpycFS.Seek(48, SeekOrigin.Begin);
            rpycBR.Read(keys);

            //导出路径
            string scriptName = Path.GetFileNameWithoutExtension(filePath);
            string extractPath = Path.GetDirectoryName(filePath) + "\\ScriptExtract\\" + scriptName + "\\";

            //检查文件夹
            if (Directory.Exists(extractPath) == false)
            {
                Directory.CreateDirectory(extractPath);
            }

            //读取脚本table
            rpycFS.Seek(RPYCSignature.Length, SeekOrigin.Begin);

            long tablePosition = rpycFS.Position;   //保存当前表位置

            int slot, start, length;
            while (true)
            {
                //读表
                slot = rpycBR.ReadInt32();
                start = rpycBR.ReadInt32();
                length = rpycBR.ReadInt32();

                tablePosition = rpycFS.Position;    //保存当前表位置

                //读取完毕
                if (slot == 0)
                {
                    break;
                }

                //解密信息
                start = start ^ keys[0] ^ keys[3];
                length = length ^ keys[1] ^ keys[2];

                //读取封包
                byte[] compressedData = ArrayPool<byte>.Shared.Rent(length);
                rpycFS.Seek(start, SeekOrigin.Begin);
                rpycBR.Read(compressedData, 0, length);

                //解压导出
                byte[] rawData = Zlib.Decompress(compressedData);
                File.WriteAllBytes(extractPath + slot.ToString() + ".bin", rawData);

                ArrayPool<byte>.Shared.Return(compressedData);  //释放
                rpycFS.Seek(tablePosition, SeekOrigin.Begin);   //回到下一个表的起始点
            }
        }

        /// <summary>
        /// 解包多个脚本
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        public static void RPYCsUnpake(string directoryPath)
        {
            DirectoryInfo dirPathInfo = new(directoryPath);

            FileInfo[] rpycFileInfos = dirPathInfo.GetFiles("*.rpyc");  //枚举脚本文件

            //遍历并提取脚本文件
            foreach(FileInfo rpycFileInfo in rpycFileInfos)
            {
                RPYCUnpake(rpycFileInfo.FullName);
            }

            DirectoryInfo[] subDirInfos = dirPathInfo.GetDirectories(); //枚举子文件夹

            //遍历递归子文件夹
            foreach(DirectoryInfo subdir in subDirInfos)
            {
                RPYCsUnpake(subdir.FullName);
            }
        }


    }
}
