namespace Accessibility_Core_App;

partial class DialogControls
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
        this.ColorDialog = new System.Windows.Forms.Button();
        this.FolderBrowserDialog = new System.Windows.Forms.Button();
        this.OpenFileDialog = new System.Windows.Forms.Button();
        this.SaveFileDialog = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // ColorDialog
        // 
        this.ColorDialog.Location = new System.Drawing.Point(42, 12);
        this.ColorDialog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.ColorDialog.Name = "ColorDialog";
        this.ColorDialog.Size = new System.Drawing.Size(88, 27);
        this.ColorDialog.TabIndex = 0;
        this.ColorDialog.Text = "ColorDialog";
        this.ColorDialog.UseVisualStyleBackColor = true;
        this.ColorDialog.Click += this.ColorDialog_Click;
        // 
        // FolderBrowserDialog
        // 
        this.FolderBrowserDialog.Location = new System.Drawing.Point(42, 67);
        this.FolderBrowserDialog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.FolderBrowserDialog.Name = "FolderBrowserDialog";
        this.FolderBrowserDialog.Size = new System.Drawing.Size(153, 27);
        this.FolderBrowserDialog.TabIndex = 1;
        this.FolderBrowserDialog.Text = "FolderBrowserDialog";
        this.FolderBrowserDialog.UseVisualStyleBackColor = true;
        this.FolderBrowserDialog.Click += this.FolderBrowserDialog_Click;
        // 
        // OpenFileDialog
        // 
        this.OpenFileDialog.Location = new System.Drawing.Point(42, 125);
        this.OpenFileDialog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.OpenFileDialog.Name = "OpenFileDialog";
        this.OpenFileDialog.Size = new System.Drawing.Size(108, 27);
        this.OpenFileDialog.TabIndex = 2;
        this.OpenFileDialog.Text = "OpenFileDialog";
        this.OpenFileDialog.UseVisualStyleBackColor = true;
        this.OpenFileDialog.Click += this.OpenFileDialog_Click;
        // 
        // SaveFileDialog
        // 
        this.SaveFileDialog.Location = new System.Drawing.Point(42, 181);
        this.SaveFileDialog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.SaveFileDialog.Name = "SaveFileDialog";
        this.SaveFileDialog.Size = new System.Drawing.Size(114, 27);
        this.SaveFileDialog.TabIndex = 3;
        this.SaveFileDialog.Text = "SaveFileDialog";
        this.SaveFileDialog.UseVisualStyleBackColor = true;
        this.SaveFileDialog.Click += this.SaveFileDialog_Click;
        // 
        // DialogControls
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(268, 233);
        this.Controls.Add(this.SaveFileDialog);
        this.Controls.Add(this.OpenFileDialog);
        this.Controls.Add(this.FolderBrowserDialog);
        this.Controls.Add(this.ColorDialog);
        this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.Name = "DialogControls";
        this.Text = "DialogsTesting";
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button ColorDialog;
    private System.Windows.Forms.Button FolderBrowserDialog;
    private System.Windows.Forms.Button OpenFileDialog;
    private System.Windows.Forms.Button SaveFileDialog;
}
