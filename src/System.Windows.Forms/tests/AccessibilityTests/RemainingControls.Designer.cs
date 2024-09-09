namespace Accessibility_Core_App;

partial class RemainingControls
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
        this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
        this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
        this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
        this.propertyGrid2 = new System.Windows.Forms.PropertyGrid();
        this.label1 = new System.Windows.Forms.Label();
        this.domainUpDown1 = new System.Windows.Forms.DomainUpDown();
        this.label2 = new System.Windows.Forms.Label();
        this.trackBar1 = new System.Windows.Forms.TrackBar();
        ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
        this.SuspendLayout();
        // 
        // hScrollBar1
        // 
        this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.hScrollBar1.Location = new System.Drawing.Point(0, 406);
        this.hScrollBar1.Name = "hScrollBar1";
        this.hScrollBar1.Size = new System.Drawing.Size(593, 17);
        this.hScrollBar1.TabIndex = 7;
        this.hScrollBar1.TabStop = true;
        // 
        // vScrollBar1
        // 
        this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
        this.vScrollBar1.Location = new System.Drawing.Point(576, 0);
        this.vScrollBar1.Name = "vScrollBar1";
        this.vScrollBar1.Size = new System.Drawing.Size(17, 406);
        this.vScrollBar1.TabIndex = 4;
        this.vScrollBar1.TabStop = true;
        // 
        // propertyGrid1
        // 
        this.propertyGrid1.Location = new System.Drawing.Point(19, 80);
        this.propertyGrid1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.propertyGrid1.Name = "propertyGrid1";
        this.propertyGrid1.Size = new System.Drawing.Size(266, 282);
        this.propertyGrid1.TabIndex = 5;
        // 
        // propertyGrid2
        // 
        this.propertyGrid2.Location = new System.Drawing.Point(292, 80);
        this.propertyGrid2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.propertyGrid2.Name = "propertyGrid2";
        this.propertyGrid2.Size = new System.Drawing.Size(260, 282);
        this.propertyGrid2.TabIndex = 6;
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(19, 33);
        this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(156, 15);
        this.label1.TabIndex = 0;
        this.label1.Text = "Setting for DomainUpDown:";
        // 
        // domainUpDown1
        // 
        this.domainUpDown1.Location = new System.Drawing.Point(192, 31);
        this.domainUpDown1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.domainUpDown1.Name = "domainUpDown1";
        this.domainUpDown1.Size = new System.Drawing.Size(140, 23);
        this.domainUpDown1.TabIndex = 1;
        this.domainUpDown1.Text = "domainUpDown1";
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(373, 33);
        this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(37, 15);
        this.label2.TabIndex = 2;
        this.label2.Text = "Track:";
        // 
        // trackBar1
        // 
        this.trackBar1.Location = new System.Drawing.Point(431, 29);
        this.trackBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.trackBar1.Name = "trackBar1";
        this.trackBar1.Size = new System.Drawing.Size(121, 45);
        this.trackBar1.TabIndex = 3;
        // 
        // RemainingControls
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(593, 423);
        this.Controls.Add(this.trackBar1);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.domainUpDown1);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.propertyGrid2);
        this.Controls.Add(this.propertyGrid1);
        this.Controls.Add(this.vScrollBar1);
        this.Controls.Add(this.hScrollBar1);
        this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.Name = "RemainingControls";
        this.Text = "RemainingControls";
        this.Load += this.Form1_Load;
        ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.HScrollBar hScrollBar1;
    private System.Windows.Forms.VScrollBar vScrollBar1;
    private System.Windows.Forms.PropertyGrid propertyGrid1;
    private System.Windows.Forms.PropertyGrid propertyGrid2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.DomainUpDown domainUpDown1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TrackBar trackBar1;
}
