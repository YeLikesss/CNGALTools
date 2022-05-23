
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
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.cmdExtract = new System.Windows.Forms.Button();
            this.listBoxFile = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "BKARC.V20(官方工具)",
            "BKARC.V21(十二色的季节)",
            "BKARC.V40(内部收费定制 2018-?)"});
            this.cmbType.Location = new System.Drawing.Point(12, 12);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(319, 29);
            this.cmbType.TabIndex = 0;
            this.cmbType.TabStop = false;
            // 
            // cmdExtract
            // 
            this.cmdExtract.Location = new System.Drawing.Point(534, 4);
            this.cmdExtract.Name = "cmdExtract";
            this.cmdExtract.Size = new System.Drawing.Size(116, 37);
            this.cmdExtract.TabIndex = 1;
            this.cmdExtract.Text = "解包";
            this.cmdExtract.UseVisualStyleBackColor = true;
            this.cmdExtract.Click += new System.EventHandler(this.cmdExtract_Click);
            // 
            // listBoxFile
            // 
            this.listBoxFile.AllowDrop = true;
            this.listBoxFile.FormattingEnabled = true;
            this.listBoxFile.HorizontalScrollbar = true;
            this.listBoxFile.ItemHeight = 21;
            this.listBoxFile.Location = new System.Drawing.Point(12, 47);
            this.listBoxFile.MultiColumn = true;
            this.listBoxFile.Name = "listBoxFile";
            this.listBoxFile.Size = new System.Drawing.Size(638, 151);
            this.listBoxFile.TabIndex = 2;
            this.listBoxFile.TabStop = false;
            this.listBoxFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileList_DragDrop);
            this.listBoxFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileList_DragEnter);
            this.listBoxFile.DragOver += new System.Windows.Forms.DragEventHandler(this.FileList_DragOver);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(662, 207);
            this.Controls.Add(this.listBoxFile);
            this.Controls.Add(this.cmdExtract);
            this.Controls.Add(this.cmbType);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "bkarc Extract By YeLike";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Button cmdExtract;
        private System.Windows.Forms.ListBox listBoxFile;
    }
}

