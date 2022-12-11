using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExtractGUI
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }
        private void FileList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void FileList_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void FileList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                List<string> filepaths = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
                ListBox filelistbox = sender as ListBox;
                filelistbox.Items.Clear();
                filepaths.ForEach(filepath => 
                {
                    filelistbox.Items.Add(filepath);
                });
            }
        }

        private void cmdExtract_Click(object sender, EventArgs e)
        {
            if (this.cmbType.SelectedIndex < 0)
            {
                MessageBox.Show("请选择封包格式", "Error");
                return;
            }

            List<string> files = this.GetFilePath(this.listBoxFile);

            if (files.Count <= 0)
            {
                MessageBox.Show("请拖拽待处理的文件", "Error");
                return;
            }

            switch (this.cmbType.SelectedIndex)
            {
                case 0:

                    foreach(string file in files)
                    {
                        BKEngine.V20.BKARCFile bkarcV20 = new BKEngine.V20.BKARCFile(file);
                        if (bkarcV20.IsVaild == false)
                        {
                            bkarcV20?.Dispose();
                            continue;
                        }
                        if (bkarcV20.DecryptFile() == false)
                        {
                            bkarcV20?.Dispose();
                            continue;
                        }
                    }

                    break;
                case 1:
                    foreach (string file in files)
                    {
                        BKEngine.V21.BKARCFile bkarcV21 = new BKEngine.V21.BKARCFile(file);
                        if (bkarcV21.IsVaild == false)
                        {
                            bkarcV21?.Dispose();
                            continue;
                        }
                        if (bkarcV21.DecryptFile() == false)
                        {
                            bkarcV21?.Dispose();
                            continue;
                        }
                    }
                    break;
                case 2:
                    foreach (string file in files)
                    {
                        BKEngine.V40.BKARCFile bkarcV40 = new BKEngine.V40.BKARCFile(file);
                        bkarcV40.DecryptArchive(bkarcV40.AnalysisFile());
                        bkarcV40.OutputArchiveData();
                    }
                    break;
            }
            MessageBox.Show("解包完毕", "Information");
        }

        private List<string> GetFilePath(ListBox listBox)
        {
            List<string> files = new List<string>();
            for(int i = 0; i < listBox.Items.Count; i++)
            {
                files.Add(listBox.Items[i].ToString());
            }
            return files;
        }

    }

}
