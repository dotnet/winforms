// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class ScalingBeforeChanges
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
        this.checkBox1 = new WinFormsControlsTest.MyCheckBox();
        this.SuspendLayout();
        //
        // checkBox1
        //
        this.checkBox1.AutoSize = true;
        this.checkBox1.Location = new System.Drawing.Point(39, 73);
        this.checkBox1.Name = "checkBox1";
        this.checkBox1.Size = new System.Drawing.Size(80, 17);
        this.checkBox1.TabIndex = 0;
        this.checkBox1.Text = "checkBox1";
        this.checkBox1.UseVisualStyleBackColor = true;
        //
        // Form1
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.ClientSize = new System.Drawing.Size(284, 261);
        this.Controls.Add(this.checkBox1);
        this.Name = "Form1";
        this.Text = "Form1";
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private MyCheckBox checkBox1;
}