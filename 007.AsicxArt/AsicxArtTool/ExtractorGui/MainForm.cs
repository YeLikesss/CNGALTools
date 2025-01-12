using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AsicxArt;
using AsicxArt.V1;

namespace Extractor.GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            ComboBox cb = this.cbTitle;

            cb.BeginUpdate();
            cb.Items.Clear();
            cb.Items.Add(new FluffyStore());
            cb.Items.Add(new VampiresMelody());
            cb.Items.Add(new VampiresMelody2());
            cb.SelectedIndex = 0;
            cb.EndUpdate();
        }

        private async void BtnExtract_OnClick(object sender, EventArgs e)
        {
            using FolderBrowserDialog fbd = new()
            {
                Description = "AsicxArt V1 请选择游戏资源文件夹",
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                UseDescriptionForTitle = true
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (this.cbTitle.SelectedItem is IGameInformationV1 gameInfo)
                {
                    Button btn = (Button)sender;
                    btn.Enabled = false;

                    await Task.Run(() =>
                    {
                        new ArchiveV1(gameInfo.SqliteAES128Key).Extract(fbd.SelectedPath);
                    });
                    MessageBox.Show("提取成功", "Information");
                    btn.Enabled = true;
                }
            }
        }
    }
}
