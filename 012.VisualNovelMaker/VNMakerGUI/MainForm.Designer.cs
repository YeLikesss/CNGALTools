namespace VNMakerGUI
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
            System.Windows.Forms.Label lbGamesTip;
            System.Windows.Forms.Button btnExtract;
            System.Windows.Forms.Label lbFilesTip;
            System.Windows.Forms.Label lbLogTips;
            cbGames = new System.Windows.Forms.ComboBox();
            tbDescription = new System.Windows.Forms.TextBox();
            lbFiles = new System.Windows.Forms.ListBox();
            tbLog = new System.Windows.Forms.TextBox();
            lbGamesTip = new System.Windows.Forms.Label();
            btnExtract = new System.Windows.Forms.Button();
            lbFilesTip = new System.Windows.Forms.Label();
            lbLogTips = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // lbGamesTip
            // 
            lbGamesTip.AutoSize = true;
            lbGamesTip.Location = new System.Drawing.Point(12, 15);
            lbGamesTip.Name = "lbGamesTip";
            lbGamesTip.Size = new System.Drawing.Size(63, 17);
            lbGamesTip.TabIndex = 0;
            lbGamesTip.Text = "游戏标题: ";
            lbGamesTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbGamesTip.UseMnemonic = false;
            // 
            // btnExtract
            // 
            btnExtract.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnExtract.Location = new System.Drawing.Point(679, 12);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new System.Drawing.Size(93, 54);
            btnExtract.TabIndex = 2;
            btnExtract.Text = "提取";
            btnExtract.UseMnemonic = false;
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += BtnExtract_OnClick;
            // 
            // lbFilesTip
            // 
            lbFilesTip.AutoSize = true;
            lbFilesTip.Location = new System.Drawing.Point(12, 69);
            lbFilesTip.Name = "lbFilesTip";
            lbFilesTip.Size = new System.Drawing.Size(63, 17);
            lbFilesTip.TabIndex = 4;
            lbFilesTip.Text = "文件列表: ";
            lbFilesTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbFilesTip.UseMnemonic = false;
            // 
            // lbLogTips
            // 
            lbLogTips.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lbLogTips.AutoSize = true;
            lbLogTips.Location = new System.Drawing.Point(12, 380);
            lbLogTips.Name = "lbLogTips";
            lbLogTips.Size = new System.Drawing.Size(39, 17);
            lbLogTips.TabIndex = 6;
            lbLogTips.Text = "日志: ";
            lbLogTips.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbLogTips.UseMnemonic = false;
            // 
            // cbGames
            // 
            cbGames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbGames.IntegralHeight = false;
            cbGames.Location = new System.Drawing.Point(81, 12);
            cbGames.Name = "cbGames";
            cbGames.Size = new System.Drawing.Size(305, 25);
            cbGames.TabIndex = 1;
            cbGames.SelectedIndexChanged += ComboBoxGames_OnSelectedIndexChanged;
            // 
            // tbDescription
            // 
            tbDescription.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbDescription.Location = new System.Drawing.Point(12, 43);
            tbDescription.Name = "tbDescription";
            tbDescription.ReadOnly = true;
            tbDescription.Size = new System.Drawing.Size(657, 23);
            tbDescription.TabIndex = 3;
            // 
            // lbFiles
            // 
            lbFiles.AllowDrop = true;
            lbFiles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbFiles.HorizontalScrollbar = true;
            lbFiles.IntegralHeight = false;
            lbFiles.ItemHeight = 17;
            lbFiles.Location = new System.Drawing.Point(12, 89);
            lbFiles.Name = "lbFiles";
            lbFiles.ScrollAlwaysVisible = true;
            lbFiles.SelectionMode = System.Windows.Forms.SelectionMode.None;
            lbFiles.Size = new System.Drawing.Size(760, 288);
            lbFiles.TabIndex = 5;
            lbFiles.DragDrop += ListBoxFiles_OnDragDrop;
            lbFiles.DragEnter += ListBoxFiles_OnDragEnter;
            // 
            // tbLog
            // 
            tbLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbLog.Location = new System.Drawing.Point(12, 400);
            tbLog.MaxLength = 65535;
            tbLog.Multiline = true;
            tbLog.Name = "tbLog";
            tbLog.ReadOnly = true;
            tbLog.Size = new System.Drawing.Size(760, 150);
            tbLog.TabIndex = 7;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(784, 562);
            Controls.Add(tbLog);
            Controls.Add(lbLogTips);
            Controls.Add(lbFiles);
            Controls.Add(lbFilesTip);
            Controls.Add(tbDescription);
            Controls.Add(btnExtract);
            Controls.Add(cbGames);
            Controls.Add(lbGamesTip);
            DoubleBuffered = true;
            ImeMode = System.Windows.Forms.ImeMode.Disable;
            MinimumSize = new System.Drawing.Size(800, 600);
            Name = "MainForm";
            Text = "Visual Novel Maker Extractor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox cbGames;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.TextBox tbLog;
    }
}