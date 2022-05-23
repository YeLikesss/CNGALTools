
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
            this.exePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbKey1 = new System.Windows.Forms.TextBox();
            this.tbKey2 = new System.Windows.Forms.TextBox();
            this.tbKey3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.listBoxFilePath = new System.Windows.Forms.ListBox();
            this.progressBarDecrypt = new System.Windows.Forms.ProgressBar();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cbUnityVer = new System.Windows.Forms.ComboBox();
            this.exePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // exePanel
            // 
            this.exePanel.AllowDrop = true;
            this.exePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.exePanel.Controls.Add(this.label1);
            this.exePanel.Location = new System.Drawing.Point(12, 12);
            this.exePanel.Name = "exePanel";
            this.exePanel.Size = new System.Drawing.Size(869, 77);
            this.exePanel.TabIndex = 0;
            this.exePanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.exePanel_DragDrop);
            this.exePanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.exePanel_DragEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(315, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(211, 42);
            this.label1.TabIndex = 0;
            this.label1.Text = "请拖拽主程序exe至此处>_<\r\n(确保已脱壳)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "Key1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(310, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "Key2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(605, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 21);
            this.label4.TabIndex = 3;
            this.label4.Text = "Key3";
            // 
            // tbKey1
            // 
            this.tbKey1.Location = new System.Drawing.Point(64, 109);
            this.tbKey1.MaxLength = 8;
            this.tbKey1.Name = "tbKey1";
            this.tbKey1.Size = new System.Drawing.Size(224, 29);
            this.tbKey1.TabIndex = 4;
            // 
            // tbKey2
            // 
            this.tbKey2.Location = new System.Drawing.Point(362, 109);
            this.tbKey2.MaxLength = 8;
            this.tbKey2.Name = "tbKey2";
            this.tbKey2.Size = new System.Drawing.Size(224, 29);
            this.tbKey2.TabIndex = 5;
            // 
            // tbKey3
            // 
            this.tbKey3.Location = new System.Drawing.Point(657, 109);
            this.tbKey3.MaxLength = 8;
            this.tbKey3.Name = "tbKey3";
            this.tbKey3.Size = new System.Drawing.Size(224, 29);
            this.tbKey3.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(671, 150);
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
            this.listBoxFilePath.Location = new System.Drawing.Point(12, 185);
            this.listBoxFilePath.Name = "listBoxFilePath";
            this.listBoxFilePath.ScrollAlwaysVisible = true;
            this.listBoxFilePath.Size = new System.Drawing.Size(869, 298);
            this.listBoxFilePath.TabIndex = 8;
            this.listBoxFilePath.TabStop = false;
            this.listBoxFilePath.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBoxFilePath_DragDrop);
            this.listBoxFilePath.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBoxFilePath_DragEnter);
            // 
            // progressBarDecrypt
            // 
            this.progressBarDecrypt.Location = new System.Drawing.Point(12, 491);
            this.progressBarDecrypt.Name = "progressBarDecrypt";
            this.progressBarDecrypt.Size = new System.Drawing.Size(659, 33);
            this.progressBarDecrypt.Step = 1;
            this.progressBarDecrypt.TabIndex = 9;
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.Location = new System.Drawing.Point(688, 489);
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
            this.label6.Location = new System.Drawing.Point(12, 150);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(162, 21);
            this.label6.TabIndex = 11;
            this.label6.Text = "请选择游戏Unity版本";
            // 
            // cbUnityVer
            // 
            this.cbUnityVer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUnityVer.FormattingEnabled = true;
            this.cbUnityVer.Items.AddRange(new object[] {
            "V2018_4_0_65448",
            "V2018_4_26_44060"});
            this.cbUnityVer.Location = new System.Drawing.Point(190, 147);
            this.cbUnityVer.Name = "cbUnityVer";
            this.cbUnityVer.Size = new System.Drawing.Size(246, 29);
            this.cbUnityVer.TabIndex = 12;
            this.cbUnityVer.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(893, 530);
            this.Controls.Add(this.cbUnityVer);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnDecrypt);
            this.Controls.Add(this.progressBarDecrypt);
            this.Controls.Add(this.listBoxFilePath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbKey3);
            this.Controls.Add(this.tbKey2);
            this.Controls.Add(this.tbKey1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.exePanel);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "NVL Unity Decryptor";
            this.exePanel.ResumeLayout(false);
            this.exePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel exePanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbKey1;
        private System.Windows.Forms.TextBox tbKey2;
        private System.Windows.Forms.TextBox tbKey3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox listBoxFilePath;
        private System.Windows.Forms.ProgressBar progressBarDecrypt;
        private System.Windows.Forms.Button btnDecrypt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbUnityVer;
    }
}

