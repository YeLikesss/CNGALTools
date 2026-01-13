using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using VNMakerCore.Crypto.V1.Games;
using VNMakerCore.General;

namespace VNMakerGUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.mLogProgress = new(this.LogEvent_OnReport);

            {
                ComboBox cbGames = this.cbGames;
                cbGames.BeginUpdate();

                ComboBox.ObjectCollection items = cbGames.Items;
                items.Clear();

                items.Add(new AiYvMingDeBiDuan());
                items.Add(new ArchenemyLunafall());
                items.Add(new XingKongQiShi());

                cbGames.EndUpdate();
            }
        }

        private readonly Progress<string> mLogProgress;

        //日志回调
        private void LogEvent_OnReport(string msg)
        {
            this.tbLog.AppendText(msg);
            this.tbLog.AppendText("\n");
        }

        private void ListBoxFiles_OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data is IDataObject obj && obj.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ListBoxFiles_OnDragDrop(object sender, DragEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            lb.BeginUpdate();
            lb.Items.Clear();

            if (e.Data is IDataObject obj && obj.GetData(DataFormats.FileDrop) is string[] paths)
            {
                foreach (string path in paths)
                {
                    lb.Items.Add(path);
                }
            }

            lb.EndUpdate();
        }

        //游戏选择切换
        private void ComboBoxGames_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            this.tbDescription.Clear();
            if (cb.SelectedItem is ICryptoFilter filter)
            {
                this.tbDescription.Text = filter.Description;
            }
        }

        //文件提取点击
        private async void BtnExtract_OnClick(object sender, EventArgs e)
        {
            if (this.cbGames.SelectedItem is ICryptoFilter filter)
            {
                ListBox.ObjectCollection items = this.lbFiles.Items;
                if (items.Count != 0)
                {
                    this.tbLog.Clear();

                    List<string> paths = new(4096);
                    for (int i = 0; i < items.Count; ++i)
                    {
                        if (items[i] is string s)
                        {
                            if (File.Exists(s))
                            {
                                paths.Add(s);
                            }
                            else if (Directory.Exists(s))
                            {
                                paths.AddRange(Directory.GetFiles(s, "*.*", SearchOption.AllDirectories));
                            }
                        }
                    }

                    Button btn = (Button)sender;
                    btn.Enabled = false;

                    await Task.Factory.StartNew(() =>
                    {
                        NWResource.ExtractFiles(paths, filter, this.mLogProgress);
                    });

                    btn.Enabled = true;

                    MessageBox.Show("提取成功", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("请拖拽文件到列表", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("请选择游戏", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}