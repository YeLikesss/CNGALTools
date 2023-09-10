namespace HashDecoder
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tCreator = new HashDecoder.TextCreator();
            this.btnLoadAutoPath = new System.Windows.Forms.Button();
            this.btnLoadDumpFile = new System.Windows.Forms.Button();
            this.btnSelectArchiveDirectory = new System.Windows.Forms.Button();
            this.btnEnumPath = new System.Windows.Forms.Button();
            this.btnEnumPathWithAutoPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tCreator
            // 
            this.tCreator.AutoPath = null;
            this.tCreator.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tCreator.Location = new System.Drawing.Point(13, 12);
            this.tCreator.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tCreator.Name = "tCreator";
            this.tCreator.Size = new System.Drawing.Size(703, 453);
            this.tCreator.TabIndex = 0;
            this.tCreator.TextProcessCallBack = null;
            // 
            // btnLoadAutoPath
            // 
            this.btnLoadAutoPath.Location = new System.Drawing.Point(723, 57);
            this.btnLoadAutoPath.Name = "btnLoadAutoPath";
            this.btnLoadAutoPath.Size = new System.Drawing.Size(162, 39);
            this.btnLoadAutoPath.TabIndex = 3;
            this.btnLoadAutoPath.Text = "加载自动路径";
            this.btnLoadAutoPath.UseVisualStyleBackColor = true;
            this.btnLoadAutoPath.Click += new System.EventHandler(this.btnLoadAutoPath_Click);
            // 
            // btnLoadDumpFile
            // 
            this.btnLoadDumpFile.Location = new System.Drawing.Point(908, 12);
            this.btnLoadDumpFile.Name = "btnLoadDumpFile";
            this.btnLoadDumpFile.Size = new System.Drawing.Size(162, 57);
            this.btnLoadDumpFile.TabIndex = 5;
            this.btnLoadDumpFile.Text = "加载Dump的文件名还原";
            this.btnLoadDumpFile.UseVisualStyleBackColor = true;
            this.btnLoadDumpFile.Click += new System.EventHandler(this.btnLoadDumpFile_Click);
            // 
            // btnSelectArchiveDirectory
            // 
            this.btnSelectArchiveDirectory.Location = new System.Drawing.Point(723, 12);
            this.btnSelectArchiveDirectory.Name = "btnSelectArchiveDirectory";
            this.btnSelectArchiveDirectory.Size = new System.Drawing.Size(162, 39);
            this.btnSelectArchiveDirectory.TabIndex = 6;
            this.btnSelectArchiveDirectory.Text = "选择目标文件夹";
            this.btnSelectArchiveDirectory.UseVisualStyleBackColor = true;
            this.btnSelectArchiveDirectory.Click += new System.EventHandler(this.btnSelectArchiveDirectory_Click);
            // 
            // btnEnumPath
            // 
            this.btnEnumPath.Location = new System.Drawing.Point(908, 75);
            this.btnEnumPath.Name = "btnEnumPath";
            this.btnEnumPath.Size = new System.Drawing.Size(162, 56);
            this.btnEnumPath.TabIndex = 7;
            this.btnEnumPath.Text = "使用本地路径还原";
            this.btnEnumPath.UseVisualStyleBackColor = true;
            this.btnEnumPath.Click += new System.EventHandler(this.btnEnumPath_Click);
            // 
            // btnEnumPathWithAutoPath
            // 
            this.btnEnumPathWithAutoPath.Location = new System.Drawing.Point(908, 137);
            this.btnEnumPathWithAutoPath.Name = "btnEnumPathWithAutoPath";
            this.btnEnumPathWithAutoPath.Size = new System.Drawing.Size(162, 54);
            this.btnEnumPathWithAutoPath.TabIndex = 8;
            this.btnEnumPathWithAutoPath.Text = "使用本地路径还原(AutoPath)";
            this.btnEnumPathWithAutoPath.UseVisualStyleBackColor = true;
            this.btnEnumPathWithAutoPath.Click += new System.EventHandler(this.btnEnumPathWithAutoPath_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1093, 475);
            this.Controls.Add(this.btnEnumPathWithAutoPath);
            this.Controls.Add(this.btnEnumPath);
            this.Controls.Add(this.btnSelectArchiveDirectory);
            this.Controls.Add(this.btnLoadDumpFile);
            this.Controls.Add(this.btnLoadAutoPath);
            this.Controls.Add(this.tCreator);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "NVLKR2 Hash Decoder";
            this.ResumeLayout(false);

        }

        #endregion

        private TextCreator tCreator;
        private System.Windows.Forms.Button btnLoadAutoPath;
        private System.Windows.Forms.Button btnLoadDumpFile;
        private System.Windows.Forms.Button btnSelectArchiveDirectory;
        private System.Windows.Forms.Button btnEnumPath;
        private System.Windows.Forms.Button btnEnumPathWithAutoPath;
    }
}