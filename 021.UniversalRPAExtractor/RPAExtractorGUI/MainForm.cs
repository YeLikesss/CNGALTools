using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RPAArchive;

namespace RPAExtractorGUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //拖拽标签获取拖拽数据
        private void DragFileTips_OnDragDrop(object sender, DragEventArgs e)
        {
            ListBox lb = this.lbFiles;
            lb.BeginUpdate();
            lb.Items.Clear();
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] resPaths)
            {
                lb.Items.AddRange(resPaths);
            }
            lb.EndUpdate();
        }

        //拖拽标签相应拖拽
        private void DragFileTips_OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) is bool v && v)
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        //解包按钮点击
        private async void BtnExtract_Click(object sender, EventArgs e)
        {
            this.tssVersion.Text = string.Empty;
            this.tssPackageName.Text = string.Empty;
            this.tssProgress.Text = string.Empty;
            this.tbLog.Clear();

            if (this.lbFiles.Items.Count == 0)
            {
                MessageBox.Show("文件列表为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            Button btn = (sender as Button)!;

            IEnumerable<string> files = this.lbFiles.Items.Cast<string>();

            int current = 0;
            int count = 0;
            IProgress<string> progressCB = new Progress<string>((string s) =>
            {
                this.tbLog.AppendText($"{s}\r\n");
                this.tssProgress.Text = $"{current} / {count}";
                ++current;
            });
            IProgress<string> logCB = new Progress<string>((string s) =>
            {
                this.tbLog.AppendText($"{s}\r\n");
            });
            IProgress<string> versionCB = new Progress<string>((string s) =>
            {
                this.tssVersion.Text = s;
            });
            IProgress<string> packageCB = new Progress<string>((string s) =>
            {
                this.tssPackageName.Text = s;
            });

            //开始解包
            btn.Enabled = false;
            foreach (string file in files)
            {
                await Task.Run(() =>
                {
                    RenpyRPA? rpa = RenpyRPAFactory.Create(file, out string error);
                    if (rpa is not null)
                    {
                        current = 0;
                        count = rpa.Count;

                        progressCB.Report($"开始提取: {rpa.FileName}");

                        versionCB.Report(rpa.Version.ToString());
                        packageCB.Report(rpa.FileName);

                        rpa.Extract(progressCB);
                    }
                    else
                    {
                        logCB.Report($"{error}: {file}");
                    }
                });
            }
            btn.Enabled = true;

            MessageBox.Show("处理完毕", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}