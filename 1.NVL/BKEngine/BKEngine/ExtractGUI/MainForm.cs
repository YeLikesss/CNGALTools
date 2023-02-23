using BKEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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

            BKEngineVersion version = this.cmbType.SelectedIndex switch
            {
                0 => BKEngineVersion.V20,
                1 => BKEngineVersion.V21,
                2 => BKEngineVersion.V40,
                _ => BKEngineVersion.Unknow,
            };

            string outDir = Path.Combine(Path.GetDirectoryName(files[0]), "Static_Extract");

            foreach (string filePath in files)
            {
                BKARCFileBase bkarc = BKARCFileBase.CreateInstance(Path.GetFileNameWithoutExtension(filePath), File.OpenRead(filePath), version);
                bkarc.Extract(outDir);
                bkarc.Dispose();
            }

            GC.Collect();
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
