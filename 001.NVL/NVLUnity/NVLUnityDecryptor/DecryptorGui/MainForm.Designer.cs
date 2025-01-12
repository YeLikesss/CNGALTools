
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
            label5 = new System.Windows.Forms.Label();
            listBoxFilePath = new System.Windows.Forms.ListBox();
            btnDecrypt = new System.Windows.Forms.Button();
            label6 = new System.Windows.Forms.Label();
            cbGameTitle = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            tbLog = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(12, 54);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(210, 21);
            label5.TabIndex = 7;
            label5.Text = "请拖拽nvldata文件到此下方";
            // 
            // listBoxFilePath
            // 
            listBoxFilePath.AllowDrop = true;
            listBoxFilePath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            listBoxFilePath.FormattingEnabled = true;
            listBoxFilePath.HorizontalScrollbar = true;
            listBoxFilePath.IntegralHeight = false;
            listBoxFilePath.ItemHeight = 21;
            listBoxFilePath.Location = new System.Drawing.Point(12, 78);
            listBoxFilePath.Name = "listBoxFilePath";
            listBoxFilePath.ScrollAlwaysVisible = true;
            listBoxFilePath.Size = new System.Drawing.Size(845, 235);
            listBoxFilePath.TabIndex = 8;
            listBoxFilePath.TabStop = false;
            listBoxFilePath.DragDrop += listBoxFilePath_DragDrop;
            listBoxFilePath.DragEnter += listBoxFilePath_DragEnter;
            // 
            // btnDecrypt
            // 
            btnDecrypt.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnDecrypt.Location = new System.Drawing.Point(664, 11);
            btnDecrypt.Name = "btnDecrypt";
            btnDecrypt.Size = new System.Drawing.Size(193, 35);
            btnDecrypt.TabIndex = 10;
            btnDecrypt.Text = "一键解密";
            btnDecrypt.UseVisualStyleBackColor = true;
            btnDecrypt.Click += btnDecrypt_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(12, 14);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(90, 21);
            label6.TabIndex = 11;
            label6.Text = "请选择游戏";
            // 
            // cbGameTitle
            // 
            cbGameTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbGameTitle.FormattingEnabled = true;
            cbGameTitle.Location = new System.Drawing.Point(108, 11);
            cbGameTitle.Name = "cbGameTitle";
            cbGameTitle.Size = new System.Drawing.Size(257, 29);
            cbGameTitle.TabIndex = 12;
            cbGameTitle.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 316);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(70, 21);
            label1.TabIndex = 13;
            label1.Text = "日志Log";
            // 
            // tbLog
            // 
            tbLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbLog.Location = new System.Drawing.Point(12, 340);
            tbLog.MaxLength = 65536;
            tbLog.Multiline = true;
            tbLog.Name = "tbLog";
            tbLog.ReadOnly = true;
            tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            tbLog.ShortcutsEnabled = false;
            tbLog.Size = new System.Drawing.Size(845, 210);
            tbLog.TabIndex = 14;
            tbLog.TabStop = false;
            tbLog.WordWrap = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new System.Drawing.Size(877, 562);
            Controls.Add(tbLog);
            Controls.Add(label1);
            Controls.Add(cbGameTitle);
            Controls.Add(label6);
            Controls.Add(btnDecrypt);
            Controls.Add(listBoxFilePath);
            Controls.Add(label5);
            Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ImeMode = System.Windows.Forms.ImeMode.Disable;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(800, 600);
            Name = "MainForm";
            Text = "NVL Unity Decryptor";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
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

