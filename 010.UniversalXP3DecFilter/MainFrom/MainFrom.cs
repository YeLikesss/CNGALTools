using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XP3;
using XP3Archive;

namespace MainFrom
{
    public partial class MainFrom : Form
    {
        public MainFrom()
        {
            InitializeComponent();

            //添加游戏加密对象
            {
                ComboBox titles = this.cbTitles;
                titles.BeginUpdate();
                titles.Items.Clear();

                titles.Items.Add(new BiAnHuaZang());
                titles.Items.Add(new JadeMoon());
                titles.Items.Add(new ConspiracyFieldSnowTrapCh1());
                titles.Items.Add(new ConspiracyFieldSnowTrapCh2());
                titles.Items.Add(new ConspiracyFieldSnowTrapEx());
                titles.Items.Add(new ConspiracyFieldFogShadow());
                titles.Items.Add(new TheRainyPortKeelung());
                titles.Items.Add(new YveZhuoEP1());
                titles.Items.Add(new YveZhuoOrange());
                titles.Items.Add(new LeaveSLeaveIfLeavesToDust_Demo());
                titles.Items.Add(new Rain());
                titles.Items.Add(new Ring());
                titles.Items.Add(new SummerInWaterDroplets());
                titles.Items.Add(new WanRuoZhaoYang());

                titles.EndUpdate();
            }
        }

        private void File_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data is IDataObject obj)
            {
                if (obj.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.All;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void listBoxFiles_DragDrop(object sender, DragEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            lb.BeginUpdate();

            lb.Items.Clear();
            if (e.Data is IDataObject obj)
            {
                string[] resPaths = (string[])obj.GetData(DataFormats.FileDrop);
                foreach (string path in resPaths)
                {
                    lb.Items.Add(path);
                }
            }

            lb.EndUpdate();
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            if(this.cbTitles.SelectedItem is IXP3Filter filter)
            {
                int pkgCount = this.listBoxFiles.Items.Count;
                if (pkgCount > 0)
                {
                    Button btn = (Button)sender;
                    btn.Enabled = false;

                    for (int i = 0; i < pkgCount; i++)
                    {
                        if(this.listBoxFiles.Items[i] is string path)
                        {
                            Archive arc = new(path, filter);
                            arc.Extract();
                        }
                    }

                    MessageBox.Show("提取完毕", "Information");
                    btn.Enabled = true;
                }
                else
                {
                    MessageBox.Show("请拖拽你要解包的文件到列表框", "Error");
                }
            }
            else
            {
                MessageBox.Show("请选择游戏", "Error");
            }
        }
    }
}