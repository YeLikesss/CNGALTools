using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BKEUnpake.V40
{
    public class BKARCList
    {
        /// <summary>
        /// 分析文件表
        /// </summary>
        /// <param name="listTableData">文件表数据</param>
        /// <returns></returns>
        public static List<FileListTable> ListTableAnalysis(byte[] listTableData)
        {
            List<FileListTable> fileTable = new List<FileListTable>();

            uint listDataPointer =(uint)Marshal.SizeOf(typeof(ListHeader));

            //循环遍历表  长度为表数据长度
            while (listDataPointer < (listTableData.Length-0x11))
            {
                /*获取文件类型    文件大小  文件偏移  文件Key  文件名字符串*/
                FileType fileType = (FileType)BitConverter.ToUInt32(listTableData, (int)listDataPointer);
                listDataPointer += 4;
                uint fileSize = BitConverter.ToUInt32(listTableData, (int)listDataPointer);
                listDataPointer += 4;
                uint fileOffset = BitConverter.ToUInt32(listTableData, (int)listDataPointer);
                listDataPointer += 4;
                uint fileKeyOrDecompressLength = BitConverter.ToUInt32(listTableData, (int)listDataPointer);
                listDataPointer += 4;

                uint fileNameStrLength;
                string fileName = StructureConvert.GetUTF8String(listTableData, listDataPointer, out fileNameStrLength);

                listDataPointer += fileNameStrLength + 1;       //文件表指针指向字符串结束符之后

                listDataPointer = AilgnmentManagar.GetAlignment(listDataPointer, 8);     //8字节对齐

                switch (fileType)
                {
                    case FileType.NormalArchive:       //一般资源
                        FileListTable fileNor = new FileListTable()
                        {
                            FileType = fileType,
                            FileSize=fileSize,
                            FileOffset=fileOffset,
                            Key= fileKeyOrDecompressLength,
                            FileName=fileName
                        };
                        fileTable.Add(fileNor);
                        break;
                    case FileType.Signuature:       //标记
                        break;
                    case FileType.CompressedArchive:      //压缩资源
                        FileListTable fileCom = new FileListTable()
                        {
                            FileType = fileType,
                            FileSize = fileSize,
                            FileOffset = fileOffset,
                            DecompressLength = fileKeyOrDecompressLength,
                            FileName = fileName
                        };
                        fileTable.Add(fileCom);
                        break;
                }

            }
            return fileTable;
        }
    }
}
