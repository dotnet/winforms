// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class TextBoxes
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
        this.textBox = new System.Windows.Forms.TextBox();
        this.RTLTextBox = new System.Windows.Forms.TextBox();
        this.multilineTextBox = new System.Windows.Forms.TextBox();
        this.RTLMultilineTextBox = new System.Windows.Forms.TextBox();
        this.richTextBox = new System.Windows.Forms.RichTextBox();
        this.RTLRichTextBox = new System.Windows.Forms.RichTextBox();
        this.SuspendLayout();
        // 
        // textBox
        // 
        this.textBox.Location = new System.Drawing.Point(20, 20);
        this.textBox.Name = "textBox";
        this.textBox.Size = new System.Drawing.Size(120, 30);
        this.textBox.TabIndex = 0;
        this.textBox.Text = "Some long long text for a TextBox";
        // 
        // RTLTextBox
        // 
        this.RTLTextBox.Location = new System.Drawing.Point(160, 20);
        this.RTLTextBox.Name = "RTLTextBox";
        this.RTLTextBox.Size = new System.Drawing.Size(120, 30);
        this.RTLTextBox.TabIndex = 1;
        this.RTLTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.RTLTextBox.Text = "Some long long text for a RTL TextBox";
        // 
        // multilineTextBox
        // 
        this.multilineTextBox.Location = new System.Drawing.Point(20, 60);
        this.multilineTextBox.Name = "multilineTextBox";
        this.multilineTextBox.Size = new System.Drawing.Size(120, 110);
        this.multilineTextBox.TabIndex = 2;
        this.multilineTextBox.Multiline = true;
        this.multilineTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.multilineTextBox.Text = @"Some long
long text
for

a multiline TextBox
that

contains several
lines
with

withspaces,tabs     and
numbers
12345";
        // 
        // RTLMultilineTextBox
        // 
        this.RTLMultilineTextBox.Location = new System.Drawing.Point(160, 60);
        this.RTLMultilineTextBox.Name = "RTLMultilineTextBox";
        this.RTLMultilineTextBox.Size = new System.Drawing.Size(120, 110);
        this.RTLMultilineTextBox.TabIndex = 3;
        this.RTLMultilineTextBox.Multiline = true;
        this.RTLMultilineTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.RTLMultilineTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.RTLMultilineTextBox.Text = @"Some long
long text
for

a multiline TextBox
that

contains several
lines
with

withspaces,tabs     and
numbers
12345";
        // 
        // richTextBox
        // 
        this.richTextBox.Location = new System.Drawing.Point(20, 190);
        this.richTextBox.Name = "richTextBox";
        this.richTextBox.Size = new System.Drawing.Size(120, 110);
        this.richTextBox.TabIndex = 4;
        this.richTextBox.Text = @"Some long
long text
for

a RichTextBox

that
contains several
lines
with

withspaces and
numbers
12345";
        // 
        // RTLRichTextBox
        // 
        this.RTLRichTextBox.Location = new System.Drawing.Point(160, 190);
        this.RTLRichTextBox.Name = "RTLRichTextBox";
        this.RTLRichTextBox.Size = new System.Drawing.Size(120, 110);
        this.RTLRichTextBox.TabIndex = 5;
        this.RTLRichTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.RTLRichTextBox.Text = @"Some long
long text
for

a RichTextBox

that
contains several
lines
with

withspaces and
numbers
12345";
        // 
        // Form
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(300, 330);
        this.Controls.Add(this.textBox);
        this.Controls.Add(this.RTLTextBox);
        this.Controls.Add(this.multilineTextBox);
        this.Controls.Add(this.RTLMultilineTextBox);
        this.Controls.Add(this.richTextBox);
        this.Controls.Add(this.RTLRichTextBox);
        this.Name = "TextBoxes";
        this.Text = "TextBoxes";
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox textBox;
    private System.Windows.Forms.TextBox RTLTextBox;
    private System.Windows.Forms.TextBox multilineTextBox;
    private System.Windows.Forms.TextBox RTLMultilineTextBox;
    private System.Windows.Forms.RichTextBox richTextBox;
    private System.Windows.Forms.RichTextBox RTLRichTextBox;
}

