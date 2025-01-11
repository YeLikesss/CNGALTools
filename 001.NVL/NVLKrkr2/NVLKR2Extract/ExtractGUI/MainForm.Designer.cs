
namespace ExtractorGUI
{
    partial class MainForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            label4 = new System.Windows.Forms.Label();
            listBoxFiles = new System.Windows.Forms.ListBox();
            btnExtract = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            cbTitles = new System.Windows.Forms.ComboBox();
            SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 47);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(167, 21);
            label4.TabIndex = 5;
            label4.Text = "请拖拽XP3文件到下方";
            // 
            // listBoxFiles
            // 
            listBoxFiles.AllowDrop = true;
            listBoxFiles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            listBoxFiles.HorizontalScrollbar = true;
            listBoxFiles.IntegralHeight = false;
            listBoxFiles.ItemHeight = 21;
            listBoxFiles.Location = new System.Drawing.Point(12, 71);
            listBoxFiles.Name = "listBoxFiles";
            listBoxFiles.ScrollAlwaysVisible = true;
            listBoxFiles.Size = new System.Drawing.Size(804, 223);
            listBoxFiles.TabIndex = 6;
            listBoxFiles.TabStop = false;
            listBoxFiles.DragDrop += listBoxFiles_DragDrop;
            listBoxFiles.DragEnter += File_DragEnter;
            // 
            // btnExtract
            // 
            btnExtract.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnExtract.Location = new System.Drawing.Point(653, 7);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new System.Drawing.Size(163, 35);
            btnExtract.TabIndex = 7;
            btnExtract.Text = "解包";
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += btnExtract_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 12);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(51, 21);
            label1.TabIndex = 8;
            label1.Text = "游戏 :";
            // 
            // cbTitles
            // 
            cbTitles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTitles.FormattingEnabled = true;
            cbTitles.Location = new System.Drawing.Point(69, 9);
            cbTitles.Name = "cbTitles";
            cbTitles.Size = new System.Drawing.Size(345, 29);
            cbTitles.TabIndex = 9;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(828, 306);
            Controls.Add(cbTitles);
            Controls.Add(label1);
            Controls.Add(btnExtract);
            Controls.Add(listBoxFiles);
            Controls.Add(label4);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ImeMode = System.Windows.Forms.ImeMode.Disable;
            Margin = new System.Windows.Forms.Padding(5);
            MinimumSize = new System.Drawing.Size(640, 240);
            Name = "MainForm";
            Text = "NVL KR2 Extractor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbTitles;
    }
}

