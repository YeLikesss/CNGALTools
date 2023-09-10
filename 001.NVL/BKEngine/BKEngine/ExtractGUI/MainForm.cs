using BKEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            ListBox lb = (ListBox)sender;

            lb.BeginUpdate();

            lb.Items.Clear();
            if (e.Data is IDataObject obj)
            {
                string[] resPaths = (string[])obj.GetData(DataFormats.FileDrop);
                foreach (string path in resPaths)
                {
                    lb.Items.Add(path);
                }
            }

            lb.EndUpdate();
        }

        private void cmdExtract_Click(object sender, EventArgs e)
        {
            if (this.cmbType.SelectedIndex < 0)
            {
                MessageBox.Show("请选择封包格式", "Error");
                return;
            }

            ListBox lb = this.listBoxFile;

            int count = lb.Items.Count;
            if (count <= 0)
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

            for (int i = 0; i < count; ++i)
            {
                if (lb.Items[i] is string filePath)
                {
                    if (File.Exists(filePath))
                    {
                        string outDir = Path.Combine(Path.GetDirectoryName(filePath), "Static_Extract");

                        using BKARCFileBase bkarc = BKARCFileBase.CreateInstance(Path.GetFileNameWithoutExtension(filePath), File.OpenRead(filePath), version);
                        bkarc.Extract(outDir);
                    }
                }
            }
            MessageBox.Show("解包完毕", "Information");
        }
    }
}
