
namespace Extractor.GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Button btnExtract;
            cbTitle = new System.Windows.Forms.ComboBox();
            btnExtract = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // btnExtract
            // 
            btnExtract.Location = new System.Drawing.Point(283, 80);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new System.Drawing.Size(148, 43);
            btnExtract.TabIndex = 0;
            btnExtract.Text = "解包";
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += btnExtract_Click;
            // 
            // cbTitle
            // 
            cbTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTitle.Location = new System.Drawing.Point(12, 20);
            cbTitle.Name = "cbTitle";
            cbTitle.Size = new System.Drawing.Size(711, 29);
            cbTitle.TabIndex = 1;
            cbTitle.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(735, 147);
            Controls.Add(cbTitle);
            Controls.Add(btnExtract);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ImeMode = System.Windows.Forms.ImeMode.Disable;
            Margin = new System.Windows.Forms.Padding(5);
            MaximizeBox = false;
            Name = "MainForm";
            Text = "AsicxArtExtractor";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ComboBox cbTitle;
    }
}