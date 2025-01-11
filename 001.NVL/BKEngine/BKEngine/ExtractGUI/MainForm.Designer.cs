
namespace ExtractGUI
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            cmbType = new System.Windows.Forms.ComboBox();
            cmdExtract = new System.Windows.Forms.Button();
            listBoxFile = new System.Windows.Forms.ListBox();
            SuspendLayout();
            // 
            // cmbType
            // 
            cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbType.FormattingEnabled = true;
            cmbType.Items.AddRange(new object[] { "BKARC.V20", "BKARC.V21", "BKARC.V40" });
            cmbType.Location = new System.Drawing.Point(12, 12);
            cmbType.Name = "cmbType";
            cmbType.Size = new System.Drawing.Size(319, 29);
            cmbType.TabIndex = 0;
            cmbType.TabStop = false;
            // 
            // cmdExtract
            // 
            cmdExtract.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cmdExtract.Location = new System.Drawing.Point(534, 4);
            cmdExtract.Name = "cmdExtract";
            cmdExtract.Size = new System.Drawing.Size(116, 37);
            cmdExtract.TabIndex = 1;
            cmdExtract.Text = "解包";
            cmdExtract.UseVisualStyleBackColor = true;
            cmdExtract.Click += cmdExtract_Click;
            // 
            // listBoxFile
            // 
            listBoxFile.AllowDrop = true;
            listBoxFile.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            listBoxFile.HorizontalScrollbar = true;
            listBoxFile.IntegralHeight = false;
            listBoxFile.ItemHeight = 21;
            listBoxFile.Location = new System.Drawing.Point(12, 47);
            listBoxFile.Name = "listBoxFile";
            listBoxFile.Size = new System.Drawing.Size(638, 151);
            listBoxFile.TabIndex = 2;
            listBoxFile.TabStop = false;
            listBoxFile.DragDrop += FileList_DragDrop;
            listBoxFile.DragEnter += FileList_DragEnter;
            listBoxFile.DragOver += FileList_DragOver;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(662, 207);
            Controls.Add(listBoxFile);
            Controls.Add(cmdExtract);
            Controls.Add(cmbType);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ImeMode = System.Windows.Forms.ImeMode.Disable;
            Margin = new System.Windows.Forms.Padding(5);
            MinimumSize = new System.Drawing.Size(600, 240);
            Name = "MainForm";
            Text = "BKARC Extractor";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Button cmdExtract;
        private System.Windows.Forms.ListBox listBoxFile;
    }
}

