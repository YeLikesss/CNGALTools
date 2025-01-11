using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace HashDecoder
{
    public partial class TextCreator : UserControl
    {

        /// <summary>
        /// 编辑模式
        /// </summary>
        protected enum EditorMode
        {
            /// <summary>
            /// 添加
            /// </summary>
            Add,
            /// <summary>
            /// 修改
            /// </summary>
            Modify,
            /// <summary>
            /// 在前一条插入
            /// </summary>
            InsertPrev,
            /// <summary>
            /// 在后一条插入
            /// </summary>
            InsertNext
        }

        /// <summary>
        /// 文本处理回调
        /// </summary>
        public Action<object> TextProcessCallBack { get; set; }

        /// <summary>
        /// 自动路径
        /// </summary>
        public List<string> AutoPath { get; set; }

        private List<TextStructure> mTexts = new();     //当前需要生成的文本类型
        private EditorMode mMode;           //编辑模式
        private int mSelectIndex;           //当前索引

        private Dictionary<EditorMode, string> mStatusString = new()        //状态表
        {
            { EditorMode.Add, "添加" },
            { EditorMode.Modify, "修改" },
            { EditorMode.InsertPrev, "选中上一行插入" },
            { EditorMode.InsertNext, "选中下一行插入" }
        };

        private Dictionary<ToolStripMenuItem, EditorMode> mModeMap = new(4);        //模式表
        private Dictionary<Control, RadioButton> mTypeBindMaps = new(4);     //类型UI映射


        /// <summary>
        /// 刷新UI
        /// </summary>
        private void UpdateUI()
        {
            //清空现有的结果
            this.tbPreview.Clear();
            this.lbNameItems.Items.Clear();

            //迭代显示
            for (int loop = 0; loop < this.mTexts.Count; loop++)
            {
                TextStructure t = this.mTexts[loop];
                //刷新预览框
                this.tbPreview.Text += t.ConstStr;
                //刷新预览表
                this.lbNameItems.Items.Add(string.Format("序号{0}    {1}", loop.ToString(), t.Preview));
            }
            this.mMode = EditorMode.Add;        //恢复至添加模式
            this.UpdateStatus();
        }

        /// <summary>
        /// 刷新状态
        /// </summary>
        private void UpdateStatus()
        {
            this.lbStatus.Text = this.mStatusString[this.mMode];
        }

        /// <summary>
        /// 提交文本数据
        /// </summary>
        private void DataCommit(TextStructure ts)
        {
            switch (this.mMode)
            {
                case EditorMode.Add:
                {
                    this.mTexts.Add(ts);
                    break;
                }
                case EditorMode.Modify:
                {
                    this.mTexts[this.mSelectIndex] = ts;
                    break;
                }
                case EditorMode.InsertPrev:
                {
                    this.mTexts.Insert(this.mSelectIndex, ts);
                    break;
                }
                case EditorMode.InsertNext:
                {
                    this.mTexts.Insert(this.mSelectIndex + 1, ts);
                    break;
                }
            }
            this.UpdateUI();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            //初始化模式映射
            this.mModeMap.Add(this.menuCleanAll, EditorMode.Add);
            this.mModeMap.Add(this.menuDelete, EditorMode.Add);
            this.mModeMap.Add(this.menuModify, EditorMode.Modify);
            this.mModeMap.Add(this.menuInsertPrev, EditorMode.InsertPrev);
            this.mModeMap.Add(this.menuInsertNext, EditorMode.InsertNext);

            //初始化输入映射
            this.mTypeBindMaps.Add(this.numCharCount, this.rbtChar);
            this.mTypeBindMaps.Add(this.numNumCount, this.rbtNum);
            this.mTypeBindMaps.Add(this.cbConstExtension, this.rbtExtension);
            this.mTypeBindMaps.Add(this.tbConstString, this.rbtConstString);

            this.mMode = EditorMode.Add;
            this.UpdateStatus();
        }

        /// <summary>
        /// 文本生成器
        /// </summary>
        /// <returns></returns>
        private IEnumerable<List<string>> TextGenerator()
        {
            List<string> sl = new(1024);

            //获得字符长度
            int bufferLen = StringGenerator.GetCharCount(this.mTexts);
            //用于存放生成的数据
            char[] buffer = new char[bufferLen];

            foreach (var res in StringGenerator.GetText(buffer, this.mTexts))
            {
                if (this.AutoPath is null)
                {
                    sl.Add(new(buffer));
                    //达到容量则返回
                    if (sl.Count == sl.Capacity)
                    {
                        yield return sl;
                        sl.Clear();
                    }
                }
                else
                {
                    foreach (var s in this.AutoPath)
                    {
                        sl.Add(string.Format("{0}{1}", s, new string(buffer)));
                        //达到容量则返回
                        if (sl.Count == sl.Capacity)
                        {
                            yield return sl;
                            sl.Clear();
                        }
                    }
                }
            }
            //剩余部分
            if (sl.Count != 0)
            {
                yield return sl;
            }
        }

        public TextCreator()
        {
            InitializeComponent();
            this.Initialize();
        }

        private void btnConfirmVar_Click(object sender, EventArgs e)
        {
            TextStructure ts = new();
            bool isVaild = false;

            if (this.rbtConstString.Checked)        //常量
            {
                //忽略空字符串
                if (!string.IsNullOrEmpty(this.tbConstString.Text))
                {
                    ts.TextType = TextType.ConstString;
                    ts.ConstStr = this.tbConstString.Text.ToLower();
                    ts.Preview = ts.ConstStr;
                    ts.Table = null;

                    isVaild = true;
                }
                else
                {
                    MessageBox.Show("请输入常量字符串", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (this.rbtChar.Checked)      //字母
            {
                ts.Count = Convert.ToInt32(this.numCharCount.Value);
                ts.TextType = TextType.Char;
                ts.Preview = string.Format("%字母{0}位%", ts.Count.ToString());
                ts.ConstStr = ts.Preview;
                ts.Table = "abcdefghijklmnopqrstuvwxyz";

                isVaild = true;
            }
            else if (this.rbtNum.Checked)       //数字
            {
                ts.Count = Convert.ToInt32(this.numNumCount.Value);
                ts.TextType = TextType.Number;
                ts.Preview = string.Format("%数字{0}位%", ts.Count.ToString());
                ts.ConstStr = ts.Preview;
                ts.Table = "0123456789";

                isVaild = true;
            }
            else if (this.rbtExtension.Checked)     //扩展名
            {
                if (this.cbConstExtension.SelectedIndex >= 0)
                {
                    string nowItem = this.cbConstExtension.SelectedItem.ToString();  //获取选中的名字
                    ts.TextType = TextType.ConstString;
                    ts.ConstStr = nowItem;
                    ts.Preview = ts.ConstStr;
                    ts.Table = null;

                    isVaild = true;
                }
                else
                {
                    MessageBox.Show("请选择后缀名", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (isVaild)
            {
                this.DataCommit(ts);
            }
        }

        private void lbNameItems_MouseUp(object sender, MouseEventArgs e)
        {
            ListBox lb = sender as ListBox;
            //右键
            if (e.Button == MouseButtons.Right)
            {
                int selectIndex = lb.IndexFromPoint(e.Location);
                if (selectIndex >= 0)
                {
                    this.menuDelete.Enabled = true;
                    this.menuInsertNext.Enabled = true;
                    this.menuInsertPrev.Enabled = true;
                    this.menuModify.Enabled = true;

                    lb.SetSelected(selectIndex, true);
                    this.mSelectIndex = selectIndex;        //保存当前索引
                }
                else
                {
                    this.menuDelete.Enabled = false;
                    this.menuInsertNext.Enabled = false;
                    this.menuInsertPrev.Enabled = false;
                    this.menuModify.Enabled = false;

                    this.mMode = EditorMode.Add;        //空白处点击还原输入状态
                    this.UpdateStatus();
                }
                this.lbNameRightClickMenuStrip.Show(lb, e.Location);
            }
        }

        //右键菜单点击  切换列表中选项模式
        private void listRightClickMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            this.mMode = this.mModeMap[menu];

            //清空与删除单独判断
            switch (menu.Name)
            {
                case "menuCleanAll":
                {
                    this.mTexts.Clear();
                    this.UpdateUI();
                    break;
                }
                case "menuDelete":
                {
                    this.mTexts.RemoveAt(this.mSelectIndex);
                    this.UpdateUI();
                    break;
                }
            }
            this.UpdateStatus();
        }

        //提取按钮
        private void btnExtract_Click(object sender, EventArgs e)
        {
            if (this.mTexts.Count > 0)
            {
                new Thread(new ParameterizedThreadStart(this.TextProcessCallBack)).Start(new Func<IEnumerable<List<string>>>(this.TextGenerator));
            }
        }

        //输入模式切换
        private void TypeInput_Enter(object sender, EventArgs e)
        {
            Control control = sender as Control;
            this.mTypeBindMaps[control].Checked = true;
        }


        /// <summary>
        /// 文本类型
        /// </summary>
        public enum TextType
        {
            /// <summary>
            /// 字符常量
            /// </summary>
            ConstString,
            /// <summary>
            /// 字母
            /// </summary>
            Char,
            /// <summary>
            /// 数字
            /// </summary>
            Number
        }

        /// <summary>
        /// 文本结构
        /// </summary>
        public class TextStructure
        {
            /// <summary>
            /// 类型
            /// </summary>
            public TextType TextType;
            /// <summary>
            /// 个数(仅字母与数字有效)
            /// </summary>
            public int Count;
            /// <summary>
            /// 预览
            /// </summary>
            public string Preview;
            /// <summary>
            /// 常量 (字母与数字为示例)
            /// </summary>
            public string ConstStr;

            /// <summary>
            /// 文字表
            /// </summary>
            public string Table;

            /// <summary>
            /// 缓存起始位置
            /// </summary>
            public int StartPos;

            /// <summary>
            /// 缓存终止位置
            /// </summary>
            public int EndPos;

            /// <summary>
            /// 可生成条目数量
            /// </summary>
            public long TextCount;
        }

        /// <summary>
        /// 字符串生成器
        /// </summary>
        public class StringGenerator
        {
            /// <summary>
            /// 获取字符长度
            /// </summary>
            /// <param name="texts">文本类型数组</param>
            /// <param name="dirMode">True为文件夹模式  False为文件名模式</param>
            /// <returns></returns>
            public static int GetCharCount(List<TextStructure> texts)
            {
                int pos = 0;        //当前位置

                //获取文本长度  并给文本定位
                Span<TextStructure> tsArray = CollectionsMarshal.AsSpan(texts);
                for (int i = 0; i < tsArray.Length; ++i)
                {
                    TextStructure ts = tsArray[i];
                    switch (ts.TextType)
                    {
                        case TextType.ConstString:
                        {
                            ts.StartPos = pos;
                            pos += ts.ConstStr.Length;
                            ts.EndPos = pos;

                            break;
                        }
                        case TextType.Char:
                        case TextType.Number:
                        {
                            ts.StartPos = pos;
                            pos += ts.Count;
                            ts.EndPos = pos;
                            ts.TextCount = StringGenerator.PowerN(ts.Table.Length, ts.Count);

                            break;
                        }
                    }
                    //回写
                    tsArray[i] = ts;
                }
                return pos;
            }

            /// <summary>
            /// 获取文本
            /// </summary>
            /// <param name="buffer">字符缓冲</param>
            /// <param name="texts">文本结构</param>
            /// <param name="slashIndex">斜杠位置(生成路径名用)</param>
            /// <returns></returns>
            public static IEnumerable<bool> GetText(char[] buffer, List<TextStructure> texts)
            {
                long textCount = 1;      //文本总数

                List<TextStructure> varArray = new();       //可变字符

                Span<char> textBuffer = buffer.AsSpan();

                //获取可变字符
                Span<TextStructure> tsArray = CollectionsMarshal.AsSpan(texts);
                for (int i = 0; i < tsArray.Length; ++i)
                {
                    TextStructure ts = tsArray[i];
                    switch (ts.TextType)
                    {
                        case TextType.ConstString:
                        {
                            break;
                        }
                        case TextType.Char:
                        case TextType.Number:
                        {
                            varArray.Add(ts);
                            textCount *= ts.TextCount;

                            break;
                        }
                    }
                }

                textBuffer.Fill('\0');

                //填充常量
                for (int i = 0; i < tsArray.Length; ++i)
                {
                    TextStructure ts = tsArray[i];
                    if (ts.TextType == TextType.ConstString)
                    {
                        ts.ConstStr.AsSpan().CopyTo(textBuffer[ts.StartPos..ts.EndPos]);
                    }
                }

                for (long i = 0; i < textCount; ++i)
                {
                    StringGenerator.FillCharByIndex(buffer, varArray, i);
                    yield return true;
                }
            }



            /// <summary>
            /// 使用索引填充缓存区
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="varArray">可变字符</param>
            /// <param name="index">索引</param>
            public static void FillCharByIndex(Span<char> buffer, List<TextStructure> varArray, long index)
            {
                //储存各段字符串的索引
                Dictionary<int, long> varCharIndexMap = new(varArray.Count);

                //找各项位置
                for (int order = varArray.Count - 1; order != -1; --order)
                {
                    long textCount = varArray[order].TextCount;        //获取文本项数

                    varCharIndexMap.Add(order, index % textCount);

                    index /= textCount;
                }

                //填充字符
                foreach (var charIndexMap in varCharIndexMap)
                {
                    TextStructure ts = varArray[charIndexMap.Key];
                    ReadOnlySpan<char> table = ts.Table.AsSpan();
                    int tableLength = table.Length;      //表长度
                    int charCount = ts.Count;       //字符长度
                    long charPos = charIndexMap.Value;

                    //填充
                    for (int offset = 0; offset < charCount; ++offset)
                    {
                        long charIndex = charPos % tableLength;
                        buffer[ts.StartPos + offset] = table[(int)charIndex];
                        charPos /= tableLength;
                    }
                }
            }


            /// <summary>
            /// 求幂次方
            /// </summary>
            /// <param name="baseN"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static long PowerN(int baseN, int n)
            {
                long res = 1;
                if (n != 0)
                {
                    for (int i = 0; i < n; i++) res *= baseN;
                }
                return res;
            }
        }

    }



}
