namespace WinFormsControlsTest;

partial class ToolTipTests
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
        this.components = new System.ComponentModel.Container();
        this.delaysNotSetButton = new System.Windows.Forms.Button();
        this.automaticDelayButton = new System.Windows.Forms.Button();
        this.autoPopDelayButton = new System.Windows.Forms.Button();
        this.delaysNotSetToolTip = new System.Windows.Forms.ToolTip(this.components);
        this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
        this.defaultAutomaticDelayButton = new System.Windows.Forms.Button();
        this.defaultAutoPopDelayButton = new System.Windows.Forms.Button();
        this.initialDelayButton = new System.Windows.Forms.Button();
        this.automaticDelayToolTip = new System.Windows.Forms.ToolTip(this.components);
        this.autoPopDelayToolTip = new System.Windows.Forms.ToolTip(this.components);
        this.defaultAutoPopDelayToolTip = new System.Windows.Forms.ToolTip(this.components);
        this.defaultAutomaticDelayToolTip = new System.Windows.Forms.ToolTip(this.components);
        this.initialDelayToolTip = new System.Windows.Forms.ToolTip(this.components);
        this.autoEllipsisButton = new System.Windows.Forms.Button();
        this.flowLayoutPanel1.SuspendLayout();
        this.SuspendLayout();
        // 
        // delaysNoSetButton
        // 
        this.delaysNotSetButton.AutoSize = true;
        this.delaysNotSetButton.Location = new System.Drawing.Point(8, 411);
        this.delaysNotSetButton.Name = "delaysNoSetButton";
        this.delaysNotSetButton.Size = new System.Drawing.Size(876, 183);
        this.delaysNotSetButton.TabIndex = 2;
        this.delaysNotSetToolTip.SetToolTip(this.delaysNotSetButton, "Persistent");
        this.delaysNotSetButton.Text = "Delays &not set";
        this.delaysNotSetButton.UseVisualStyleBackColor = true;
        // 
        // automaticDelayButton
        // 
        this.automaticDelayButton.AutoSize = true;
        this.automaticDelayButton.Location = new System.Drawing.Point(8, 9);
        this.automaticDelayButton.Name = "automaticDelayButton";
        this.automaticDelayButton.Size = new System.Drawing.Size(1031, 183);
        this.automaticDelayButton.TabIndex = 0;
        this.automaticDelayToolTip.SetToolTip(this.automaticDelayButton, "Not persistent");
        this.automaticDelayButton.Text = "&AutomaticDelay = 300";
        this.automaticDelayButton.UseVisualStyleBackColor = true;
        // 
        // autoPopDelayButton
        // 
        this.autoPopDelayButton.AutoSize = true;
        this.autoPopDelayButton.Location = new System.Drawing.Point(8, 210);
        this.autoPopDelayButton.Name = "autoPopDelayButton";
        this.autoPopDelayButton.Size = new System.Drawing.Size(949, 183);
        this.autoPopDelayButton.TabIndex = 1;
        this.autoPopDelayToolTip.SetToolTip(this.autoPopDelayButton, "Not persistent");
        this.autoPopDelayButton.Text = "Auto&PopDelay = 6000";
        this.autoPopDelayButton.UseVisualStyleBackColor = true;
        // 
        // flowLayoutPanel1
        // 
        this.flowLayoutPanel1.Controls.Add(this.automaticDelayButton);
        this.flowLayoutPanel1.Controls.Add(this.autoPopDelayButton);
        this.flowLayoutPanel1.Controls.Add(this.delaysNotSetButton);
        this.flowLayoutPanel1.Controls.Add(this.defaultAutomaticDelayButton);
        this.flowLayoutPanel1.Controls.Add(this.defaultAutoPopDelayButton);
        this.flowLayoutPanel1.Controls.Add(this.initialDelayButton);
        this.flowLayoutPanel1.Controls.Add(this.autoEllipsisButton);
        this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        this.flowLayoutPanel1.Name = "flowLayoutPanel1";
        this.flowLayoutPanel1.Size = new System.Drawing.Size(1417, 1577);
        this.flowLayoutPanel1.TabIndex = 0;
        // 
        // defaultAutomaticDelayButton
        // 
        this.defaultAutomaticDelayButton.AutoSize = true;
        this.defaultAutomaticDelayButton.Location = new System.Drawing.Point(8, 612);
        this.defaultAutomaticDelayButton.Name = "defaultAutomaticDelayButton";
        this.defaultAutomaticDelayButton.Size = new System.Drawing.Size(915, 183);
        this.defaultAutomaticDelayButton.TabIndex = 3;
        this.defaultAutomaticDelayToolTip.SetToolTip(this.defaultAutomaticDelayButton, "Persistent");
        this.defaultAutomaticDelayButton.Text = "AutomaticDelay = 500";
        this.defaultAutomaticDelayButton.UseVisualStyleBackColor = true;
        // 
        // defaultAutoPopDelayButton
        // 
        this.defaultAutoPopDelayButton.AutoSize = true;
        this.defaultAutoPopDelayButton.Location = new System.Drawing.Point(8, 813);
        this.defaultAutoPopDelayButton.Name = "defaultAutoPopDelayButton";
        this.defaultAutoPopDelayButton.Size = new System.Drawing.Size(904, 183);
        this.defaultAutoPopDelayButton.TabIndex = 4;
        this.defaultAutoPopDelayToolTip.SetToolTip(this.defaultAutoPopDelayButton, "Persistent");
        this.defaultAutoPopDelayButton.Text = "AutoPopDelay = 5000";
        this.defaultAutoPopDelayButton.UseVisualStyleBackColor = true;
        // 
        // initialDelayButton
        // 
        this.initialDelayButton.AutoSize = true;
        this.initialDelayButton.Location = new System.Drawing.Point(8, 1014);
        this.initialDelayButton.Name = "initialDelayButton";
        this.initialDelayButton.Size = new System.Drawing.Size(876, 183);
        this.initialDelayButton.TabIndex = 5;
        this.initialDelayToolTip.SetToolTip(this.initialDelayButton, "Persistent");
        this.initialDelayButton.Text = "I&nitial delay = 10";
        this.initialDelayButton.UseVisualStyleBackColor = true;
        // 
        // autoEllipsisButton
        // 
        this.autoEllipsisButton.AutoEllipsis = true;
        this.autoEllipsisButton.AutoSize = false;
        this.autoEllipsisButton.Location = new System.Drawing.Point(8, 1215);
        this.autoEllipsisButton.Name = "autoEllipsisButton";
        this.autoEllipsisButton.Size = new System.Drawing.Size(876, 183);
        this.autoEllipsisButton.TabIndex = 6;
        this.autoEllipsisButton.Text = "Auto&Ellipsis = true; 1234567890";
        this.autoEllipsisButton.UseVisualStyleBackColor = true;
        // 
        // automaticDelayToolTip
        // 
        this.automaticDelayToolTip.AutomaticDelay = 300;
        // 
        // autoPopDelayToolTip
        // 
        this.autoPopDelayToolTip.AutoPopDelay = 6000;
        this.autoPopDelayToolTip.InitialDelay = 500;
        this.autoPopDelayToolTip.ReshowDelay = 100;
        // 
        // initialDelayToolTip
        // 
        this.initialDelayToolTip.AutoPopDelay = 5000;
        this.initialDelayToolTip.InitialDelay = 10;
        this.initialDelayToolTip.ReshowDelay = 100;
        // 
        // ToolTipTests
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(17F, 41F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1417, 1577);
        this.Controls.Add(this.flowLayoutPanel1);
        this.Name = "ToolTipTests";
        this.Text = "ToolTips";
        this.flowLayoutPanel1.ResumeLayout(false);
        this.flowLayoutPanel1.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button delaysNotSetButton;
    private System.Windows.Forms.Button automaticDelayButton;
    private System.Windows.Forms.Button autoPopDelayButton;
    private System.Windows.Forms.Button autoEllipsisButton;
    private System.Windows.Forms.Button defaultAutomaticDelayButton;
    private System.Windows.Forms.Button defaultAutoPopDelayButton;
    private System.Windows.Forms.Button initialDelayButton;
    private System.Windows.Forms.ToolTip automaticDelayToolTip;
    private System.Windows.Forms.ToolTip autoPopDelayToolTip;
    private System.Windows.Forms.ToolTip defaultAutomaticDelayToolTip;
    private System.Windows.Forms.ToolTip defaultAutoPopDelayToolTip;
    private System.Windows.Forms.ToolTip delaysNotSetToolTip;
    private System.Windows.Forms.ToolTip initialDelayToolTip;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
}

