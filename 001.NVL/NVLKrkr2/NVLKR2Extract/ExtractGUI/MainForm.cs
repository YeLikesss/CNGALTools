using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using NVLKR2Static;
using System.Diagnostics;

namespace ExtractorGUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.cbTitles.Items.Clear();
            foreach(var title in DataManager.GameMaps.Keys)
            {
                this.cbTitles.Items.Add(title);
            }

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

        private void btnExtract_Click(object sender, EventArgs e)
        {
            if (this.listBoxFiles.Items.Count <= 0)
            {
                MessageBox.Show("请拖拽你要解包的文件到列表框", "Error");
                return;
            }
            if (this.cbTitles.SelectedIndex < 0)
            {
                MessageBox.Show("请选择游戏", "Error");
                return;
            }

            Button btn = sender as Button;
            btn.Enabled = false;

            for (int i = 0; i < this.listBoxFiles.Items.Count; i++)
            {
                string path = this.listBoxFiles.Items[i].ToString();
                XP3Archive archive = XP3Archive.CreateInstance(path);
                archive?.Extract(Path.Combine(Path.GetDirectoryName(path), "Static_Extract"), DataManager.GameMaps[this.cbTitles.SelectedItem.ToString()]);
                archive?.Dispose();
            }
            MessageBox.Show("提取完毕", "Information");
            btn.Enabled = true;
        }
    }
}
