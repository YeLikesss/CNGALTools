using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;

using NekoNyanStatic.Crypto;
using System.Threading.Tasks;

namespace ExtractorGUI
{
    public partial class MainForm : Form
    {
        private readonly TextBoxLog mLog;
        private Dictionary<string, CryptoVersion> mGameInfo;

        public MainForm()
        {
            InitializeComponent();

            //初始化日志
            {
                this.mLog = new(this.tbLog);
                Console.SetOut(this.mLog);
            }

            //初始化游戏信息
            {
                this.mGameInfo = DataManager.GameInformations;

                this.cbGameTitle.BeginUpdate();
                this.cbGameTitle.Items.Clear();
                foreach (string title in this.mGameInfo.Keys)
                {
                    this.cbGameTitle.Items.Add(title);
                }
                this.cbGameTitle.EndUpdate();
            }
        }


        private void FileDragEnter(object sender, DragEventArgs e)
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

        private void lbFilePath_DragDrop(object sender, DragEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            lb.Items.Clear();
            string[] resPaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string path in resPaths)
            {
                lb.Items.Add(path);
            }
        }

        private void cbGameTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelCryptoVer.Text = string.Empty;
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex >= 0)
            {
                if(cb.SelectedItem is string title)
                {
                    this.labelCryptoVer.Text = string.Format("{0}版加密", this.mGameInfo[title].ToString());
                }
            }
        }

        private async void btnExtract_Click(object sender, EventArgs e)
        {
            this.tbLog.Clear();
            if (this.lbFilePath.Items.Count <= 0)
            {
                MessageBox.Show("请拖拽待解封包到指定位置", "Error");
                return;
            }
            if (this.cbGameTitle.SelectedIndex < 0)
            {
                MessageBox.Show("请选择游戏", "Error");
                return;
            }

            Button btn = (Button)sender;
            if(this.cbGameTitle.SelectedItem is string title)
            {
                if (this.mGameInfo.TryGetValue(title, out CryptoVersion ver))
                {
                    btn.Enabled = false;
                    IEnumerable<string> fullPaths = this.lbFilePath.Items.Cast<string>();

                    Task proc = Task.Run(() =>
                    {
                        foreach (string pkgPath in fullPaths)
                        {
                            using ArchiveCryptoBase filter = ArchiveCryptoBase.Create(pkgPath, ver);
                            filter.Extract();
                        }
                    });

                    await proc;
                    this.mLog.Flush();
                    btn.Enabled = true;
                }
            }
        }

        private class TextBoxLog : TextWriter
        {
            private readonly StringBuilder mBuffer = new(8192);

            private readonly TextBox mTextBox;

            private void Print()
            {
                if (this.mBuffer.Length > this.mBuffer.Capacity - 1024)
                {
                    this.Flush();
                }
            }

            public override void Flush()
            {
                string str = this.mBuffer.ToString();
                this.mTextBox.BeginInvoke((string msg) =>
                {
                    this.mTextBox.AppendText(msg);
                }, str);
                this.mBuffer.Clear();
            }

            public override void Write(string? value)
            {
                this.mBuffer.Append(value);
                this.Print();
            }

            public override void WriteLine(string? value)
            {
                this.mBuffer.Append(value);
                this.mBuffer.Append("\r\n");
                this.Print();
            }

            public override Encoding Encoding => Encoding.Unicode;

            public TextBoxLog(TextBox tb)
            {
                this.mTextBox = tb;
            }
        }
    }
}