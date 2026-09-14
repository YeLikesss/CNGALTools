using System;
using System.Windows.Forms;

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
            Label labelTitle;
            Label labelFileList;
            Label labelLog;
            Button btnExtract;
            labelCryptoVer = new Label();
            cbGameTitle = new ComboBox();
            lbFilePath = new ListBox();
            tbLog = new TextBox();
            labelTitle = new Label();
            labelFileList = new Label();
            labelLog = new Label();
            btnExtract = new Button();
            SuspendLayout();
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Location = new System.Drawing.Point(12, 14);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new System.Drawing.Size(61, 19);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "游戏名称";
            // 
            // labelFileList
            // 
            labelFileList.AutoSize = true;
            labelFileList.Location = new System.Drawing.Point(12, 53);
            labelFileList.Name = "labelFileList";
            labelFileList.Size = new System.Drawing.Size(186, 19);
            labelFileList.TabIndex = 2;
            labelFileList.Text = "封包文件(拖拽封包到此列表框)";
            // 
            // labelLog
            // 
            labelLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelLog.AutoSize = true;
            labelLog.Location = new System.Drawing.Point(12, 312);
            labelLog.Name = "labelLog";
            labelLog.Size = new System.Drawing.Size(35, 19);
            labelLog.TabIndex = 3;
            labelLog.Text = "日志";
            // 
            // btnExtract
            // 
            btnExtract.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExtract.Location = new System.Drawing.Point(624, 48);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new System.Drawing.Size(148, 29);
            btnExtract.TabIndex = 7;
            btnExtract.Text = "解包";
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += BtnExtract_OnClick;
            // 
            // labelCryptoVer
            // 
            labelCryptoVer.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelCryptoVer.Location = new System.Drawing.Point(624, 11);
            labelCryptoVer.Name = "labelCryptoVer";
            labelCryptoVer.Size = new System.Drawing.Size(148, 27);
            labelCryptoVer.TabIndex = 1;
            labelCryptoVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbGameTitle
            // 
            cbGameTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbGameTitle.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGameTitle.FormattingEnabled = true;
            cbGameTitle.Location = new System.Drawing.Point(79, 11);
            cbGameTitle.Name = "cbGameTitle";
            cbGameTitle.Size = new System.Drawing.Size(492, 27);
            cbGameTitle.TabIndex = 4;
            cbGameTitle.SelectedIndexChanged += CbGameTitle_OnSelectedIndexChanged;
            // 
            // lbFilePath
            // 
            lbFilePath.AllowDrop = true;
            lbFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbFilePath.FormattingEnabled = true;
            lbFilePath.HorizontalScrollbar = true;
            lbFilePath.IntegralHeight = false;
            lbFilePath.ItemHeight = 19;
            lbFilePath.Location = new System.Drawing.Point(12, 83);
            lbFilePath.Name = "lbFilePath";
            lbFilePath.Size = new System.Drawing.Size(760, 226);
            lbFilePath.TabIndex = 5;
            lbFilePath.DragDrop += LbFilePath_OnDragDrop;
            lbFilePath.DragEnter += FileDragEnter;
            // 
            // tbLog
            // 
            tbLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbLog.Location = new System.Drawing.Point(12, 334);
            tbLog.MaxLength = 65536;
            tbLog.Multiline = true;
            tbLog.Name = "tbLog";
            tbLog.ReadOnly = true;
            tbLog.ScrollBars = ScrollBars.Both;
            tbLog.Size = new System.Drawing.Size(760, 216);
            tbLog.TabIndex = 6;
            tbLog.TabStop = false;
            tbLog.WordWrap = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(784, 562);
            Controls.Add(btnExtract);
            Controls.Add(tbLog);
            Controls.Add(lbFilePath);
            Controls.Add(cbGameTitle);
            Controls.Add(labelLog);
            Controls.Add(labelFileList);
            Controls.Add(labelCryptoVer);
            Controls.Add(labelTitle);
            Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ImeMode = ImeMode.Disable;
            MinimumSize = new System.Drawing.Size(800, 600);
            Name = "MainForm";
            Text = "NekoNyanExtractor - Static Mode";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelCryptoVer;
        private ComboBox cbGameTitle;
        private ListBox lbFilePath;
        private TextBox tbLog;
    }
}