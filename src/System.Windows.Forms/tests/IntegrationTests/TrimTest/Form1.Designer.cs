// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ScratchProject;

partial class Form1
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
        splitContainer1 = new SplitContainer();
        textBox1 = new TextBox();
        comboBox1 = new ComboBox();
        button1 = new Button();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        SuspendLayout();
        // 
        // splitContainer1
        // 
        splitContainer1.Dock = DockStyle.Fill;
        splitContainer1.Location = new Point(0, 0);
        splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        splitContainer1.Panel1.Controls.Add(textBox1);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(button1);
        splitContainer1.Panel2.Controls.Add(comboBox1);
        splitContainer1.Size = new Size(800, 450);
        splitContainer1.SplitterDistance = 457;
        splitContainer1.TabIndex = 0;
        // 
        // textBox1
        // 
        textBox1.AllowDrop = true;
        textBox1.Dock = DockStyle.Fill;
        textBox1.Location = new Point(0, 0);
        textBox1.Multiline = true;
        textBox1.Name = "textBox1";
        textBox1.Size = new Size(457, 450);
        textBox1.TabIndex = 0;
        textBox1.Text = "This is a multiline TextBox. Dragging text to it will replace the content.";
        textBox1.DragDrop += textBox1_DragDrop;
        textBox1.DragEnter += textBox1_DragEnter;
        textBox1.DragOver += textBox1_DragOver;
        // 
        // comboBox1
        // 
        comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        comboBox1.FormattingEnabled = true;
        comboBox1.Items.AddRange(new object[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" });
        comboBox1.Location = new Point(37, 37);
        comboBox1.Name = "comboBox1";
        comboBox1.Size = new Size(271, 23);
        comboBox1.TabIndex = 0;
        // 
        // button1
        // 
        button1.Location = new Point(76, 108);
        button1.Name = "button1";
        button1.Size = new Size(187, 46);
        button1.TabIndex = 1;
        button1.Text = "Copy ComboBox Text";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(splitContainer1);
        Name = "Form1";
        Text = "Form1";
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel1.PerformLayout();
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private SplitContainer splitContainer1;
    private TextBox textBox1;
    private ComboBox comboBox1;
    private Button button1;
}
