
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
            components = new System.ComponentModel.Container();
            lbNameItems = new System.Windows.Forms.ListBox();
            tbPreview = new System.Windows.Forms.TextBox();
            gbCreator = new System.Windows.Forms.GroupBox();
            btnExtract = new System.Windows.Forms.Button();
            lbStatus = new System.Windows.Forms.Label();
            btnConfirmVar = new System.Windows.Forms.Button();
            rbtConstString = new System.Windows.Forms.RadioButton();
            cbConstExtension = new System.Windows.Forms.ComboBox();
            tbConstString = new System.Windows.Forms.TextBox();
            rbtExtension = new System.Windows.Forms.RadioButton();
            rbtNum = new System.Windows.Forms.RadioButton();
            rbtChar = new System.Windows.Forms.RadioButton();
            numCharCount = new System.Windows.Forms.NumericUpDown();
            numNumCount = new System.Windows.Forms.NumericUpDown();
            lbNameRightClickMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            menuCleanAll = new System.Windows.Forms.ToolStripMenuItem();
            menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            menuModify = new System.Windows.Forms.ToolStripMenuItem();
            menuInsertPrev = new System.Windows.Forms.ToolStripMenuItem();
            menuInsertNext = new System.Windows.Forms.ToolStripMenuItem();
            gbCreator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCharCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numNumCount).BeginInit();
            lbNameRightClickMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // lbNameItems
            // 
            lbNameItems.HorizontalScrollbar = true;
            lbNameItems.ImeMode = System.Windows.Forms.ImeMode.Disable;
            lbNameItems.ItemHeight = 21;
            lbNameItems.Location = new System.Drawing.Point(7, 70);
            lbNameItems.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lbNameItems.Name = "lbNameItems";
            lbNameItems.ScrollAlwaysVisible = true;
            lbNameItems.Size = new System.Drawing.Size(478, 319);
            lbNameItems.TabIndex = 5;
            lbNameItems.TabStop = false;
            lbNameItems.UseTabStops = false;
            lbNameItems.MouseUp += lbNameItems_MouseUp;
            // 
            // tbPreview
            // 
            tbPreview.ImeMode = System.Windows.Forms.ImeMode.Disable;
            tbPreview.Location = new System.Drawing.Point(7, 29);
            tbPreview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tbPreview.Name = "tbPreview";
            tbPreview.ReadOnly = true;
            tbPreview.Size = new System.Drawing.Size(675, 29);
            tbPreview.TabIndex = 4;
            // 
            // gbCreator
            // 
            gbCreator.Controls.Add(btnExtract);
            gbCreator.Controls.Add(lbStatus);
            gbCreator.Controls.Add(btnConfirmVar);
            gbCreator.Controls.Add(rbtConstString);
            gbCreator.Controls.Add(cbConstExtension);
            gbCreator.Controls.Add(tbConstString);
            gbCreator.Controls.Add(rbtExtension);
            gbCreator.Controls.Add(rbtNum);
            gbCreator.Controls.Add(rbtChar);
            gbCreator.Controls.Add(numCharCount);
            gbCreator.Controls.Add(numNumCount);
            gbCreator.Controls.Add(lbNameItems);
            gbCreator.Controls.Add(tbPreview);
            gbCreator.Location = new System.Drawing.Point(3, 3);
            gbCreator.Name = "gbCreator";
            gbCreator.Size = new System.Drawing.Size(697, 443);
            gbCreator.TabIndex = 6;
            gbCreator.TabStop = false;
            gbCreator.Text = "文本生成器";
            // 
            // btnExtract
            // 
            btnExtract.Location = new System.Drawing.Point(502, 70);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new System.Drawing.Size(180, 42);
            btnExtract.TabIndex = 16;
            btnExtract.Text = "导出并解码Hash";
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += btnExtract_Click;
            // 
            // lbStatus
            // 
            lbStatus.Location = new System.Drawing.Point(502, 164);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new System.Drawing.Size(180, 36);
            lbStatus.TabIndex = 15;
            lbStatus.Text = "状态";
            lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnConfirmVar
            // 
            btnConfirmVar.Location = new System.Drawing.Point(502, 203);
            btnConfirmVar.Name = "btnConfirmVar";
            btnConfirmVar.Size = new System.Drawing.Size(180, 41);
            btnConfirmVar.TabIndex = 14;
            btnConfirmVar.Text = "确认";
            btnConfirmVar.UseVisualStyleBackColor = true;
            btnConfirmVar.Click += btnConfirmVar_Click;
            // 
            // rbtConstString
            // 
            rbtConstString.AutoSize = true;
            rbtConstString.Checked = true;
            rbtConstString.Location = new System.Drawing.Point(624, 402);
            rbtConstString.Name = "rbtConstString";
            rbtConstString.Size = new System.Drawing.Size(60, 25);
            rbtConstString.TabIndex = 9;
            rbtConstString.TabStop = true;
            rbtConstString.Text = "常量";
            rbtConstString.UseVisualStyleBackColor = true;
            // 
            // cbConstExtension
            // 
            cbConstExtension.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbConstExtension.FormattingEnabled = true;
            cbConstExtension.ImeMode = System.Windows.Forms.ImeMode.Disable;
            cbConstExtension.Items.AddRange(new object[] { "/", ".png", ".jpg", ".bmp", ".tlg", ".pimg", ".ogg", ".ogg.sli", ".pbd", ".sinfo", ".stand", ".txt", ".scn", ".txt.scn", ".func", ".csv", ".stand", ".ks", ".ks.scn", ".ttf", ".wmv", ".tjs", ".ini", ".wav", ".mp3", ".mp4", ".asd", ".opus", ".json", ".avi", ".jpeg" });
            cbConstExtension.Location = new System.Drawing.Point(502, 350);
            cbConstExtension.Name = "cbConstExtension";
            cbConstExtension.Size = new System.Drawing.Size(114, 29);
            cbConstExtension.TabIndex = 13;
            cbConstExtension.Enter += TypeInput_Enter;
            // 
            // tbConstString
            // 
            tbConstString.Location = new System.Drawing.Point(7, 402);
            tbConstString.Name = "tbConstString";
            tbConstString.Size = new System.Drawing.Size(609, 29);
            tbConstString.TabIndex = 6;
            tbConstString.Enter += TypeInput_Enter;
            // 
            // rbtExtension
            // 
            rbtExtension.AutoSize = true;
            rbtExtension.Location = new System.Drawing.Point(624, 351);
            rbtExtension.Name = "rbtExtension";
            rbtExtension.Size = new System.Drawing.Size(60, 25);
            rbtExtension.TabIndex = 12;
            rbtExtension.Text = "后缀";
            rbtExtension.UseVisualStyleBackColor = true;
            // 
            // rbtNum
            // 
            rbtNum.AutoSize = true;
            rbtNum.Location = new System.Drawing.Point(624, 300);
            rbtNum.Name = "rbtNum";
            rbtNum.Size = new System.Drawing.Size(60, 25);
            rbtNum.TabIndex = 11;
            rbtNum.Text = "数字";
            rbtNum.UseVisualStyleBackColor = true;
            // 
            // rbtChar
            // 
            rbtChar.AutoSize = true;
            rbtChar.Location = new System.Drawing.Point(624, 250);
            rbtChar.Name = "rbtChar";
            rbtChar.Size = new System.Drawing.Size(60, 25);
            rbtChar.TabIndex = 10;
            rbtChar.Text = "字母";
            rbtChar.UseVisualStyleBackColor = true;
            // 
            // numCharCount
            // 
            numCharCount.ImeMode = System.Windows.Forms.ImeMode.Disable;
            numCharCount.Location = new System.Drawing.Point(502, 250);
            numCharCount.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            numCharCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCharCount.Name = "numCharCount";
            numCharCount.Size = new System.Drawing.Size(114, 29);
            numCharCount.TabIndex = 7;
            numCharCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            numCharCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numCharCount.Enter += TypeInput_Enter;
            // 
            // numNumCount
            // 
            numNumCount.ImeMode = System.Windows.Forms.ImeMode.Disable;
            numNumCount.Location = new System.Drawing.Point(502, 300);
            numNumCount.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            numNumCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numNumCount.Name = "numNumCount";
            numNumCount.Size = new System.Drawing.Size(114, 29);
            numNumCount.TabIndex = 8;
            numNumCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            numNumCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numNumCount.Enter += TypeInput_Enter;
            // 
            // lbNameRightClickMenuStrip
            // 
            lbNameRightClickMenuStrip.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbNameRightClickMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { menuCleanAll, menuDelete, menuModify, menuInsertPrev, menuInsertNext });
            lbNameRightClickMenuStrip.Name = "lbNameRightClickMenuStrip";
            lbNameRightClickMenuStrip.Size = new System.Drawing.Size(177, 134);
            // 
            // menuCleanAll
            // 
            menuCleanAll.Name = "menuCleanAll";
            menuCleanAll.Size = new System.Drawing.Size(176, 26);
            menuCleanAll.Text = "清除全部";
            menuCleanAll.Click += listRightClickMenu_Click;
            // 
            // menuDelete
            // 
            menuDelete.Name = "menuDelete";
            menuDelete.Size = new System.Drawing.Size(176, 26);
            menuDelete.Text = "删除";
            menuDelete.Click += listRightClickMenu_Click;
            // 
            // menuModify
            // 
            menuModify.Name = "menuModify";
            menuModify.Size = new System.Drawing.Size(176, 26);
            menuModify.Text = "修改";
            menuModify.Click += listRightClickMenu_Click;
            // 
            // menuInsertPrev
            // 
            menuInsertPrev.Name = "menuInsertPrev";
            menuInsertPrev.Size = new System.Drawing.Size(176, 26);
            menuInsertPrev.Text = "在上一行插入";
            menuInsertPrev.Click += listRightClickMenu_Click;
            // 
            // menuInsertNext
            // 
            menuInsertNext.Name = "menuInsertNext";
            menuInsertNext.Size = new System.Drawing.Size(176, 26);
            menuInsertNext.Text = "在下一行插入";
            menuInsertNext.Click += listRightClickMenu_Click;
            // 
            // TextCreator
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(gbCreator);
            Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "TextCreator";
            Size = new System.Drawing.Size(703, 453);
            gbCreator.ResumeLayout(false);
            gbCreator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCharCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numNumCount).EndInit();
            lbNameRightClickMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
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
