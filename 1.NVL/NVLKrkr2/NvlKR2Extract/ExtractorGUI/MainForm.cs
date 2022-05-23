using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using NvlKr2Extract;
using NvlKr2Extract.V2;
using NvlKr2Extract.V2.Games;

namespace ExtractorGUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void File_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listBoxFiles_DragDrop(object sender, DragEventArgs e)
        {
            ListBox lb = sender as ListBox;
            lb.Items.Clear();
            string[] resPaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string path in resPaths)
            {
                lb.Items.Add(path);
            }
        }

        private void panelExeCheck_DragDrop(object sender, DragEventArgs e)
        {
            //获取exe路径
            string exePath = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            bool error = true;

            try
            {
                //读取exe文件
                FileStream exeFs = new FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader exeBinRead = new BinaryReader(exeFs);

                //ImageDosHeader.e_magic
                if (exeBinRead.ReadUInt16() == 0x5A4D)
                {
                    exeFs.Position = 0x3C;  //e_ifnew
                    exeFs.Position = exeBinRead.ReadInt32();
                    int peOffset = (int)exeFs.Position;
                    //PESign
                    if (exeBinRead.ReadUInt32() == 0x00004550)
                    {
                        exeFs.Position += 0x10; //Machine(2)+NumberOfSection(2)+12
                        //可选头大小
                        int optionalHeaderSize = exeBinRead.ReadUInt16();

                        //第一个节偏移 PEOffset+PESize(4)+FileHeaderSize(20)+OptionalHeaderSize
                        int firstSectionHeader = peOffset + 0x04 + 0x14 + optionalHeaderSize;

                        exeFs.Position = firstSectionHeader;
                        exeFs.Position += 8 + 4 + 4;  //secName(8)+VirtualSize(4)+VirtualAddress(4)

                        //获取代码段偏移与大小
                        int codeSize = exeBinRead.ReadInt32();
                        int codeOffset = exeBinRead.ReadInt32();

                        byte[] codeData = new byte[codeSize];

                        exeFs.Seek(codeOffset, SeekOrigin.Begin);
                        exeFs.Read(codeData, 0, codeSize);

                        int decryptFuncOffset;

                        //解密函数特征码
                        decryptFuncOffset = PECodeSearch(codeData, Signature.DecryptFunc, codeSize);   //KRKR 主程序

                        //wump3.dll
                        if (decryptFuncOffset == -1)
                        {
                            decryptFuncOffset = PECodeSearch(codeData, Signature.DecryptFuncWump3Plugin, codeSize);
                            if (decryptFuncOffset != -1)
                            {
                                //搜索key
                                uint key1, key2;

                                //mov dword ptr ss:[eax],imm32
                                exeFs.Seek(MemorySearch(codeData, new byte[] { 0xC7, 0x00 }, 0x40, decryptFuncOffset) + 2 + codeOffset, SeekOrigin.Begin);
                                key2 = exeBinRead.ReadUInt32();

                                //mov dword ptr ss:[ecx],imm32
                                exeFs.Seek(MemorySearch(codeData, new byte[] { 0xC7, 0x01 }, 0x40, decryptFuncOffset) + 2 + codeOffset, SeekOrigin.Begin);
                                key1 = exeBinRead.ReadUInt32();

                                //显示到文本框
                                ShowKey(key1, key2);

                                error = false;
                            }
                        }
                        else
                        {
                            //搜索key
                            uint key1, key2;

                            //mov dword ptr ss:[eax],imm32
                            exeFs.Seek(MemorySearch(codeData, new byte[] { 0xC7, 0x00 }, 0x40, decryptFuncOffset) + 2 + codeOffset, SeekOrigin.Begin);
                            key1 = exeBinRead.ReadUInt32();

                            //mov dword ptr ss:[ecx],imm32
                            exeFs.Seek(MemorySearch(codeData, new byte[] { 0xC7, 0x01 }, 0x40, decryptFuncOffset) + 2 + codeOffset, SeekOrigin.Begin);
                            key2 = exeBinRead.ReadUInt32();

                            //显示到文本框
                            ShowKey(key1, key2);

                            error = false;
                        }

                    }
                }
                exeBinRead.Dispose();
                exeFs.Dispose();

            }
            catch
            {
                MessageBox.Show("请选择正确的exe/dll文件", "错误", MessageBoxButtons.OK);
            }
            if (error == true)
            {
                MessageBox.Show("请选择正确的exe/dll文件", "错误", MessageBoxButtons.OK);
            }
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            if (this.listBoxFiles.Items.Count <= 0)
            {
                return;
            }


            Button btn = sender as Button;

            //检查key
            string strKey1 = this.tbKey1.Text.Trim();
            string strKey2 = this.tbKey2.Text.Trim();

            Regex regex = new Regex("^[0-9,a-f,A-F]{8}$");
            if ((regex.IsMatch(strKey1) && regex.IsMatch(strKey2)) == false)
            {
                MessageBox.Show("请输入正确的Key\n自动识别错误可自行使用OllyDbg或x64dbg寻找", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            uint key1 = Convert.ToUInt32(strKey1, 16);
            uint key2 = Convert.ToUInt32(strKey2, 16);

            List<string> fileList = new List<string>();
            for (int i = 0; i < this.listBoxFiles.Items.Count; i++)
            {
                fileList.Add(this.listBoxFiles.Items[i].ToString());
            }

            //解包
            fileList.ForEach(filePath =>
            {
                Thread thread = new Thread(() =>
                {
                    ArchiveFile archiveFile = new ArchiveFile();
                    archiveFile.Analysis(filePath);

                    archiveFile.Decrypt(key1, key2);
                    archiveFile.Extract();
                    Console.WriteLine(Path.GetFileName(filePath)+"  解包完毕");
                });
                thread.Start();
            });
        }

        /// <summary>
        /// 内存搜索
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="signature">搜索目标</param>
        /// <param name="searchSize">范围</param>
        /// <param name="offset">起始点</param>
        /// <returns></returns>
        public int MemorySearch(byte[] data, byte[] signature, int searchSize, int offset = 0)
        {
            int indexMax = Math.Min(searchSize - signature.Length, data.Length - signature.Length) + offset;

            for (; offset < indexMax; offset++)
            {
                if (data.Skip(offset).Take(signature.Length).SequenceEqual(signature))
                {
                    return offset;
                }
            }
            return -1;
        }

        /// <summary>
        /// 代码扫描
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="signature">扫描内容</param>
        /// <param name="codeSize">代码大小</param>
        /// <param name="offset">偏移</param>
        /// <returns></returns>
        public int PECodeSearch(byte[] data,byte[] signature,int codeSize,int offset = 0)
        {
            int decryptFuncOffset = -1;
            //内存扫描
            for (int codeindex = 0; codeindex < codeSize - signature.Length; codeindex++)
            {
                for (int signindex = 0; signindex < signature.Length; signindex++)
                {
                    if (signature[signindex] == 0)
                    {
                        continue;       //模糊搜索
                    }
                    if (data[codeindex + signindex] != signature[signindex])
                    {
                        break;  //不匹配
                    }
                    if (signindex == signature.Length - 1)
                    {
                        decryptFuncOffset = codeindex;      //匹配成功
                        break;
                    }
                }
                if (decryptFuncOffset != -1)
                {
                    break;      //搜索成功跳出循环
                }
            }
            return decryptFuncOffset;
        }

        private void ShowKey(uint key1, uint key2)
        {
            this.tbKey1.Text = key1.ToString("X8");
            this.tbKey2.Text = key2.ToString("X8");
        }
    }
}
