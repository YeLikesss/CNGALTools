namespace GCExtractorGUI
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
            System.Windows.Forms.Label lbSelectGames;
            System.Windows.Forms.Button btnExtract;
            System.Windows.Forms.Label lbEngineTips;
            System.Windows.Forms.Label lbGameDirectory;
            System.Windows.Forms.Button btnSelectDirectory;
            System.Windows.Forms.Label lbLog;
            cbTitles = new System.Windows.Forms.ComboBox();
            tbEngineDescription = new System.Windows.Forms.TextBox();
            tbGameDirectory = new System.Windows.Forms.TextBox();
            tbLog = new System.Windows.Forms.TextBox();
            lbSelectGames = new System.Windows.Forms.Label();
            btnExtract = new System.Windows.Forms.Button();
            lbEngineTips = new System.Windows.Forms.Label();
            lbGameDirectory = new System.Windows.Forms.Label();
            btnSelectDirectory = new System.Windows.Forms.Button();
            lbLog = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // lbSelectGames
            // 
            lbSelectGames.AutoSize = true;
            lbSelectGames.Location = new System.Drawing.Point(12, 15);
            lbSelectGames.Name = "lbSelectGames";
            lbSelectGames.Size = new System.Drawing.Size(63, 17);
            lbSelectGames.TabIndex = 0;
            lbSelectGames.Text = "选择游戏: ";
            lbSelectGames.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbSelectGames.UseMnemonic = false;
            // 
            // btnExtract
            // 
            btnExtract.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnExtract.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnExtract.Location = new System.Drawing.Point(800, 12);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new System.Drawing.Size(132, 80);
            btnExtract.TabIndex = 2;
            btnExtract.Text = "提取资源";
            btnExtract.UseMnemonic = false;
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += BtnExtract_Click;
            // 
            // lbEngineTips
            // 
            lbEngineTips.AutoSize = true;
            lbEngineTips.Location = new System.Drawing.Point(36, 46);
            lbEngineTips.Name = "lbEngineTips";
            lbEngineTips.Size = new System.Drawing.Size(39, 17);
            lbEngineTips.TabIndex = 4;
            lbEngineTips.Text = "描述: ";
            lbEngineTips.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbEngineTips.UseMnemonic = false;
            // 
            // lbGameDirectory
            // 
            lbGameDirectory.AutoSize = true;
            lbGameDirectory.Location = new System.Drawing.Point(12, 75);
            lbGameDirectory.Name = "lbGameDirectory";
            lbGameDirectory.Size = new System.Drawing.Size(63, 17);
            lbGameDirectory.TabIndex = 5;
            lbGameDirectory.Text = "游戏路径: ";
            lbGameDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbGameDirectory.UseMnemonic = false;
            // 
            // btnSelectDirectory
            // 
            btnSelectDirectory.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnSelectDirectory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            btnSelectDirectory.Location = new System.Drawing.Point(741, 72);
            btnSelectDirectory.Name = "btnSelectDirectory";
            btnSelectDirectory.Size = new System.Drawing.Size(42, 23);
            btnSelectDirectory.TabIndex = 7;
            btnSelectDirectory.Text = "...";
            btnSelectDirectory.UseMnemonic = false;
            btnSelectDirectory.UseVisualStyleBackColor = true;
            btnSelectDirectory.Click += BtnSelectDirectory_OnClick;
            // 
            // lbLog
            // 
            lbLog.AutoSize = true;
            lbLog.Location = new System.Drawing.Point(36, 101);
            lbLog.Name = "lbLog";
            lbLog.Size = new System.Drawing.Size(39, 17);
            lbLog.TabIndex = 8;
            lbLog.Text = "日志: ";
            lbLog.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbLog.UseMnemonic = false;
            // 
            // cbTitles
            // 
            cbTitles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cbTitles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTitles.IntegralHeight = false;
            cbTitles.Location = new System.Drawing.Point(81, 12);
            cbTitles.Name = "cbTitles";
            cbTitles.Size = new System.Drawing.Size(702, 25);
            cbTitles.TabIndex = 1;
            cbTitles.SelectedIndexChanged += CbTitles_OnSelectedIndexChanged;
            // 
            // tbEngineDescription
            // 
            tbEngineDescription.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbEngineDescription.Location = new System.Drawing.Point(81, 43);
            tbEngineDescription.Name = "tbEngineDescription";
            tbEngineDescription.ReadOnly = true;
            tbEngineDescription.Size = new System.Drawing.Size(702, 23);
            tbEngineDescription.TabIndex = 3;
            tbEngineDescription.TabStop = false;
            tbEngineDescription.WordWrap = false;
            // 
            // tbGameDirectory
            // 
            tbGameDirectory.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbGameDirectory.Location = new System.Drawing.Point(81, 72);
            tbGameDirectory.Name = "tbGameDirectory";
            tbGameDirectory.ReadOnly = true;
            tbGameDirectory.Size = new System.Drawing.Size(654, 23);
            tbGameDirectory.TabIndex = 6;
            tbGameDirectory.TabStop = false;
            tbGameDirectory.WordWrap = false;
            // 
            // tbLog
            // 
            tbLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbLog.Location = new System.Drawing.Point(12, 128);
            tbLog.MaxLength = 65535;
            tbLog.Multiline = true;
            tbLog.Name = "tbLog";
            tbLog.ReadOnly = true;
            tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            tbLog.Size = new System.Drawing.Size(920, 362);
            tbLog.TabIndex = 9;
            tbLog.TabStop = false;
            tbLog.WordWrap = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ClientSize = new System.Drawing.Size(944, 502);
            Controls.Add(tbLog);
            Controls.Add(lbLog);
            Controls.Add(btnSelectDirectory);
            Controls.Add(tbGameDirectory);
            Controls.Add(lbGameDirectory);
            Controls.Add(lbEngineTips);
            Controls.Add(tbEngineDescription);
            Controls.Add(btnExtract);
            Controls.Add(cbTitles);
            Controls.Add(lbSelectGames);
            DoubleBuffered = true;
            ImeMode = System.Windows.Forms.ImeMode.Disable;
            MinimumSize = new System.Drawing.Size(800, 450);
            Name = "MainForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "GCExtractor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox cbTitles;
        private System.Windows.Forms.TextBox tbEngineDescription;
        private System.Windows.Forms.TextBox tbGameDirectory;
        private System.Windows.Forms.TextBox tbLog;
    }
}