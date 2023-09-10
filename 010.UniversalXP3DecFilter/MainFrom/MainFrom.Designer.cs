using System;
using System.Windows.Forms;

namespace MainFrom
{
    partial class MainFrom
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
            Label label1;
            Button btnExtract;
            Label label4;
            cbTitles = new ComboBox();
            listBoxFiles = new ListBox();
            label1 = new Label();
            btnExtract = new Button();
            label4 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 12);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(51, 21);
            label1.TabIndex = 13;
            label1.Text = "游戏 :";
            // 
            // btnExtract
            // 
            btnExtract.Location = new System.Drawing.Point(637, 12);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new System.Drawing.Size(137, 40);
            btnExtract.TabIndex = 12;
            btnExtract.Text = "解包";
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += btnExtract_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 40);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(167, 21);
            label4.TabIndex = 10;
            label4.Text = "请拖拽XP3文件到下方";
            // 
            // cbTitles
            // 
            cbTitles.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTitles.FormattingEnabled = true;
            cbTitles.Location = new System.Drawing.Point(69, 9);
            cbTitles.Name = "cbTitles";
            cbTitles.Size = new System.Drawing.Size(319, 29);
            cbTitles.TabIndex = 14;
            // 
            // listBoxFiles
            // 
            listBoxFiles.AllowDrop = true;
            listBoxFiles.FormattingEnabled = true;
            listBoxFiles.HorizontalScrollbar = true;
            listBoxFiles.ItemHeight = 21;
            listBoxFiles.Location = new System.Drawing.Point(12, 64);
            listBoxFiles.Name = "listBoxFiles";
            listBoxFiles.ScrollAlwaysVisible = true;
            listBoxFiles.Size = new System.Drawing.Size(762, 235);
            listBoxFiles.TabIndex = 11;
            listBoxFiles.TabStop = false;
            listBoxFiles.DragDrop += listBoxFiles_DragDrop;
            listBoxFiles.DragEnter += File_DragEnter;
            // 
            // MainFrom
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(783, 308);
            Controls.Add(cbTitles);
            Controls.Add(label1);
            Controls.Add(btnExtract);
            Controls.Add(listBoxFiles);
            Controls.Add(label4);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ImeMode = ImeMode.Disable;
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "MainFrom";
            Text = "XP3DecTPM GUI";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cbTitles;
        private ListBox listBoxFiles;
    }
}