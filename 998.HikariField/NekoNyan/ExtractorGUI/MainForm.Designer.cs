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
            this.label1 = new System.Windows.Forms.Label();
            this.labelCryptoVer = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbGameTitle = new System.Windows.Forms.ComboBox();
            this.lbFilePath = new System.Windows.Forms.ListBox();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "游戏名称";
            // 
            // labelCryptoVer
            // 
            this.labelCryptoVer.Location = new System.Drawing.Point(727, 11);
            this.labelCryptoVer.Name = "labelCryptoVer";
            this.labelCryptoVer.Size = new System.Drawing.Size(148, 27);
            this.labelCryptoVer.TabIndex = 1;
            this.labelCryptoVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(186, 19);
            this.label3.TabIndex = 2;
            this.label3.Text = "封包文件(拖拽封包到此列表框)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 188);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "日志";
            // 
            // cbGameTitle
            // 
            this.cbGameTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGameTitle.FormattingEnabled = true;
            this.cbGameTitle.Location = new System.Drawing.Point(79, 11);
            this.cbGameTitle.Name = "cbGameTitle";
            this.cbGameTitle.Size = new System.Drawing.Size(595, 27);
            this.cbGameTitle.TabIndex = 4;
            this.cbGameTitle.SelectedIndexChanged += new System.EventHandler(this.cbGameTitle_SelectedIndexChanged);
            // 
            // lbFilePath
            // 
            this.lbFilePath.AllowDrop = true;
            this.lbFilePath.FormattingEnabled = true;
            this.lbFilePath.HorizontalScrollbar = true;
            this.lbFilePath.ItemHeight = 19;
            this.lbFilePath.Location = new System.Drawing.Point(12, 86);
            this.lbFilePath.Name = "lbFilePath";
            this.lbFilePath.Size = new System.Drawing.Size(863, 99);
            this.lbFilePath.TabIndex = 5;
            this.lbFilePath.DragDrop += new System.Windows.Forms.DragEventHandler(this.lbFilePath_DragDrop);
            this.lbFilePath.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileDragEnter);
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(12, 210);
            this.tbLog.MaxLength = 65536;
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLog.ShortcutsEnabled = false;
            this.tbLog.Size = new System.Drawing.Size(863, 210);
            this.tbLog.TabIndex = 6;
            this.tbLog.TabStop = false;
            this.tbLog.WordWrap = false;
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(727, 48);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(148, 29);
            this.btnExtract.TabIndex = 7;
            this.btnExtract.Text = "解包";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(887, 435);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.lbFilePath);
            this.Controls.Add(this.cbGameTitle);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelCryptoVer);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "NekoNyanExtractor - Static Mode";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Label labelCryptoVer;
        private Label label3;
        private Label label4;
        private ComboBox cbGameTitle;
        private ListBox lbFilePath;
        private TextBox tbLog;
        private Button btnExtract;
    }
}