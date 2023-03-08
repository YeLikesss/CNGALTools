using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Utils.PathProcess;
using System.Threading;
using NVLKR2Static;
using System.Text.RegularExpressions;

namespace HashDecoder
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 目标路径
        /// </summary>
        private string mDecoderPath = string.Empty;

        public MainForm()
        {
            InitializeComponent();

            this.tCreator.TextProcessCallBack = this.HasherProcess;
        }

        private Regex mRegexHashNameFilter = new("^[0-9,A-F]{8}$");

        /// <summary>
        /// hash解码
        /// </summary>
        /// <param name="filePaths">文件名列表</param>
        /// <param name="strings">字符串列表</param>
        private void HasherDecode(List<string> filePaths,List<string> strings)
        {
            Dictionary<string, string> maps = XP3Archive.CalculateHashMulit(strings);

            Regex filter = this.mRegexHashNameFilter;

            for (int i = 0; i < filePaths.Count; ++i)
            {
                string op = filePaths[i];

                string hashName = Path.GetFileNameWithoutExtension(op);
                string hashExt = Path.GetExtension(op)[1..];                //去掉'.'字符

                if (filter.IsMatch(hashName) && filter.IsMatch(hashExt))
                {
                    if (maps.TryGetValue(hashName, out string name))
                    { 
                        if(XP3Archive.StringHash(Path.GetExtension(name)) == uint.Parse(hashExt, System.Globalization.NumberStyles.HexNumber))
                        {
                            string np = Path.Combine(Path.GetDirectoryName(op), name);
                            {
                                string dir = Path.GetDirectoryName(np);
                                if (!Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                            }
                            File.Move(op, np);
                            filePaths[i] = np;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Hash处理进程
        /// </summary>
        private void HasherProcess(object callback)
        {
            if (this.Invoke(this.CheckVaild))
            {
                this.Invoke(this.SetProcessUIEnable, false);         //锁定UI

                List<string> filePaths = PathUtil.EnumerateFullName(this.mDecoderPath);

                //文本处理
                if (callback is Func<IEnumerable<List<string>>> textProc)
                {
                    foreach (List<string> strings in textProc())
                    {
                        this.HasherDecode(filePaths, strings);
                    }
                }
                else if(callback is List<string> strings)
                {
                    this.HasherDecode(filePaths, strings);
                }
                this.BeginInvoke(this.SetProcessUIEnable, true);         //解锁UI
            }
        }

        /// <summary>
        /// 检查环境合法性
        /// </summary>
        private bool CheckVaild()
        {
            if (string.IsNullOrEmpty(this.mDecoderPath))
            {
                MessageBox.Show("目标路径未设置", "Error");
                return false;
            }

            if(this.tCreator.AutoPath is null)
            {
                MessageBox.Show("自动路径未设置", "Error");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 设置处理UI可用性
        /// </summary>
        /// <param name="enable"></param>
        private void SetProcessUIEnable(bool enable)
        {
            this.tCreator.Enabled = enable;
            this.btnEnumPath.Enabled = enable;
            this.btnLoadDumpFile.Enabled = enable;
            this.btnLoadAutoPath.Enabled = enable;
            this.btnSelectArchiveDirectory.Enabled = enable;
            this.btnEnumPathWithAutoPath.Enabled = enable;
        }

        private void btnSelectArchiveDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new()
            {
                Description = "请选择文件夹",
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                UseDescriptionForTitle = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.mDecoderPath = dialog.SelectedPath;
            }
        }

        private void btnEnumPath_Click(object sender, EventArgs e)
        {
            string path = string.Empty;
            FolderBrowserDialog dialog = new()
            {
                Description = "请选择文件夹",
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                UseDescriptionForTitle = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.SelectedPath;
            }
            if (!string.IsNullOrEmpty(path))
            {
                //开一个线程还原
                new Thread(new ParameterizedThreadStart(this.HasherProcess)).Start(PathUtil.EnumerateKirikiriRelativeName(path));
            }
        }

        private void btnLoadAutoPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new()
            {
                Multiselect = false,
                Title = "请选择文件",
                Filter = "文本文档(*.lst)|*.lst",
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                CheckPathExists = true,
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = fileDialog.FileName;
                List<string> autoPath = new(128);

                using StreamReader reader = new(path, Encoding.UTF8);
                while (!reader.EndOfStream)
                {
                    string s = reader.ReadLine();

                    if (!autoPath.Contains(s))
                    {
                        autoPath.Add(s);
                    }
                }
                reader.Close();

                //刷新autopath
                this.tCreator.AutoPath = autoPath;
            }
        }

        private void btnLoadDumpFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new()
            {
                Multiselect = false,
                Title = "请选择文件",
                Filter = "文本文档(*.lst)|*.lst",
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                CheckPathExists = true,
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = fileDialog.FileName;
                List<string> paths = new(512);

                using StreamReader reader = new(path, Encoding.UTF8);
                while (!reader.EndOfStream)
                {
                    string s = reader.ReadLine();

                    if (!paths.Contains(s))
                    {
                        paths.Add(s);
                    }
                }
                reader.Close();

                //开一个线程还原
                new Thread(new ParameterizedThreadStart(this.HasherProcess)).Start(paths);
            }
        }

        private void btnEnumPathWithAutoPath_Click(object sender, EventArgs e)
        {
            if (this.CheckVaild())
            {

                string path = string.Empty;
                FolderBrowserDialog dialog = new()
                {
                    Description = "请选择文件夹",
                    ShowNewFolderButton = false,
                    AutoUpgradeEnabled = true,
                    UseDescriptionForTitle = true
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    path = dialog.SelectedPath;
                }
                if (!string.IsNullOrEmpty(path))
                { 

                    List<string> ps = PathUtil.EnumerateKirikiriRelativeName(path);

                    List<string> strings = new(ps.Count * this.tCreator.AutoPath.Count);

                    foreach (var ap in this.tCreator.AutoPath)
                    {
                        foreach (var p in ps)
                        {
                            strings.Add(ap + p);
                        }
                    }
                    //开一个线程还原
                    new Thread(new ParameterizedThreadStart(this.HasherProcess)).Start(strings);
                }
            }
        }
    }
}