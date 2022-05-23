
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
            this.panelExeCheck = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbKey1 = new System.Windows.Forms.TextBox();
            this.tbKey2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.panelExeCheck.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelExeCheck
            // 
            this.panelExeCheck.AllowDrop = true;
            this.panelExeCheck.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelExeCheck.Controls.Add(this.label1);
            this.panelExeCheck.Location = new System.Drawing.Point(12, 12);
            this.panelExeCheck.Name = "panelExeCheck";
            this.panelExeCheck.Size = new System.Drawing.Size(804, 88);
            this.panelExeCheck.TabIndex = 0;
            this.panelExeCheck.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelExeCheck_DragDrop);
            this.panelExeCheck.DragEnter += new System.Windows.Forms.DragEventHandler(this.File_DragEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(272, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 42);
            this.label1.TabIndex = 0;
            this.label1.Text = "请拖拽主程序exe/dll至此处>_<\r\n(确保已脱壳)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "Key1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(593, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "Key2";
            // 
            // tbKey1
            // 
            this.tbKey1.Location = new System.Drawing.Point(64, 110);
            this.tbKey1.MaxLength = 8;
            this.tbKey1.Name = "tbKey1";
            this.tbKey1.Size = new System.Drawing.Size(162, 29);
            this.tbKey1.TabIndex = 3;
            // 
            // tbKey2
            // 
            this.tbKey2.Location = new System.Drawing.Point(645, 110);
            this.tbKey2.MaxLength = 8;
            this.tbKey2.Name = "tbKey2";
            this.tbKey2.Size = new System.Drawing.Size(171, 29);
            this.tbKey2.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(326, 113);
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
            this.listBoxFiles.Location = new System.Drawing.Point(12, 145);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.ScrollAlwaysVisible = true;
            this.listBoxFiles.Size = new System.Drawing.Size(804, 256);
            this.listBoxFiles.TabIndex = 6;
            this.listBoxFiles.TabStop = false;
            this.listBoxFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxFiles_DragDrop);
            this.listBoxFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.File_DragEnter);
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(330, 419);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(163, 30);
            this.btnExtract.TabIndex = 7;
            this.btnExtract.Text = "一键解包";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(828, 467);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.listBoxFiles);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbKey2);
            this.Controls.Add(this.tbKey1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panelExeCheck);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "MainForm";
            this.Text = "NVL KR2 Extractor";
            this.panelExeCheck.ResumeLayout(false);
            this.panelExeCheck.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelExeCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbKey1;
        private System.Windows.Forms.TextBox tbKey2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button btnExtract;
    }
}

