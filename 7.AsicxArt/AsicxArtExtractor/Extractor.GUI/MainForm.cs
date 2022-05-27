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
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "请选择游戏资源文件夹";
            folderDialog.ShowNewFolderButton = false;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string dirPath = folderDialog.SelectedPath;
                switch (this.cbSelectGame.SelectedIndex)
                {
                    case 0:
                        Archive.ExtractImage(dirPath, FluffyStore.Key);
                        Archive.ExtractL2DImage(dirPath, FluffyStore.Key);
                        break;
                    case 1:
                        Archive.ExtractImage(dirPath, VampiresMelody.Key);
                        Archive.ExtractL2DImage(dirPath, VampiresMelody.Key);
                        break;
                    default:
                        MessageBox.Show("请选择游戏", "错误");
                        return;
                }
                MessageBox.Show("提取成功", "Information");

            }
        }
    }
}
