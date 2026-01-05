namespace ExtractorGUI
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
            System.Windows.Forms.Label lbLog;
            System.Windows.Forms.Label lbDragFileTips;
            System.Windows.Forms.Label lbFileList;
            System.Windows.Forms.Button btnExtractAsset;
            System.Windows.Forms.Button btnExtractScenario;
            System.Windows.Forms.Label lbTitles;
            System.Windows.Forms.Label lbKey;
            System.Windows.Forms.Label lbIV;
            tbLog = new System.Windows.Forms.TextBox();
            lbFiles = new System.Windows.Forms.ListBox();
            cbTitles = new System.Windows.Forms.ComboBox();
            tbKey = new System.Windows.Forms.TextBox();
            tbIV = new System.Windows.Forms.TextBox();
            lbLog = new System.Windows.Forms.Label();
            lbDragFileTips = new System.Windows.Forms.Label();
            lbFileList = new System.Windows.Forms.Label();
            btnExtractAsset = new System.Windows.Forms.Button();
            btnExtractScenario = new System.Windows.Forms.Button();
            lbTitles = new System.Windows.Forms.Label();
            lbKey = new System.Windows.Forms.Label();
            lbIV = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // lbLog
            // 
            lbLog.AutoSize = true;
            lbLog.Location = new System.Drawing.Point(12, 269);
            lbLog.Name = "lbLog";
            lbLog.Size = new System.Drawing.Size(32, 17);
            lbLog.TabIndex = 8;
            lbLog.Text = "日志";
            lbLog.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbLog.UseMnemonic = false;
            // 
            // lbDragFileTips
            // 
            lbDragFileTips.AllowDrop = true;
            lbDragFileTips.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbDragFileTips.Location = new System.Drawing.Point(12, 12);
            lbDragFileTips.Margin = new System.Windows.Forms.Padding(3);
            lbDragFileTips.Name = "lbDragFileTips";
            lbDragFileTips.Size = new System.Drawing.Size(245, 110);
            lbDragFileTips.TabIndex = 5;
            lbDragFileTips.Text = "拖拽封包文件到此处";
            lbDragFileTips.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbDragFileTips.UseMnemonic = false;
            lbDragFileTips.DragDrop += DragFileTips_OnDragDrop;
            lbDragFileTips.DragEnter += DragFileTips_OnDragEnter;
            // 
            // lbFileList
            // 
            lbFileList.AutoSize = true;
            lbFileList.Location = new System.Drawing.Point(12, 125);
            lbFileList.Name = "lbFileList";
            lbFileList.Size = new System.Drawing.Size(80, 17);
            lbFileList.TabIndex = 6;
            lbFileList.Text = "文件处理列表";
            lbFileList.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbFileList.UseMnemonic = false;
            // 
            // btnExtractAsset
            // 
            btnExtractAsset.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnExtractAsset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnExtractAsset.Location = new System.Drawing.Point(676, 518);
            btnExtractAsset.Name = "btnExtractAsset";
            btnExtractAsset.Size = new System.Drawing.Size(96, 35);
            btnExtractAsset.TabIndex = 10;
            btnExtractAsset.Tag = "ExtractPackage";
            btnExtractAsset.Text = "解包资源";
            btnExtractAsset.UseMnemonic = false;
            btnExtractAsset.UseVisualStyleBackColor = true;
            btnExtractAsset.Click += BtnExtract_Click;
            // 
            // btnExtractScenario
            // 
            btnExtractScenario.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnExtractScenario.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnExtractScenario.Location = new System.Drawing.Point(574, 518);
            btnExtractScenario.Name = "btnExtractScenario";
            btnExtractScenario.Size = new System.Drawing.Size(96, 35);
            btnExtractScenario.TabIndex = 11;
            btnExtractScenario.Tag = "ExtractScenario";
            btnExtractScenario.Text = "解包脚本";
            btnExtractScenario.UseMnemonic = false;
            btnExtractScenario.UseVisualStyleBackColor = true;
            btnExtractScenario.Click += BtnExtract_Click;
            // 
            // lbTitles
            // 
            lbTitles.AutoSize = true;
            lbTitles.Location = new System.Drawing.Point(263, 12);
            lbTitles.Name = "lbTitles";
            lbTitles.Size = new System.Drawing.Size(56, 17);
            lbTitles.TabIndex = 12;
            lbTitles.Text = "游戏标题";
            lbTitles.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbTitles.UseMnemonic = false;
            // 
            // lbKey
            // 
            lbKey.AutoSize = true;
            lbKey.Location = new System.Drawing.Point(263, 43);
            lbKey.Name = "lbKey";
            lbKey.Size = new System.Drawing.Size(29, 17);
            lbKey.TabIndex = 14;
            lbKey.Text = "Key";
            lbKey.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbKey.UseMnemonic = false;
            // 
            // lbIV
            // 
            lbIV.AutoSize = true;
            lbIV.Location = new System.Drawing.Point(272, 72);
            lbIV.Name = "lbIV";
            lbIV.Size = new System.Drawing.Size(20, 17);
            lbIV.TabIndex = 15;
            lbIV.Text = "IV";
            lbIV.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbIV.UseMnemonic = false;
            // 
            // tbLog
            // 
            tbLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbLog.Location = new System.Drawing.Point(12, 289);
            tbLog.MaxLength = 65535;
            tbLog.Multiline = true;
            tbLog.Name = "tbLog";
            tbLog.ReadOnly = true;
            tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            tbLog.Size = new System.Drawing.Size(760, 220);
            tbLog.TabIndex = 9;
            tbLog.WordWrap = false;
            // 
            // lbFiles
            // 
            lbFiles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lbFiles.HorizontalScrollbar = true;
            lbFiles.IntegralHeight = false;
            lbFiles.ItemHeight = 17;
            lbFiles.Location = new System.Drawing.Point(12, 145);
            lbFiles.Name = "lbFiles";
            lbFiles.SelectionMode = System.Windows.Forms.SelectionMode.None;
            lbFiles.Size = new System.Drawing.Size(760, 121);
            lbFiles.TabIndex = 7;
            // 
            // cbTitles
            // 
            cbTitles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cbTitles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTitles.Location = new System.Drawing.Point(325, 9);
            cbTitles.Name = "cbTitles";
            cbTitles.Size = new System.Drawing.Size(447, 25);
            cbTitles.TabIndex = 13;
            cbTitles.SelectedIndexChanged += CbTitles_OnSelectedIndexChanged;
            // 
            // tbKey
            // 
            tbKey.Location = new System.Drawing.Point(298, 40);
            tbKey.Name = "tbKey";
            tbKey.ReadOnly = true;
            tbKey.Size = new System.Drawing.Size(201, 23);
            tbKey.TabIndex = 16;
            tbKey.WordWrap = false;
            // 
            // tbIV
            // 
            tbIV.Location = new System.Drawing.Point(298, 69);
            tbIV.Name = "tbIV";
            tbIV.ReadOnly = true;
            tbIV.Size = new System.Drawing.Size(201, 23);
            tbIV.TabIndex = 17;
            tbIV.WordWrap = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ClientSize = new System.Drawing.Size(784, 562);
            Controls.Add(tbIV);
            Controls.Add(tbKey);
            Controls.Add(lbIV);
            Controls.Add(lbKey);
            Controls.Add(cbTitles);
            Controls.Add(lbTitles);
            Controls.Add(btnExtractScenario);
            Controls.Add(btnExtractAsset);
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
            Text = "Yuri AVG Engine Extractor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.TextBox tbKey;
        private System.Windows.Forms.TextBox tbIV;
        private System.Windows.Forms.ComboBox cbTitles;
    }
}