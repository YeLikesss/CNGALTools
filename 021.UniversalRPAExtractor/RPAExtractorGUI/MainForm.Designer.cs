namespace RPAExtractorGUI
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
            System.Windows.Forms.Label lbDragFileTips;
            System.Windows.Forms.Label lbFileList;
            System.Windows.Forms.Label lbLog;
            System.Windows.Forms.Button btnExtract;
            System.Windows.Forms.StatusStrip statusStrip;
            tssVersion = new System.Windows.Forms.ToolStripStatusLabel();
            tssPackageName = new System.Windows.Forms.ToolStripStatusLabel();
            tssProgress = new System.Windows.Forms.ToolStripStatusLabel();
            lbFiles = new System.Windows.Forms.ListBox();
            tbLog = new System.Windows.Forms.TextBox();
            lbDragFileTips = new System.Windows.Forms.Label();
            lbFileList = new System.Windows.Forms.Label();
            lbLog = new System.Windows.Forms.Label();
            btnExtract = new System.Windows.Forms.Button();
            statusStrip = new System.Windows.Forms.StatusStrip();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // lbDragFileTips
            // 
            lbDragFileTips.AllowDrop = true;
            lbDragFileTips.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbDragFileTips.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbDragFileTips.Location = new System.Drawing.Point(12, 12);
            lbDragFileTips.Margin = new System.Windows.Forms.Padding(3);
            lbDragFileTips.Name = "lbDragFileTips";
            lbDragFileTips.Size = new System.Drawing.Size(760, 82);
            lbDragFileTips.TabIndex = 0;
            lbDragFileTips.Text = "拖拽封包文件到此处";
            lbDragFileTips.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbDragFileTips.UseMnemonic = false;
            lbDragFileTips.DragDrop += DragFileTips_OnDragDrop;
            lbDragFileTips.DragEnter += DragFileTips_OnDragEnter;
            // 
            // lbFileList
            // 
            lbFileList.AutoSize = true;
            lbFileList.Location = new System.Drawing.Point(12, 97);
            lbFileList.Name = "lbFileList";
            lbFileList.Size = new System.Drawing.Size(80, 17);
            lbFileList.TabIndex = 1;
            lbFileList.Text = "文件处理列表";
            lbFileList.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbFileList.UseMnemonic = false;
            // 
            // lbLog
            // 
            lbLog.AutoSize = true;
            lbLog.Location = new System.Drawing.Point(12, 241);
            lbLog.Name = "lbLog";
            lbLog.Size = new System.Drawing.Size(32, 17);
            lbLog.TabIndex = 3;
            lbLog.Text = "日志";
            lbLog.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbLog.UseMnemonic = false;
            // 
            // btnExtract
            // 
            btnExtract.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnExtract.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnExtract.Location = new System.Drawing.Point(676, 492);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new System.Drawing.Size(96, 36);
            btnExtract.TabIndex = 5;
            btnExtract.Text = "解包";
            btnExtract.UseMnemonic = false;
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += BtnExtract_Click;
            // 
            // statusStrip
            // 
            statusStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tssVersion, tssPackageName, tssProgress });
            statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            statusStrip.Location = new System.Drawing.Point(0, 540);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new System.Drawing.Size(784, 22);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 6;
            // 
            // tssVersion
            // 
            tssVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            tssVersion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            tssVersion.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            tssVersion.Name = "tssVersion";
            tssVersion.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            tssVersion.Size = new System.Drawing.Size(24, 17);
            tssVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tssPackageName
            // 
            tssPackageName.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            tssPackageName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            tssPackageName.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            tssPackageName.Name = "tssPackageName";
            tssPackageName.Padding = new System.Windows.Forms.Padding(15, 0, 15, 0);
            tssPackageName.Size = new System.Drawing.Size(34, 17);
            // 
            // tssProgress
            // 
            tssProgress.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            tssProgress.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            tssProgress.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            tssProgress.Name = "tssProgress";
            tssProgress.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            tssProgress.Size = new System.Drawing.Size(14, 17);
            // 
            // lbFiles
            // 
            lbFiles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbFiles.HorizontalScrollbar = true;
            lbFiles.IntegralHeight = false;
            lbFiles.ItemHeight = 17;
            lbFiles.Location = new System.Drawing.Point(12, 117);
            lbFiles.Name = "lbFiles";
            lbFiles.SelectionMode = System.Windows.Forms.SelectionMode.None;
            lbFiles.Size = new System.Drawing.Size(760, 121);
            lbFiles.TabIndex = 2;
            // 
            // tbLog
            // 
            tbLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbLog.Location = new System.Drawing.Point(12, 261);
            tbLog.MaxLength = 65535;
            tbLog.Multiline = true;
            tbLog.Name = "tbLog";
            tbLog.ReadOnly = true;
            tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            tbLog.Size = new System.Drawing.Size(760, 220);
            tbLog.TabIndex = 4;
            tbLog.WordWrap = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ClientSize = new System.Drawing.Size(784, 562);
            Controls.Add(statusStrip);
            Controls.Add(btnExtract);
            Controls.Add(tbLog);
            Controls.Add(lbLog);
            Controls.Add(lbDragFileTips);
            Controls.Add(lbFiles);
            Controls.Add(lbFileList);
            DoubleBuffered = true;
            ImeMode = System.Windows.Forms.ImeMode.Disable;
            MinimumSize = new System.Drawing.Size(600, 450);
            Name = "MainForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "RPAExtractor";
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.ToolStripStatusLabel tssVersion;
        private System.Windows.Forms.ToolStripStatusLabel tssPackageName;
        private System.Windows.Forms.ToolStripStatusLabel tssProgress;
    }
}