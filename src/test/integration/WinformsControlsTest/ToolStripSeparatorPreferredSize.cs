// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
internal class ToolStripSeparatorPreferredSize : Form
{
    public ToolStripSeparatorPreferredSize()
    {
        InitializeComponent();
        _buttonChangeOrientation.Click += ChangeOrientation;
        SetLabelText();
    }

    private void ChangeOrientation(object sender, EventArgs e)
    {
        _toolStrip1.LayoutStyle = _toolStrip1.LayoutStyle == ToolStripLayoutStyle.VerticalStackWithOverflow
            ? ToolStripLayoutStyle.HorizontalStackWithOverflow
            : ToolStripLayoutStyle.VerticalStackWithOverflow;
        SetLabelText();
    }

    private void SetLabelText()
    {
        _label1.Text = $"Layout: {_toolStrip1.LayoutStyle}, Size: {_toolStripSeparator1.Size}, GetPreferredSize: {_toolStripSeparator1.GetPreferredSize(_toolStripSeparator1.Size)}";
    }

    private void InitializeComponent()
    {
        _toolStrip1 = new ToolStrip();
        _toolStripLabel1 = new ToolStripLabel();
        _toolStripLabel2 = new ToolStripLabel();
        _toolStripSeparator1 = new ToolStripSeparator();
        _buttonChangeOrientation = new Button();
        _label1 = new Label();
        _toolStrip1.SuspendLayout();
        SuspendLayout();
        //
        // toolStrip1
        //
        _toolStrip1.Items.AddRange((ToolStripItem[])
        [
            _toolStripLabel1,
            _toolStripSeparator1,
            _toolStripLabel2
        ]);
        _toolStrip1.Location = new System.Drawing.Point(0, 0);
        _toolStrip1.Name = "toolStrip1";
        _toolStrip1.Size = new System.Drawing.Size(481, 22);
        _toolStrip1.TabIndex = 0;
        _toolStrip1.Text = "toolStrip1";
        _toolStrip1.TabStop = true;
        //
        // toolStripLabel1
        //
        _toolStripLabel1.Name = "toolStripLabel1";
        _toolStripLabel1.Size = new System.Drawing.Size(86, 22);
        _toolStripLabel1.Text = "ToolStripLabel";
        //
        // toolStripLabel2
        //
        _toolStripLabel2.Name = "toolStripLabel2";
        _toolStripLabel2.Size = new System.Drawing.Size(86, 22);
        _toolStripLabel2.Text = "ToolStripLabel";
        //
        // buttonChangeOrientation
        //
        _buttonChangeOrientation.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _buttonChangeOrientation.Name = "buttonChangeOrientation";
        _buttonChangeOrientation.Location = new System.Drawing.Point(10, 75);
        _buttonChangeOrientation.Size = new System.Drawing.Size(150, 25);
        _buttonChangeOrientation.TabIndex = 0;
        _buttonChangeOrientation.Text = "Change Orientation";
        _buttonChangeOrientation.UseVisualStyleBackColor = true;
        //
        // label1
        //
        _label1.Name = "label1";
        _label1.Location = new System.Drawing.Point(10, 50);
        _label1.AutoSize = true;
        //
        // Form1
        //
        AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        AutoScaleMode = AutoScaleMode.Font;
        Size = new System.Drawing.Size(881, 150);
        Controls.Add(_toolStrip1);
        Controls.Add(_label1);
        Controls.Add(_buttonChangeOrientation);
        Name = "ToolStripSeparatorPreferredSize";
        Text = "ToolStripSeparator GetPreferredSize";
        _toolStrip1.ResumeLayout(false);
        _toolStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private ToolStrip _toolStrip1;
    private ToolStripLabel _toolStripLabel1;
    private ToolStripLabel _toolStripLabel2;
    private ToolStripSeparator _toolStripSeparator1;
    private Label _label1;
    private Button _buttonChangeOrientation;
}
