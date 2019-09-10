namespace WindowsFormsApp1
{
    partial class FormBorderStyles
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
            this.btnChangeFormBorderStyle = new System.Windows.Forms.Button();
            this.lblFormBorderStyle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnChangeFormBorderStyle
            // 
            this.btnChangeFormBorderStyle.Location = new System.Drawing.Point(13, 13);
            this.btnChangeFormBorderStyle.Name = "btnChangeFormBorderStyle";
            this.btnChangeFormBorderStyle.Size = new System.Drawing.Size(162, 23);
            this.btnChangeFormBorderStyle.TabIndex = 0;
            this.btnChangeFormBorderStyle.Text = "Change FormBorderStyle";
            this.btnChangeFormBorderStyle.UseVisualStyleBackColor = true;
            this.btnChangeFormBorderStyle.Click += new System.EventHandler(this.btnChangeFormBorderStyle_Click);
            // 
            // lblFormBorderStyle
            // 
            this.lblFormBorderStyle.AutoSize = true;
            this.lblFormBorderStyle.Location = new System.Drawing.Point(13, 43);
            this.lblFormBorderStyle.Name = "lblFormBorderStyle";
            this.lblFormBorderStyle.Size = new System.Drawing.Size(0, 13);
            this.lblFormBorderStyle.TabIndex = 1;
            // 
            // FormBorderStyles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 214);
            this.Controls.Add(this.lblFormBorderStyle);
            this.Controls.Add(this.btnChangeFormBorderStyle);
            this.DoubleBuffered = true;
            this.Name = "FormBorderStyles";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "FormBorderStyles";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnChangeFormBorderStyle;
        private System.Windows.Forms.Label lblFormBorderStyle;
    }
}