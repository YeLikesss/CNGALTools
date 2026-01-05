using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EngineCore;

namespace ExtractorGUI
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public enum OperatorType
        {
            /// <summary>
            /// 提取封包
            /// </summary>
            ExtractPackage,
            /// <summary>
            /// 提取脚本
            /// </summary>
            ExtractScenario,
        }

        public MainForm()
        {
            InitializeComponent();

            {
                ComboBox cb = this.cbTitles;
                cb.BeginUpdate();
                cb.Items.AddRange(YuriGameInformation.Titles.ToArray());
                cb.EndUpdate();
            }
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

        //下拉框选择
        private void CbTitles_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.tbKey.Clear();
            this.tbIV.Clear();
            if (sender is ComboBox { SelectedItem: YuriGameInformation gameInfo })
            {
                this.tbKey.Text = gameInfo.StringKey;
                this.tbIV.Text = string.Join(' ', gameInfo.IV.ToList().ConvertAll(b => b.ToString("X2")));
            }
        }

        //解包按钮点击
        private async void BtnExtract_Click(object sender, EventArgs e)
        {
            this.tbLog.Clear();
            if (this.lbFiles.Items.Count == 0)
            {
                MessageBox.Show("文件列表为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            if (this.cbTitles.SelectedItem is not YuriGameInformation gameInfo)
            {
                MessageBox.Show("请选择游戏", "错误", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            IEnumerable<string> files = this.lbFiles.Items.Cast<string>();

            IProgress<string> logCB = new Progress<string>((string s) =>
            {
                this.tbLog.AppendText($"{s}\r\n");
            });

            //开始解包
            Button btn = (sender as Button)!;
            btn.Enabled = false;
            foreach (string file in files)
            {
                await Task.Run(() =>
                {
                    if (Enum.TryParse(btn.Tag as string, false, out OperatorType type))
                    {
                        switch (type)
                        {
                            case OperatorType.ExtractPackage:
                            {
                                YuriPackage? pkg = YuriPackage.Open(file, gameInfo, out string error);
                                if (pkg is not null)
                                {
                                    pkg.Extract(Path.GetDirectoryName(file)!, logCB);
                                }
                                else
                                {
                                    logCB.Report($"{error}: {file}");
                                }
                                break;
                            }
                            case OperatorType.ExtractScenario:
                            {
                                YuriScenario? scr = YuriScenario.Open(file, gameInfo, out string error);
                                if (scr is not null)
                                {
                                    scr.Extract(Path.GetDirectoryName(file)!, logCB);
                                }
                                else
                                {
                                    logCB.Report($"{error}: {file}");
                                }
                                break;
                            }
                        }
                    }
                });
            }
            btn.Enabled = true;

            MessageBox.Show("处理完毕", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}