using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using NvlUnity;
using NvlUnity.V1;
using System.Threading.Tasks;
using System.Text;

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


        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (this.listBoxFilePath.Items.Count <= 0)
            {
                MessageBox.Show("请拖拽需要提取的文件到列表框", "Error");
                return;
            }

            if (this.cbGameTitle.SelectedIndex < 0)
            {
                MessageBox.Show("请选择游戏", "Error");
                return;
            }

            Button btn = sender as Button;
            btn.Enabled = false;

            this.tbLog.Clear();

            IEnumerable<string> filePaths = this.listBoxFilePath.Items.Cast<string>();
            string title = this.cbGameTitle.SelectedItem.ToString();
            string outDir = Path.Combine(Path.GetDirectoryName(listBoxFilePath.Items[0].ToString()), "Static_Extract");
            new Thread(() =>
            {
                ArchiveDecryptorBase decryptor = ArchiveDecryptorBase.Create(outDir, title);

                foreach (var path in filePaths)
                {
                    decryptor.Extract(path);
                }

                this.BeginInvoke(() =>
                {
                    btn.Enabled = true;
                    System.Diagnostics.Process.Start("explorer.exe", outDir);
                });
            }).Start();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.cbGameTitle.Items.Clear();
            foreach(var title in DataManager.Instance.GameTitles)
            {
                this.cbGameTitle.Items.Add(title);
            }

            Console.SetOut(new TextBoxLog(this.tbLog));
        }


        private class TextBoxLog : TextWriter
        {
            private TextBox mTextBox;

            public override void Write(string? value)
            {
                this.mTextBox.BeginInvoke((string msg) =>
                {
                    this.mTextBox.AppendText(msg);
                }, value);
            }

            public override void WriteLine(string? value)
            {
                this.Write(value);
                this.mTextBox.BeginInvoke(() =>
                {
                    this.mTextBox.AppendText("\n");
                });
            }

            public override Encoding Encoding => Encoding.Unicode;

            public TextBoxLog(TextBox tb)
            {
                this.mTextBox = tb;
            }
        }
    }
}
