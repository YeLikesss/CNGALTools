using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using NekoNyanStatic.Crypto;

namespace ExtractorGUI
{
    public partial class MainForm : Form
    {
        private readonly Progress<string> mLogger;
        private readonly Dictionary<string, CryptoVersion> mGameInfo;

        public MainForm()
        {
            InitializeComponent();

            this.mLogger = new Progress<string>((string s) =>
            {
                this.tbLog.AppendText(s);
                this.tbLog.AppendText("\r\n");
            });

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
            if (e.Data is IDataObject obj && obj.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void LbFilePath_OnDragDrop(object sender, DragEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            lb.BeginUpdate();
            lb.Items.Clear();
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] resPaths)
            {
                lb.Items.AddRange(resPaths);
            }
            lb.EndUpdate();
        }

        private void CbGameTitle_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelCryptoVer.Text = string.Empty;
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex >= 0)
            {
                if (cb.SelectedItem is string title)
                {
                    this.labelCryptoVer.Text = string.Format("{0}版加密", this.mGameInfo[title].ToString());
                }
            }
        }

        private async void BtnExtract_OnClick(object sender, EventArgs e)
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
            if (this.cbGameTitle.SelectedItem is string title)
            {
                if (this.mGameInfo.TryGetValue(title, out CryptoVersion ver))
                {
                    btn.Enabled = false;
                    IEnumerable<string> fullPaths = this.lbFilePath.Items.Cast<string>();

                    Task proc = Task.Run(() =>
                    {
                        foreach (string pkgPath in fullPaths)
                        {
                            using ArchiveCryptoBase? filter = ArchiveCryptoBase.Create(pkgPath, ver);
                            filter?.Extract(this.mLogger);
                        }
                    });

                    await proc;
                    btn.Enabled = true;
                }
            }
        }
    }
}