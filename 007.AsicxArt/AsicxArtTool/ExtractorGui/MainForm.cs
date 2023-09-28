using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AsicxArt;
using AsicxArt.V1;
using System.IO;

namespace Extractor.GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            {
                ComboBox cb = this.cbTitle;

                cb.BeginUpdate();
                cb.Items.Clear();
                cb.Items.Add(new FluffyStore());
                cb.Items.Add(new VampiresMelody());
                cb.Items.Add(new VampiresMelody2());
                cb.SelectedIndex = 0;
                cb.EndUpdate();
            }

        }

        private async void btnExtract_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog fbd = new();
            fbd.Description = "请选择游戏资源文件夹";
            fbd.ShowNewFolderButton = false;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (this.cbTitle.SelectedItem is IGameInformation gameInfo)
                {
                    Button btn = (Button)sender;
                    btn.Enabled = false;

                    await Task.Run(() =>
                    {
                        new Archive(gameInfo.SqliteAES128Key).Extract(fbd.SelectedPath);
                    });
                    MessageBox.Show("提取成功", "Information");
                    btn.Enabled = true;
                }
            }
        }
    }
}
