namespace AccessibilityTests
{
    partial class DialogsTesting
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
            this.ColorDialog = new System.Windows.Forms.Button();
            this.FontDialog = new System.Windows.Forms.Button();
            this.FolderBrowserDialog = new System.Windows.Forms.Button();
            this.OpenFileDialog = new System.Windows.Forms.Button();
            this.SaveFileDialog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ColorDialog
            // 
            this.ColorDialog.Location = new System.Drawing.Point(138, 34);
            this.ColorDialog.Name = "ColorDialog";
            this.ColorDialog.Size = new System.Drawing.Size(75, 23);
            this.ColorDialog.TabIndex = 0;
            this.ColorDialog.Text = "ColorDialog";
            this.ColorDialog.UseVisualStyleBackColor = true;
            this.ColorDialog.Click += new System.EventHandler(this.ColorDialog_Click);
            // 
            // FontDialog
            // 
            this.FontDialog.Location = new System.Drawing.Point(345, 34);
            this.FontDialog.Name = "FontDialog";
            this.FontDialog.Size = new System.Drawing.Size(75, 23);
            this.FontDialog.TabIndex = 1;
            this.FontDialog.Text = "FontDialog";
            this.FontDialog.UseVisualStyleBackColor = true;
            this.FontDialog.Click += new System.EventHandler(this.FontDialog_Click);
            // 
            // FolderBrowserDialog
            // 
            this.FolderBrowserDialog.Location = new System.Drawing.Point(36, 108);
            this.FolderBrowserDialog.Name = "FolderBrowserDialog";
            this.FolderBrowserDialog.Size = new System.Drawing.Size(131, 23);
            this.FolderBrowserDialog.TabIndex = 2;
            this.FolderBrowserDialog.Text = "FolderBrowserDialog";
            this.FolderBrowserDialog.UseVisualStyleBackColor = true;
            this.FolderBrowserDialog.Click += new System.EventHandler(this.FolderBrowserDialog_Click);
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.Location = new System.Drawing.Point(247, 108);
            this.OpenFileDialog.Name = "OpenFileDialog";
            this.OpenFileDialog.Size = new System.Drawing.Size(93, 23);
            this.OpenFileDialog.TabIndex = 3;
            this.OpenFileDialog.Text = "OpenFileDialog";
            this.OpenFileDialog.UseVisualStyleBackColor = true;
            this.OpenFileDialog.Click += new System.EventHandler(this.OpenFileDialog_Click);
            // 
            // SaveFileDialog
            // 
            this.SaveFileDialog.Location = new System.Drawing.Point(428, 108);
            this.SaveFileDialog.Name = "SaveFileDialog";
            this.SaveFileDialog.Size = new System.Drawing.Size(98, 23);
            this.SaveFileDialog.TabIndex = 4;
            this.SaveFileDialog.Text = "SaveFileDialog";
            this.SaveFileDialog.UseVisualStyleBackColor = true;
            this.SaveFileDialog.Click += new System.EventHandler(this.SaveFileDialog_Click);
            // 
            // DialogsTesting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 237);
            this.Controls.Add(this.SaveFileDialog);
            this.Controls.Add(this.OpenFileDialog);
            this.Controls.Add(this.FolderBrowserDialog);
            this.Controls.Add(this.FontDialog);
            this.Controls.Add(this.ColorDialog);
            this.Name = "DialogsTesting";
            this.Text = "DialogsTesting";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ColorDialog;
        private System.Windows.Forms.Button FontDialog;
        private System.Windows.Forms.Button FolderBrowserDialog;
        private System.Windows.Forms.Button OpenFileDialog;
        private System.Windows.Forms.Button SaveFileDialog;
    }
}

