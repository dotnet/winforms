namespace Accessibility_Core_App;

partial class MenuForm
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
        this.button1 = new System.Windows.Forms.Button();
        this.button2 = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // button1
        // 
        this.button1.Location = new System.Drawing.Point(78, 46);
        this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(88, 27);
        this.button1.TabIndex = 0;
        this.button1.Text = "StripControls";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += this.Button1_Click;
        // 
        // button2
        // 
        this.button2.Location = new System.Drawing.Point(264, 46);
        this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button2.Name = "button2";
        this.button2.Size = new System.Drawing.Size(122, 27);
        this.button2.TabIndex = 1;
        this.button2.Text = "ToolStripContainer";
        this.button2.UseVisualStyleBackColor = true;
        this.button2.Click += this.Button2_Click;
        // 
        // MenuForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(467, 441);
        this.Controls.Add(this.button2);
        this.Controls.Add(this.button1);
        this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.Name = "MenuForm";
        this.Text = "MenuForm";
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
}
