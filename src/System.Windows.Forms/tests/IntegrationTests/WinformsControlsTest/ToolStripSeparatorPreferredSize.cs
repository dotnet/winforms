// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinformsControlsTest;

internal class ToolStripSeparatorPreferredSize : Form
{
    public ToolStripSeparatorPreferredSize()
    {
        InitializeComponent();
        buttonChangeOrientation.Click += ChangeOrientation;
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
        toolStrip1 = new System.Windows.Forms.ToolStrip();
        toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
        toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
        toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        buttonChangeOrientation = new System.Windows.Forms.Button();
        label1 = new System.Windows.Forms.Label();
        toolStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // toolStrip1
        // 
        toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            toolStripLabel1,
            toolStripSeparator1,
            toolStripLabel2
        });
        toolStrip1.Location = new System.Drawing.Point(0, 0);
        toolStrip1.Name = "toolStrip1";
        toolStrip1.Size = new System.Drawing.Size(481, 22);
        toolStrip1.TabIndex = 0;
        toolStrip1.Text = "toolStrip1";
        toolStrip1.TabStop = true;
        // 
        // toolStripLabel1
        // 
        toolStripLabel1.Name = "toolStripLabel1";
        toolStripLabel1.Size = new System.Drawing.Size(86, 22);
        toolStripLabel1.Text = "ToolStripLabel";
        // 
        // toolStripLabel2
        // 
        toolStripLabel2.Name = "toolStripLabel2";
        toolStripLabel2.Size = new System.Drawing.Size(86, 22);
        toolStripLabel2.Text = "ToolStripLabel";
        // 
        // buttonChangeOrientation
        // 
        buttonChangeOrientation.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        buttonChangeOrientation.Name = "buttonChangeOrientation";
        buttonChangeOrientation.Location = new System.Drawing.Point(10, 75);
        buttonChangeOrientation.Size = new System.Drawing.Size(150, 25);
        buttonChangeOrientation.TabIndex = 0;
        buttonChangeOrientation.Text = "Change Orientation";
        buttonChangeOrientation.UseVisualStyleBackColor = true;
        // 
        // label1
        //
        label1.Name = "label1";
        label1.Location = new System.Drawing.Point(10, 50);
        label1.AutoSize = true;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        Size = new System.Drawing.Size(881, 150);
        Controls.Add(toolStrip1);
        Controls.Add(label1);
        Controls.Add(buttonChangeOrientation);
        Name = "ToolStripSeparatorPreferredSize";
        Text = "ToolStripSeparator GetPreferredSize";
        toolStrip1.ResumeLayout(false);
        toolStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    private System.Windows.Forms.ToolStripLabel toolStripLabel2;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonChangeOrientation;
}
