using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace NvlKr2Extract.V2
{
    public class ArchiveTable
    {
        /// <summary>
        /// 分析资源表
        /// </summary>
        /// <param name="tableData">资源表数据</param>
        /// <returns>表结构</returns>
        public static List<ArchiveStructure.FileTable> Analysis(byte[] tableData)
        {
            List<ArchiveStructure.FileTable> fileTables = new List<ArchiveStructure.FileTable>();

            int tableOffset = 0;        //表偏移

            while (tableOffset < tableData.Length)
            {
                //获取文件表结构体
                ArchiveStructure.FileTable fileTable = StructureConvert.GetStructure<ArchiveStructure.FileTable>(tableData, tableOffset);
                fileTables.Add(fileTable);
                tableOffset += Marshal.SizeOf(typeof(ArchiveStructure.FileTable));
            }
            return fileTables;
        }
    }
}
