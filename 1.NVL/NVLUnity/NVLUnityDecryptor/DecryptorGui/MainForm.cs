using System;
using NvlUnity;
using NvlUnity.V1;
using System.Buffers;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NvlUnity.V1.Games;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;

namespace DecryptorGui
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void exePanel_DragEnter(object sender, DragEventArgs e)
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

        private void listBoxFilePath_DragEnter(object sender, DragEventArgs e)
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

        private void listBoxFilePath_DragDrop(object sender, DragEventArgs e)
        {
            ListBox lb = sender as ListBox;
            lb.Items.Clear();
            string[] resPaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach(string path in resPaths)
            {
                lb.Items.Add(path);
            }

        }


        private void exePanel_DragDrop(object sender, DragEventArgs e)
        {
            //获取exe路径
            string exePath = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            bool error = true;

            try
            {
                //读取exe文件
                using FileStream exeFs = new FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using BinaryReader exeBinRead = new BinaryReader(exeFs);

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

                        byte[] codeData = ArrayPool<byte>.Shared.Rent(codeSize);

                        exeFs.Seek(codeOffset, SeekOrigin.Begin);
                        exeFs.Read(codeData, 0, codeSize);

                        int decryptFuncOffset = -1;

                        //解密函数特征码
                        byte[] sign = Signature.DecryptFunc;

                        //内存扫描
                        for(int codeindex = 0; codeindex < codeSize - sign.Length; codeindex++)
                        {
                            for(int signindex = 0; signindex < sign.Length; signindex++)
                            {
                                if (sign[signindex] == 0)
                                {
                                    continue;       //模糊搜索
                                }
                                if (codeData[codeindex + signindex] != sign[signindex])
                                {
                                    break;  //不匹配
                                }
                                if (signindex == sign.Length - 1)
                                {
                                    decryptFuncOffset = codeindex;      //匹配成功
                                    break;
                                }
                            }
                            if (decryptFuncOffset != -1)
                            {
                                break;
                            }
                        }

                        //搜索key
                        uint key1, key2, key3;

                        //mov dword ptr ss:[ebp-C],imm32
                        exeFs.Seek(MemorySearch(codeData, new byte[] { 0xC7, 0x45, 0xF4 }, 0x60, decryptFuncOffset) + 3 + codeOffset, SeekOrigin.Begin);
                        key1 = exeBinRead.ReadUInt32();

                        //mov dword ptr ss:[ebp-8],imm32
                        exeFs.Seek(MemorySearch(codeData, new byte[] { 0xC7, 0x45, 0xF8 }, 0x60, decryptFuncOffset) + 3 + codeOffset, SeekOrigin.Begin);
                        key2 = exeBinRead.ReadUInt32();

                        //mov dword ptr ss:[ebp-4],imm32
                        exeFs.Seek(MemorySearch(codeData, new byte[] { 0xC7, 0x45, 0xFC }, 0x60, decryptFuncOffset) + 3 + codeOffset, SeekOrigin.Begin);
                        key3 = exeBinRead.ReadUInt32();

                        //显示到文本框
                        ShowKey(key1, key2, key3);

                        ArrayPool<byte>.Shared.Return(codeData);

                        error = false;
                    }
                }


            }
            catch
            {
                MessageBox.Show("请选择正确的exe文件", "错误", MessageBoxButtons.OK);
            }
            if (error == true)
            {
                MessageBox.Show("请选择正确的exe文件", "错误", MessageBoxButtons.OK);
            }

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


        private void ShowKey(uint key1,uint key2,uint key3)
        {
            this.tbKey1.Text = key1.ToString("X8");
            this.tbKey2.Text = key2.ToString("X8");
            this.tbKey3.Text = key3.ToString("X8");
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (this.listBoxFilePath.Items.Count <= 0)
            {
                return;
            }

            ArchiveHeader.UnityVersion ver = 0;
            switch (this.cbUnityVer.SelectedIndex)
            {
                case -1:
                    MessageBox.Show("请选择Unity版本", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;


                case 0:
                    ver = ArchiveHeader.UnityVersion.V2018_4_0_65448;
                break;
                case 1:
                    ver = ArchiveHeader.UnityVersion.V2018_4_26_44060;
                break;
            }
            Button btn = sender as Button;

            string strKey1 = this.tbKey1.Text.Trim();
            string strKey2 = this.tbKey2.Text.Trim();
            string strKey3 = this.tbKey3.Text.Trim();

            Regex regex = new Regex("^[0-9,a-f,A-F]{8}$");
            if ((regex.IsMatch(strKey1) && regex.IsMatch(strKey2) && regex.IsMatch(strKey3)) == false)
            {
                MessageBox.Show("请输入正确的Key\n自动识别错误可自行使用OllyDbg或x64dbg寻找", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            uint key1 = Convert.ToUInt32(strKey1, 16);
            uint key2 = Convert.ToUInt32(strKey2, 16);
            uint key3 = Convert.ToUInt32(strKey3, 16);

            this.progressBarDecrypt.Maximum = this.listBoxFilePath.Items.Count;
            this.progressBarDecrypt.Minimum = 0;
            this.progressBarDecrypt.Value = 0;
            this.progressBarDecrypt.Step = 1;

            List<string> fileList = new();
            for(int i = 0; i < this.listBoxFilePath.Items.Count; i++)
            {
                fileList.Add(this.listBoxFilePath.Items[i].ToString());
            }

            btn.Enabled = false;
            fileList.ForEach(filePath => 
            {
                Thread thread = new Thread(() =>
                {
                    ArchiveFile archiveFile = new();
                    archiveFile.Analysis(filePath);

                    //3组32位Key(位于游戏exe中)  版本号(对不上都试一下)
                    archiveFile.Extract(key1, key2, key3, ver);
                    this.BeginInvoke(new Action(()=> 
                    {
                        this.progressBarDecrypt.Value += this.progressBarDecrypt.Step;
                        if (this.progressBarDecrypt.Maximum == this.progressBarDecrypt.Value)
                        {
                            MessageBox.Show("解密完成!!!\n请使用AssetStudio进行解包", "Information", MessageBoxButtons.OK);
                            System.Diagnostics.Process.Start("explorer.exe", string.Concat(Path.GetDirectoryName(filePath), "\\Extract"));
                            btn.Enabled = true;
                        }
                    }));
                });
                thread.Start();
            });
        }
    }
}
