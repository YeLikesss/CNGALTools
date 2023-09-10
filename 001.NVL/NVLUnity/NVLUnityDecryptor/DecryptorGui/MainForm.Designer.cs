
namespace DecryptorGui
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
            this.label5 = new System.Windows.Forms.Label();
            this.listBoxFilePath = new System.Windows.Forms.ListBox();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cbGameTitle = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(210, 21);
            this.label5.TabIndex = 7;
            this.label5.Text = "请拖拽nvldata文件到此下方";
            // 
            // listBoxFilePath
            // 
            this.listBoxFilePath.AllowDrop = true;
            this.listBoxFilePath.FormattingEnabled = true;
            this.listBoxFilePath.HorizontalScrollbar = true;
            this.listBoxFilePath.ItemHeight = 21;
            this.listBoxFilePath.Location = new System.Drawing.Point(12, 78);
            this.listBoxFilePath.Name = "listBoxFilePath";
            this.listBoxFilePath.ScrollAlwaysVisible = true;
            this.listBoxFilePath.Size = new System.Drawing.Size(845, 172);
            this.listBoxFilePath.TabIndex = 8;
            this.listBoxFilePath.TabStop = false;
            this.listBoxFilePath.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxFilePath_DragDrop);
            this.listBoxFilePath.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBoxFilePath_DragEnter);
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.Location = new System.Drawing.Point(664, 11);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(193, 35);
            this.btnDecrypt.TabIndex = 10;
            this.btnDecrypt.Text = "一键解密";
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 21);
            this.label6.TabIndex = 11;
            this.label6.Text = "请选择游戏";
            // 
            // cbGameTitle
            // 
            this.cbGameTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGameTitle.FormattingEnabled = true;
            this.cbGameTitle.Location = new System.Drawing.Point(108, 11);
            this.cbGameTitle.Name = "cbGameTitle";
            this.cbGameTitle.Size = new System.Drawing.Size(257, 29);
            this.cbGameTitle.TabIndex = 12;
            this.cbGameTitle.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 253);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 21);
            this.label1.TabIndex = 13;
            this.label1.Text = "日志Log";
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(12, 277);
            this.tbLog.MaxLength = 65536;
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLog.ShortcutsEnabled = false;
            this.tbLog.Size = new System.Drawing.Size(845, 161);
            this.tbLog.TabIndex = 14;
            this.tbLog.TabStop = false;
            this.tbLog.WordWrap = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(877, 450);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbGameTitle);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnDecrypt);
            this.Controls.Add(this.listBoxFilePath);
            this.Controls.Add(this.label5);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "NVL Unity Decryptor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox listBoxFilePath;
        private System.Windows.Forms.Button btnDecrypt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbGameTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbLog;
    }
}

