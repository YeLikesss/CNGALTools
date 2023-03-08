
namespace HashDecoder
{
    partial class TextCreator
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbNameItems = new System.Windows.Forms.ListBox();
            this.tbPreview = new System.Windows.Forms.TextBox();
            this.gbCreator = new System.Windows.Forms.GroupBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.lbStatus = new System.Windows.Forms.Label();
            this.btnConfirmVar = new System.Windows.Forms.Button();
            this.rbtConstString = new System.Windows.Forms.RadioButton();
            this.cbConstExtension = new System.Windows.Forms.ComboBox();
            this.tbConstString = new System.Windows.Forms.TextBox();
            this.rbtExtension = new System.Windows.Forms.RadioButton();
            this.rbtNum = new System.Windows.Forms.RadioButton();
            this.rbtChar = new System.Windows.Forms.RadioButton();
            this.numCharCount = new System.Windows.Forms.NumericUpDown();
            this.numNumCount = new System.Windows.Forms.NumericUpDown();
            this.lbNameRightClickMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuCleanAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuModify = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInsertPrev = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInsertNext = new System.Windows.Forms.ToolStripMenuItem();
            this.gbCreator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCharCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumCount)).BeginInit();
            this.lbNameRightClickMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbNameItems
            // 
            this.lbNameItems.HorizontalScrollbar = true;
            this.lbNameItems.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.lbNameItems.ItemHeight = 21;
            this.lbNameItems.Location = new System.Drawing.Point(7, 70);
            this.lbNameItems.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lbNameItems.Name = "lbNameItems";
            this.lbNameItems.ScrollAlwaysVisible = true;
            this.lbNameItems.Size = new System.Drawing.Size(478, 319);
            this.lbNameItems.TabIndex = 5;
            this.lbNameItems.TabStop = false;
            this.lbNameItems.UseTabStops = false;
            this.lbNameItems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbNameItems_MouseUp);
            // 
            // tbPreview
            // 
            this.tbPreview.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.tbPreview.Location = new System.Drawing.Point(7, 29);
            this.tbPreview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbPreview.Name = "tbPreview";
            this.tbPreview.ReadOnly = true;
            this.tbPreview.Size = new System.Drawing.Size(675, 29);
            this.tbPreview.TabIndex = 4;
            // 
            // gbCreator
            // 
            this.gbCreator.Controls.Add(this.btnExtract);
            this.gbCreator.Controls.Add(this.lbStatus);
            this.gbCreator.Controls.Add(this.btnConfirmVar);
            this.gbCreator.Controls.Add(this.rbtConstString);
            this.gbCreator.Controls.Add(this.cbConstExtension);
            this.gbCreator.Controls.Add(this.tbConstString);
            this.gbCreator.Controls.Add(this.rbtExtension);
            this.gbCreator.Controls.Add(this.rbtNum);
            this.gbCreator.Controls.Add(this.rbtChar);
            this.gbCreator.Controls.Add(this.numCharCount);
            this.gbCreator.Controls.Add(this.numNumCount);
            this.gbCreator.Controls.Add(this.lbNameItems);
            this.gbCreator.Controls.Add(this.tbPreview);
            this.gbCreator.Location = new System.Drawing.Point(3, 3);
            this.gbCreator.Name = "gbCreator";
            this.gbCreator.Size = new System.Drawing.Size(697, 443);
            this.gbCreator.TabIndex = 6;
            this.gbCreator.TabStop = false;
            this.gbCreator.Text = "文本生成器";
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(502, 70);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(180, 42);
            this.btnExtract.TabIndex = 16;
            this.btnExtract.Text = "导出并解码Hash";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // lbStatus
            // 
            this.lbStatus.Location = new System.Drawing.Point(502, 164);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(180, 36);
            this.lbStatus.TabIndex = 15;
            this.lbStatus.Text = "状态";
            this.lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnConfirmVar
            // 
            this.btnConfirmVar.Location = new System.Drawing.Point(502, 203);
            this.btnConfirmVar.Name = "btnConfirmVar";
            this.btnConfirmVar.Size = new System.Drawing.Size(180, 41);
            this.btnConfirmVar.TabIndex = 14;
            this.btnConfirmVar.Text = "确认";
            this.btnConfirmVar.UseVisualStyleBackColor = true;
            this.btnConfirmVar.Click += new System.EventHandler(this.btnConfirmVar_Click);
            // 
            // rbtConstString
            // 
            this.rbtConstString.AutoSize = true;
            this.rbtConstString.Checked = true;
            this.rbtConstString.Location = new System.Drawing.Point(624, 402);
            this.rbtConstString.Name = "rbtConstString";
            this.rbtConstString.Size = new System.Drawing.Size(60, 25);
            this.rbtConstString.TabIndex = 9;
            this.rbtConstString.TabStop = true;
            this.rbtConstString.Text = "常量";
            this.rbtConstString.UseVisualStyleBackColor = true;
            // 
            // cbConstExtension
            // 
            this.cbConstExtension.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConstExtension.FormattingEnabled = true;
            this.cbConstExtension.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cbConstExtension.Items.AddRange(new object[] {
            "/",
            ".png",
            ".jpg",
            ".bmp",
            ".tlg",
            ".pimg",
            ".ogg",
            ".ogg.sli",
            ".pbd",
            ".sinfo",
            ".stand",
            ".txt",
            ".scn",
            ".txt.scn",
            ".func",
            ".csv",
            ".stand",
            ".ks",
            ".ks.scn",
            ".ttf",
            ".wmv",
            ".tjs",
            ".ini"});
            this.cbConstExtension.Location = new System.Drawing.Point(502, 350);
            this.cbConstExtension.Name = "cbConstExtension";
            this.cbConstExtension.Size = new System.Drawing.Size(114, 29);
            this.cbConstExtension.TabIndex = 13;
            this.cbConstExtension.Enter += new System.EventHandler(this.TypeInput_Enter);
            // 
            // tbConstString
            // 
            this.tbConstString.Location = new System.Drawing.Point(7, 402);
            this.tbConstString.Name = "tbConstString";
            this.tbConstString.Size = new System.Drawing.Size(609, 29);
            this.tbConstString.TabIndex = 6;
            this.tbConstString.Enter += new System.EventHandler(this.TypeInput_Enter);
            // 
            // rbtExtension
            // 
            this.rbtExtension.AutoSize = true;
            this.rbtExtension.Location = new System.Drawing.Point(624, 351);
            this.rbtExtension.Name = "rbtExtension";
            this.rbtExtension.Size = new System.Drawing.Size(60, 25);
            this.rbtExtension.TabIndex = 12;
            this.rbtExtension.Text = "后缀";
            this.rbtExtension.UseVisualStyleBackColor = true;
            // 
            // rbtNum
            // 
            this.rbtNum.AutoSize = true;
            this.rbtNum.Location = new System.Drawing.Point(624, 300);
            this.rbtNum.Name = "rbtNum";
            this.rbtNum.Size = new System.Drawing.Size(60, 25);
            this.rbtNum.TabIndex = 11;
            this.rbtNum.Text = "数字";
            this.rbtNum.UseVisualStyleBackColor = true;
            // 
            // rbtChar
            // 
            this.rbtChar.AutoSize = true;
            this.rbtChar.Location = new System.Drawing.Point(624, 250);
            this.rbtChar.Name = "rbtChar";
            this.rbtChar.Size = new System.Drawing.Size(60, 25);
            this.rbtChar.TabIndex = 10;
            this.rbtChar.Text = "字母";
            this.rbtChar.UseVisualStyleBackColor = true;
            // 
            // numCharCount
            // 
            this.numCharCount.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.numCharCount.Location = new System.Drawing.Point(502, 250);
            this.numCharCount.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numCharCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCharCount.Name = "numCharCount";
            this.numCharCount.Size = new System.Drawing.Size(114, 29);
            this.numCharCount.TabIndex = 7;
            this.numCharCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numCharCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCharCount.Enter += new System.EventHandler(this.TypeInput_Enter);
            // 
            // numNumCount
            // 
            this.numNumCount.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.numNumCount.Location = new System.Drawing.Point(502, 300);
            this.numNumCount.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numNumCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumCount.Name = "numNumCount";
            this.numNumCount.Size = new System.Drawing.Size(114, 29);
            this.numNumCount.TabIndex = 8;
            this.numNumCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numNumCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumCount.Enter += new System.EventHandler(this.TypeInput_Enter);
            // 
            // lbNameRightClickMenuStrip
            // 
            this.lbNameRightClickMenuStrip.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbNameRightClickMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCleanAll,
            this.menuDelete,
            this.menuModify,
            this.menuInsertPrev,
            this.menuInsertNext});
            this.lbNameRightClickMenuStrip.Name = "lbNameRightClickMenuStrip";
            this.lbNameRightClickMenuStrip.Size = new System.Drawing.Size(177, 134);
            // 
            // menuCleanAll
            // 
            this.menuCleanAll.Name = "menuCleanAll";
            this.menuCleanAll.Size = new System.Drawing.Size(176, 26);
            this.menuCleanAll.Text = "清除全部";
            this.menuCleanAll.Click += new System.EventHandler(this.listRightClickMenu_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.Size = new System.Drawing.Size(176, 26);
            this.menuDelete.Text = "删除";
            this.menuDelete.Click += new System.EventHandler(this.listRightClickMenu_Click);
            // 
            // menuModify
            // 
            this.menuModify.Name = "menuModify";
            this.menuModify.Size = new System.Drawing.Size(176, 26);
            this.menuModify.Text = "修改";
            this.menuModify.Click += new System.EventHandler(this.listRightClickMenu_Click);
            // 
            // menuInsertPrev
            // 
            this.menuInsertPrev.Name = "menuInsertPrev";
            this.menuInsertPrev.Size = new System.Drawing.Size(176, 26);
            this.menuInsertPrev.Text = "在上一行插入";
            this.menuInsertPrev.Click += new System.EventHandler(this.listRightClickMenu_Click);
            // 
            // menuInsertNext
            // 
            this.menuInsertNext.Name = "menuInsertNext";
            this.menuInsertNext.Size = new System.Drawing.Size(176, 26);
            this.menuInsertNext.Text = "在下一行插入";
            this.menuInsertNext.Click += new System.EventHandler(this.listRightClickMenu_Click);
            // 
            // TextCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbCreator);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "TextCreator";
            this.Size = new System.Drawing.Size(703, 453);
            this.gbCreator.ResumeLayout(false);
            this.gbCreator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCharCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumCount)).EndInit();
            this.lbNameRightClickMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbNameItems;
        private System.Windows.Forms.TextBox tbPreview;
        private System.Windows.Forms.GroupBox gbCreator;
        private System.Windows.Forms.RadioButton rbtExtension;
        private System.Windows.Forms.RadioButton rbtNum;
        private System.Windows.Forms.RadioButton rbtChar;
        private System.Windows.Forms.RadioButton rbtConstString;
        private System.Windows.Forms.NumericUpDown numNumCount;
        private System.Windows.Forms.NumericUpDown numCharCount;
        private System.Windows.Forms.TextBox tbConstString;
        private System.Windows.Forms.Button btnConfirmVar;
        private System.Windows.Forms.ComboBox cbConstExtension;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.ContextMenuStrip lbNameRightClickMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuCleanAll;
        private System.Windows.Forms.ToolStripMenuItem menuModify;
        private System.Windows.Forms.ToolStripMenuItem menuInsertPrev;
        private System.Windows.Forms.ToolStripMenuItem menuInsertNext;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.Button btnExtract;
    }
}
