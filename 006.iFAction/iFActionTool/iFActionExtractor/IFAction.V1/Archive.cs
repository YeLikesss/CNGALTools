using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Buffers;

namespace IFAction.V1
{
    public class Archive
    {
        public struct FileTable
        {
            public int Offset;
            public int Size;
            public string Name;
        }

        /// <summary>
        /// 提取封包资源
        /// </summary>
        /// <param name="archivePath">封包路径</param>
        /// <returns></returns>
        public bool Extract(string archivePath)
        {
            using FileStream archiveStream = File.OpenRead(archivePath);
            using BinaryReader archiveReader = new(archiveStream);

            archiveStream.Seek(0, SeekOrigin.Begin);

            //Signature
            if (this.ReadUTF8String(archiveReader, 6) != "iFFile")
            {
                return false;
            }

            //数据长度
            int dataSize = archiveReader.ReadInt32();

            //读取文件表项数
            int tableCount = archiveReader.ReadInt32();

            //读取并储存文件表
            List<FileTable> archiveTables = new(tableCount);
            while (tableCount != 0)
            {
                FileTable ft = new();
                ft.Offset = archiveReader.ReadInt32();      //数据偏移
                ft.Size = archiveReader.ReadInt32();        //数据大小
                ft.Name = this.ReadUTF8String(archiveReader);   //文件相对路径

                archiveTables.Add(ft);
                --tableCount;
            }

            //提取目标文件夹路径
            string outputDirPath = Path.Combine(Path.GetDirectoryName(archivePath), "Extract_Static", Path.GetFileNameWithoutExtension(archivePath));

            //数据段偏移
            long dataAreaPos = archiveStream.Position;

            //读取并导出
            foreach(FileTable archiveInfo in archiveTables)
            {
                byte[] buffer = ArrayPool<byte>.Shared.Rent(archiveInfo.Size);  //申请公用内存

                archiveStream.Seek(dataAreaPos + archiveInfo.Offset, SeekOrigin.Begin);  //数据段偏移+文件偏移
                archiveStream.Read(buffer, 0, archiveInfo.Size);    //读取资源


                string outputArchivePath = Path.Combine(outputDirPath, archiveInfo.Name);  //导出文件路径
                string outputArchiveDir = Path.GetDirectoryName(outputArchivePath);     //导出文件文件夹路径
                //检查导出文件夹是否存在 不存在创建
                if (Directory.Exists(outputArchiveDir) == false)
                {
                    Directory.CreateDirectory(outputArchiveDir);
                }

                //回写导出
                FileStream extractStream = new(outputArchivePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                extractStream.Write(buffer, 0, archiveInfo.Size);
                extractStream.Flush();
                extractStream.Dispose();

                ArrayPool<byte>.Shared.Return(buffer);      //释放归还公用内存
            }
            return true;
        }

        /// <summary>
        /// 读取UTF-8字符串 (数据头含4字节用于表示长度)
        /// </summary>
        /// <param name="read">读取流</param>
        /// <returns></returns>
        private string ReadUTF8String(BinaryReader read)
        {
            int length = read.ReadInt32();
            return Encoding.UTF8.GetString(read.ReadBytes(length));
        }
        /// <summary>
        /// 读取UTF-8字符串 (指定长度模式)
        /// </summary>
        /// <param name="read">读取流</param>
        /// <param name="length">字节长度</param>
        /// <returns></returns>
        private string ReadUTF8String(BinaryReader read, int length)
        {
            return Encoding.UTF8.GetString(read.ReadBytes(length));
        }
    }
}
