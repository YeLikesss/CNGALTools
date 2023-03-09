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
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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

        private readonly Regex mRegexHashNameFilter = new("^[0-9,A-F]{8}$");

        private readonly ParallelOptions mMultiThreadOption = new() { MaxDegreeOfParallelism = 8 };     //8线程


        /// <summary>
        /// hash解码
        /// </summary>
        /// <param name="filePaths">文件名列表</param>
        /// <param name="strings">字符串列表</param>
        private void HasherDecode(List<string> filePaths,List<string> strings)
        {
            Dictionary<string, string> maps = XP3Archive.CalculateHashMulit(strings);

            Regex filter = this.mRegexHashNameFilter;
            ParallelOptions options = this.mMultiThreadOption;

            Parallel.For(0, filePaths.Count, i =>
            {
                string op = filePaths[i];

                string hashName = Path.GetFileNameWithoutExtension(op);
                string hashExt = Path.GetExtension(op)[1..];                //去掉'.'字符

                if (filter.IsMatch(hashName) && filter.IsMatch(hashExt))
                {
                    if (maps.TryGetValue(hashName, out string name))
                    {
                        if (XP3Archive.StringHash(Path.GetExtension(name)) == uint.Parse(hashExt, System.Globalization.NumberStyles.HexNumber))
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
            });

            //for (int i = 0; i < filePaths.Count; ++i)
            //{

            //}
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
            }
            this.BeginInvoke(this.SetProcessUIEnable, true);         //解锁UI
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

        /// <summary>
        /// 选择文件夹路径
        /// </summary>
        /// <returns></returns>
        private static string SelectDirectory()
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
                return dialog.SelectedPath;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 选择dump文件
        /// </summary>
        /// <returns></returns>
        private static string SelectDumpFile()
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
                return fileDialog.FileName;
            }
            else
            {
                return null;
            }
        }


        private void btnSelectArchiveDirectory_Click(object sender, EventArgs e)
        {
            string directory = SelectDirectory();
            if (!string.IsNullOrEmpty(directory))
            {
                this.mDecoderPath = directory;
            }
        }

        private void btnEnumPath_Click(object sender, EventArgs e)
        {
            string directory = SelectDirectory();
            if (!string.IsNullOrEmpty(directory))
            {
                //开一个线程还原
                new Thread(new ParameterizedThreadStart(this.HasherProcess)).Start(PathUtil.EnumerateKirikiriRelativeName(directory));
            }
        }

        private void btnLoadAutoPath_Click(object sender, EventArgs e)
        {
            string autopathFile = SelectDumpFile();
            if (!string.IsNullOrEmpty(autopathFile))
            {
                List<string> autoPath = new(128);

                using StreamReader reader = new(autopathFile, Encoding.UTF8);
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
            string dumpfilePath = SelectDumpFile();
            if (!string.IsNullOrEmpty(dumpfilePath))
            {
                this.SetProcessUIEnable(false);
                new Thread(new ThreadStart(() =>
                {
                    List<string> paths = new(512);

                    using StreamReader reader = new(dumpfilePath, Encoding.UTF8);
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
                })).Start();
            }
        }

        private void btnEnumPathWithAutoPath_Click(object sender, EventArgs e)
        {
            if (this.CheckVaild())
            {
                string directory = SelectDirectory();
                if (!string.IsNullOrEmpty(directory))
                {
                    this.SetProcessUIEnable(false);
                    new Thread(new ThreadStart(() =>
                    {
                        List<string> ps = PathUtil.EnumerateKirikiriRelativeName(directory);
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
                    })).Start();
                }
            }
        }
    }
}