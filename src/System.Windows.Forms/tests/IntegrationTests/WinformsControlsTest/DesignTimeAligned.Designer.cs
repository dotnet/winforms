// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class DesignTimeAligned
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
        this.label1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.button1 = new System.Windows.Forms.Button();
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.currentDPILabel1 = new WinFormsControlsTest.CurrentDPILabel();
        this.SuspendLayout();
        //
        // label1
        //
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(72, 18);
        this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(36, 25);
        this.label1.TabIndex = 0;
        this.label1.Text = "LL";
        this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        //
        // label2
        //
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(72, 76);
        this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(36, 25);
        this.label2.TabIndex = 2;
        this.label2.Text = "LL";
        this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        //
        // button1
        //
        this.button1.Location = new System.Drawing.Point(122, 66);
        this.button1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(60, 46);
        this.button1.TabIndex = 3;
        this.button1.Text = "LL";
        this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.button1.UseVisualStyleBackColor = true;
        //
        // textBox1
        //
        this.textBox1.Location = new System.Drawing.Point(122, 12);
        this.textBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.textBox1.Name = "textBox1";
        this.textBox1.Size = new System.Drawing.Size(56, 31);
        this.textBox1.TabIndex = 4;
        this.textBox1.Text = "LLL";
        //
        // currentDPILabel1
        //
        this.currentDPILabel1.AutoSize = true;
        this.currentDPILabel1.Location = new System.Drawing.Point(25, 159);
        this.currentDPILabel1.Name = "currentDPILabel1";
        this.currentDPILabel1.Size = new System.Drawing.Size(178, 25);
        this.currentDPILabel1.TabIndex = 5;
        this.currentDPILabel1.Text = "currentDPILabel1";
        //
        // DesignTimeAligned
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.ClientSize = new System.Drawing.Size(490, 196);
        this.Controls.Add(this.currentDPILabel1);
        this.Controls.Add(this.textBox1);
        this.Controls.Add(this.button1);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.label1);
        this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
        this.Name = "DesignTimeAligned";
        this.Text = "DesignTime Aligned";
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TextBox textBox1;
    private CurrentDPILabel currentDPILabel1;
}