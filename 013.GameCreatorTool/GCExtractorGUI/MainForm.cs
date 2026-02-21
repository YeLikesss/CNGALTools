using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameCreatorStatic;
using GameCreatorStatic.Extractor.V1;

namespace GCExtractorGUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            //添加游戏
            {
                ComboBox cb = this.cbTitles;
                cb.BeginUpdate();
                cb.Items.Clear();

                cb.Items.Add(new ShiLing());
                cb.Items.Add(new DeadlyEndgame());
                cb.Items.Add(new LingHeHanJianWuYv());
                cb.Items.Add(new MomentOfMoonset());
                cb.Items.Add(new FellInLoveWithTheNobilityGirlAsAMemberOfTheRebelOrganization());
                cb.Items.Add(new MySuccubusKukula());
                cb.Items.Add(new DeadlyEndgameRemaster());
                cb.Items.Add(new BrokenGodAwakening());
                cb.Items.Add(new ShiinaTakisDecameron());
                cb.Items.Add(new YourCow());
                cb.Items.Add(new WaitingForYouAtTheEndOfTime());
                cb.Items.Add(new HappySistersLife());
                cb.Items.Add(new WindsPoem());
                cb.Items.Add(new FloainPlus());
                cb.Items.Add(new ReturnToCollegeAge());
                cb.Items.Add(new LovelyDeskmateLovelyLife());

                cb.EndUpdate();
            }
        }

        //游戏选择列表-选择事件
        private void CbTitles_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (sender as ComboBox)!;
            if (cb.SelectedIndex >= 0)
            {
                if (cb.SelectedItem is IGCExtractor extractor)
                {
                    TextBox tb = this.tbEngineDescription;
                    tb.Clear();

                    switch (extractor.ExtractorVersion)
                    {
                        case GCExtractorVersion.V1:
                        {
                            GCExtractorV1 v1 = (extractor as GCExtractorV1)!;

                            tb.Text = $"[版本]{v1.Version}  [加密]{v1.EntryptionFlag}";
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                }
            }
        }

        //选择文件夹按钮-点击事件
        private void BtnSelectDirectory_OnClick(object sender, EventArgs e)
        {
            using FolderBrowserDialog fbd = new()
            {
                Description = "请选择游戏文件夹",
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                UseDescriptionForTitle = true,
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.tbGameDirectory.Text = fbd.SelectedPath;
            }
        }

        //提取按钮-点击事件
        private async void BtnExtract_Click(object sender, EventArgs e)
        {
            ComboBox cbTitle = this.cbTitles;
            if (cbTitle.SelectedIndex >= 0)
            {
                string gameDirectory = this.tbGameDirectory.Text;
                if(!string.IsNullOrEmpty(gameDirectory))
                {
                    Button btn = (sender as Button)!;
                    TextBox tbLog = this.tbLog;
                    IGCExtractor extractor = (cbTitle.SelectedItem as IGCExtractor)!;

                    tbLog.Clear();
                    IProgress<string> messageCB = new Progress<string>((string s) =>
                    {
                        tbLog.AppendText($"{s}\r\n");
                    });

                    //提取资源
                    btn.Enabled = false;
                    await Task.Run(() =>
                    {
                        extractor.Extract(gameDirectory, messageCB);
                    });
                    btn.Enabled = true;

                    MessageBox.Show("资源提取成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("请选择游戏路径", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("请选择游戏", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}