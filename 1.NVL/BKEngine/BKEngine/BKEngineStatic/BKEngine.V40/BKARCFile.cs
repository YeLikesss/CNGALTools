using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace BKEngine.V40
{
    public class BKARCFile
    {
        private byte[] mFileData;
        private FileHeader mFileHeader;
        private TableKeyGroup mTableKeyGroup;
        private FileInfo mArcFile;
        private Dictionary<FileListTable, byte[]> mArchiveData;

        public Dictionary<FileListTable, byte[]> ArchiveData => this.mArchiveData;

        public BKARCFile(string path)
        {
            try
            {
                this.mArcFile = new FileInfo(path);
                this.mFileData=File.ReadAllBytes(this.mArcFile.FullName);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 分析文件
        /// </summary>
        /// <returns>文件表数据</returns>
        public byte[] AnalysisFile()
        {
            try
            {
                //获取文件头
                this.mFileHeader = StructureConvert.GetStructure<FileHeader>(this.mFileData, 0);

                if (this.mFileHeader.IsVaild == false)
                {
                    return null;
                }
                
                //计算Key组文件偏移
                uint keyGroupTableFOA = DecryptHelper.DecryptTableKeyGroupFileOffset(mFileHeader);

                //获取Key组结构体
                this.mTableKeyGroup = StructureConvert.GetStructure<TableKeyGroup>(this.mFileData, (int)keyGroupTableFOA);

                uint DecompressedLength;        //解压缩之后的长度
                //解密文件表压缩包长度
                uint listTableLength = DecryptHelper.DecryptTableSize(this.mFileHeader, this.mTableKeyGroup, out DecompressedLength);   

                //获取文件表压缩包文件偏移
                uint listTableFOA = keyGroupTableFOA + (uint)Marshal.SizeOf(typeof(TableKeyGroup));

                //文件表压缩数据
                byte[] listTableData = new byte[listTableLength];
                Array.Copy(this.mFileData, listTableFOA, listTableData, 0, listTableLength);
                //解密压缩数据
                DecryptHelper.DecryptCompressedTable(listTableData, listTableData.Length, this.mTableKeyGroup.TableKey);
                //解压缩数据
                listTableData = ZstdHelper.Decompress(listTableData);

                return listTableData;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 解密文件数据
        /// </summary>
        /// <param name="uncompressedListData">解压后文件表</param>
        /// <returns></returns>
        public void DecryptArchive(byte[] uncompressedListData)
        {
            this.mArchiveData = new Dictionary<FileListTable, byte[]>();

            //分析文件表
            List<FileListTable> fileListTables = BKARCList.ListTableAnalysis(uncompressedListData);

            fileListTables.ForEach(fileListTable =>
            {
                byte[] buffer;
                switch (fileListTable.FileType)
                {
                    case FileType.NormalArchive:        //普通资源

                        //获取数据
                        buffer = new byte[fileListTable.FileSize];
                        Array.Copy(this.mFileData, fileListTable.FileOffset, buffer, 0, fileListTable.FileSize);

                        //解密得到Key
                        uint xorKey, xorLength;
                        DecryptHelper.DecryptFileKey(fileListTable.Key, out xorKey, out xorLength);

                        //解密文件
                        DecryptHelper.DecryptFile(buffer, xorKey, xorLength);

                        this.mArchiveData.Add(fileListTable, buffer);

                        break;
                    case FileType.CompressedArchive:      //压缩资源

                        //获取数据
                        buffer = new byte[fileListTable.FileSize];
                        Array.Copy(this.mFileData, fileListTable.FileOffset, buffer, 0, fileListTable.FileSize);

                        //修复压缩头
                        buffer = FileFix.CompressedResourcesFix(buffer);

                        //解压数据
                        buffer = ZstdHelper.Decompress(buffer);

                        this.mArchiveData.Add(fileListTable, buffer);

                        break;
                }

            });

        }
        /// <summary>
        /// 导出资源
        /// </summary>
        public void OutputArchiveData()
        {
            string directory = Path.Combine(this.mArcFile.DirectoryName, "Extract", Path.GetFileNameWithoutExtension(this.mArcFile.FullName));
                
            foreach(KeyValuePair<FileListTable,byte[]> singleArcData in this.ArchiveData)
            {
                string filePath = Path.Combine(directory, singleArcData.Key.FileName);
                {
                    string dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                }

                try
                {
                    FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs.Write(singleArcData.Value, 0, singleArcData.Value.Length);
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
                catch 
                {
                }
            }
        }
    }
}
