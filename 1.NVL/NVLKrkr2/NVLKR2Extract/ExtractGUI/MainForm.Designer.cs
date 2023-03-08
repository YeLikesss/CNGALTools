
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
            this.label4 = new System.Windows.Forms.Label();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbTitles = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(167, 21);
            this.label4.TabIndex = 5;
            this.label4.Text = "请拖拽XP3文件到下方";
            // 
            // listBoxFiles
            // 
            this.listBoxFiles.AllowDrop = true;
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.HorizontalScrollbar = true;
            this.listBoxFiles.ItemHeight = 21;
            this.listBoxFiles.Location = new System.Drawing.Point(12, 71);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.ScrollAlwaysVisible = true;
            this.listBoxFiles.Size = new System.Drawing.Size(804, 214);
            this.listBoxFiles.TabIndex = 6;
            this.listBoxFiles.TabStop = false;
            this.listBoxFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxFiles_DragDrop);
            this.listBoxFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.File_DragEnter);
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(653, 7);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(163, 35);
            this.btnExtract.TabIndex = 7;
            this.btnExtract.Text = "解包";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 21);
            this.label1.TabIndex = 8;
            this.label1.Text = "游戏 :";
            // 
            // cbTitles
            // 
            this.cbTitles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTitles.FormattingEnabled = true;
            this.cbTitles.Location = new System.Drawing.Point(69, 9);
            this.cbTitles.Name = "cbTitles";
            this.cbTitles.Size = new System.Drawing.Size(345, 29);
            this.cbTitles.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(828, 306);
            this.Controls.Add(this.cbTitles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.listBoxFiles);
            this.Controls.Add(this.label4);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "MainForm";
            this.Text = "NVL KR2 Extractor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbTitles;
    }
}

