// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinformsControlsTest;

internal class ToolStripSeparatorPreferredSize : Form
{
    public ToolStripSeparatorPreferredSize()
    {
        InitializeComponent();
        this.buttonChangeOrientation.Click += ChangeOrientation;
        SetLabelText();
    }

    private void ChangeOrientation(object sender, EventArgs e)
    {
        toolStrip1.LayoutStyle = toolStrip1.LayoutStyle == ToolStripLayoutStyle.VerticalStackWithOverflow
            ? ToolStripLayoutStyle.HorizontalStackWithOverflow
            : ToolStripLayoutStyle.VerticalStackWithOverflow;
        SetLabelText();
    }

    private void SetLabelText()
    {
        label1.Text = $"Layout: {toolStrip1.LayoutStyle}, Size: {toolStripSeparator1.Size}, GetPreferredSize: {toolStripSeparator1.GetPreferredSize(toolStripSeparator1.Size)}";
    }

    private void InitializeComponent()
    {
        this.toolStrip1 = new System.Windows.Forms.ToolStrip();
        this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
        this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
        this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        this.buttonChangeOrientation = new System.Windows.Forms.Button();
        this.label1 = new System.Windows.Forms.Label();
        this.toolStrip1.SuspendLayout();
        this.SuspendLayout();
        // 
        // toolStrip1
        // 
        this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            this.toolStripLabel1,
            this.toolStripSeparator1,
            this.toolStripLabel2
        });
        this.toolStrip1.Location = new System.Drawing.Point(0, 0);
        this.toolStrip1.Name = "toolStrip1";
        this.toolStrip1.Size = new System.Drawing.Size(481, 22);
        this.toolStrip1.TabIndex = 0;
        this.toolStrip1.Text = "toolStrip1";
        this.toolStrip1.TabStop = true;
        // 
        // toolStripLabel1
        // 
        this.toolStripLabel1.Name = "toolStripLabel1";
        this.toolStripLabel1.Size = new System.Drawing.Size(86, 22);
        this.toolStripLabel1.Text = "ToolStripLabel";
        // 
        // toolStripLabel2
        // 
        this.toolStripLabel2.Name = "toolStripLabel2";
        this.toolStripLabel2.Size = new System.Drawing.Size(86, 22);
        this.toolStripLabel2.Text = "ToolStripLabel";
        // 
        // buttonChangeOrientation
        // 
        this.buttonChangeOrientation.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        this.buttonChangeOrientation.Name = "buttonChangeOrientation";
        this.buttonChangeOrientation.Location = new System.Drawing.Point(10, 75);
        this.buttonChangeOrientation.Size = new System.Drawing.Size(150, 25);
        this.buttonChangeOrientation.TabIndex = 0;
        this.buttonChangeOrientation.Text = "Change Orientation";
        this.buttonChangeOrientation.UseVisualStyleBackColor = true;
        // 
        // label1
        //
        this.label1.Name = "label1";
        this.label1.Location = new System.Drawing.Point(10, 50);
        this.label1.AutoSize = true;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Size = new System.Drawing.Size(881, 150);
        this.Controls.Add(this.toolStrip1);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.buttonChangeOrientation);
        this.Name = "ToolStripSeparatorPreferredSize";
        this.Text = "ToolStripSeparator GetPreferredSize";
        this.toolStrip1.ResumeLayout(false);
        this.toolStrip1.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    private System.Windows.Forms.ToolStripLabel toolStripLabel2;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonChangeOrientation;
}
