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
            tCreator = new TextCreator();
            btnLoadAutoPath = new System.Windows.Forms.Button();
            btnLoadDumpFile = new System.Windows.Forms.Button();
            btnSelectArchiveDirectory = new System.Windows.Forms.Button();
            btnEnumPath = new System.Windows.Forms.Button();
            btnEnumPathWithAutoPath = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // tCreator
            // 
            tCreator.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tCreator.AutoPath = null;
            tCreator.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            tCreator.Location = new System.Drawing.Point(13, 12);
            tCreator.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tCreator.Name = "tCreator";
            tCreator.Size = new System.Drawing.Size(703, 538);
            tCreator.TabIndex = 0;
            tCreator.TextProcessCallBack = null;
            // 
            // btnLoadAutoPath
            // 
            btnLoadAutoPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnLoadAutoPath.Location = new System.Drawing.Point(723, 57);
            btnLoadAutoPath.Name = "btnLoadAutoPath";
            btnLoadAutoPath.Size = new System.Drawing.Size(162, 39);
            btnLoadAutoPath.TabIndex = 3;
            btnLoadAutoPath.Text = "加载自动路径";
            btnLoadAutoPath.UseVisualStyleBackColor = true;
            btnLoadAutoPath.Click += btnLoadAutoPath_Click;
            // 
            // btnLoadDumpFile
            // 
            btnLoadDumpFile.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnLoadDumpFile.Location = new System.Drawing.Point(908, 12);
            btnLoadDumpFile.Name = "btnLoadDumpFile";
            btnLoadDumpFile.Size = new System.Drawing.Size(162, 57);
            btnLoadDumpFile.TabIndex = 5;
            btnLoadDumpFile.Text = "加载Dump的文件名还原";
            btnLoadDumpFile.UseVisualStyleBackColor = true;
            btnLoadDumpFile.Click += btnLoadDumpFile_Click;
            // 
            // btnSelectArchiveDirectory
            // 
            btnSelectArchiveDirectory.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnSelectArchiveDirectory.Location = new System.Drawing.Point(723, 12);
            btnSelectArchiveDirectory.Name = "btnSelectArchiveDirectory";
            btnSelectArchiveDirectory.Size = new System.Drawing.Size(162, 39);
            btnSelectArchiveDirectory.TabIndex = 6;
            btnSelectArchiveDirectory.Text = "选择目标文件夹";
            btnSelectArchiveDirectory.UseVisualStyleBackColor = true;
            btnSelectArchiveDirectory.Click += btnSelectArchiveDirectory_Click;
            // 
            // btnEnumPath
            // 
            btnEnumPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnEnumPath.Location = new System.Drawing.Point(908, 75);
            btnEnumPath.Name = "btnEnumPath";
            btnEnumPath.Size = new System.Drawing.Size(162, 56);
            btnEnumPath.TabIndex = 7;
            btnEnumPath.Text = "使用本地路径还原";
            btnEnumPath.UseVisualStyleBackColor = true;
            btnEnumPath.Click += btnEnumPath_Click;
            // 
            // btnEnumPathWithAutoPath
            // 
            btnEnumPathWithAutoPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnEnumPathWithAutoPath.Location = new System.Drawing.Point(908, 137);
            btnEnumPathWithAutoPath.Name = "btnEnumPathWithAutoPath";
            btnEnumPathWithAutoPath.Size = new System.Drawing.Size(162, 54);
            btnEnumPathWithAutoPath.TabIndex = 8;
            btnEnumPathWithAutoPath.Text = "使用本地路径还原(AutoPath)";
            btnEnumPathWithAutoPath.UseVisualStyleBackColor = true;
            btnEnumPathWithAutoPath.Click += btnEnumPathWithAutoPath_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1093, 562);
            Controls.Add(btnEnumPathWithAutoPath);
            Controls.Add(btnEnumPath);
            Controls.Add(btnSelectArchiveDirectory);
            Controls.Add(btnLoadDumpFile);
            Controls.Add(btnLoadAutoPath);
            Controls.Add(tCreator);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(4);
            MinimumSize = new System.Drawing.Size(800, 600);
            Name = "MainForm";
            Text = "NVLKR2 Hash Decoder";
            ResumeLayout(false);
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